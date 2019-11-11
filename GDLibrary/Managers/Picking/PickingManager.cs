using System;
using Microsoft.Xna.Framework;
using JigLibX.Physics;

namespace GDLibrary
{
    public class PickingManager : PausableGameComponent
    {
        protected static readonly string NoObjectSelectedText = "no object selected";
        protected static readonly float DefaultMinPickPlaceDistance = 20;
        protected static readonly float DefaultMaxPickPlaceDistance = 100;
        private static readonly int DefaultDistanceToTargetPrecision = 1;

        private ManagerParameters managerParameters;
        private CameraManager cameraManager;
        private float pickStartDistance;
        private float pickEndDistance;
        private Predicate<CollidableObject> collisionPredicate;
        private PickingBehaviourType pickingBehaviourType;

        //local vars
        private CollidableObject currentPickedObject;
        private Vector3 pos, normal;
        private float distanceToObject;
        private Camera3D camera;
        private float cameraPickDistance;
        private bool bCurrentlyPicking;
        private ConstraintWorldPoint objectController = new ConstraintWorldPoint();
        private ConstraintVelocity damperController = new ConstraintVelocity();

        public PickingManager(Game game, EventDispatcher eventDispatcher, StatusType statusType,
           ManagerParameters managerParameters, CameraManager cameraManager,
           PickingBehaviourType pickingBehaviourType, float pickStartDistance, float pickEndDistance, Predicate<CollidableObject> collisionPredicate)
           : base(game, eventDispatcher, statusType)
        {
            this.managerParameters = managerParameters;
            this.cameraManager = cameraManager;
            this.pickingBehaviourType = pickingBehaviourType;
            this.pickStartDistance = pickStartDistance;
            this.pickEndDistance = pickEndDistance;
            this.collisionPredicate = collisionPredicate;
        }

        #region Event Handling 
        #endregion

        protected override void HandleInput(GameTime gameTime)
        {
            HandleMouse(gameTime);
            HandleKeyboard(gameTime);
            HandleGamePad(gameTime);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //listen to input and check for picking from mouse and any input from gamepad and keyboard
            HandleInput(gameTime);

            base.ApplyUpdate(gameTime);
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            if (this.pickingBehaviourType == PickingBehaviourType.PickAndPlace)
                DoPickAndPlace(gameTime);
            else if (this.pickingBehaviourType == PickingBehaviourType.PickAndRemove)
                DoPickAndRemove(gameTime);
            else
                DoPick(gameTime);
        }

        private void DoPick(GameTime gameTime)
        {
            this.camera = this.cameraManager.ActiveCamera;
            this.currentPickedObject = this.managerParameters.MouseManager.GetPickedObject(camera, camera.ViewportCentre,
                this.pickStartDistance, this.pickEndDistance, out pos, out normal) as CollidableObject;

            if (this.currentPickedObject != null)
            {
                //Publish an event for the HUD because we're mousing over something pickable!
            }
        }

        private void DoPickAndRemove(GameTime gameTime)
        {
            if (this.managerParameters.MouseManager.IsLeftButtonClickedOnce())
            {
                this.camera = this.cameraManager.ActiveCamera;
                this.currentPickedObject = this.managerParameters.MouseManager.GetPickedObject(camera, camera.ViewportCentre,
                    this.pickStartDistance, this.pickEndDistance, out pos, out normal) as CollidableObject;

                if (this.currentPickedObject != null)
                {
                    //generate event to tell object manager and physics manager to remove the object
                    EventDispatcher.Publish(new EventData(this.currentPickedObject, EventActionType.OnRemoveActor, EventCategoryType.SystemRemove));
                }
            }
        }

        private void DoPickAndPlace(GameTime gameTime)
        {
            if (this.managerParameters.MouseManager.IsMiddleButtonClicked())
            {
                if (!this.bCurrentlyPicking)
                {
                    this.camera = this.cameraManager.ActiveCamera;
                    this.currentPickedObject = this.managerParameters.MouseManager.GetPickedObject(camera, camera.ViewportCentre,
                        this.pickStartDistance, this.pickEndDistance, out pos, out normal) as CollidableObject;

                    this.distanceToObject = (float)Math.Round(Vector3.Distance(camera.Transform.Translation, pos), DefaultDistanceToTargetPrecision);

                    if (this.currentPickedObject != null)// && IsValidCollision(currentPickedObject, pos, normal))
                    {
                        Vector3 vectorDeltaFromCentreOfMass = pos - this.currentPickedObject.Collision.Owner.Position;
                        vectorDeltaFromCentreOfMass = Vector3.Transform(vectorDeltaFromCentreOfMass, Matrix.Transpose(this.currentPickedObject.Collision.Owner.Orientation));
                        cameraPickDistance = (this.cameraManager.ActiveCamera.Transform.Translation - pos).Length();

                        //remove any controller from any previous pick-release 
                        objectController.Destroy();
                        damperController.Destroy();

                        this.currentPickedObject.Collision.Owner.SetActive();
                        //move object by pos (i.e. point of collision and not centre of mass)
                        this.objectController.Initialise(this.currentPickedObject.Collision.Owner, vectorDeltaFromCentreOfMass, pos);
                        //dampen velocity (linear and angular) on object to Zero
                        this.damperController.Initialise(this.currentPickedObject.Collision.Owner, ConstraintVelocity.ReferenceFrame.Body, Vector3.Zero, Vector3.Zero);
                        this.objectController.EnableConstraint();
                        this.damperController.EnableConstraint();
                        //we're picking a valid object for the first time
                        this.bCurrentlyPicking = true;

                        //update mouse text
                        object[] additionalParameters = { currentPickedObject, this.distanceToObject };
                        EventDispatcher.Publish(new EventData(EventActionType.OnObjectPicked, EventCategoryType.ObjectPicking, additionalParameters));
                    }
                }

                //if we have an object picked from the last update then move it according to the mouse pointer
                if (objectController.IsConstraintEnabled && (objectController.Body != null))
                {
                    // Vector3 delta = objectController.Body.Position - this.managerParameters.CameraManager.ActiveCamera.Transform.Translation;
                    Vector3 direction = this.managerParameters.MouseManager.GetMouseRay(this.cameraManager.ActiveCamera).Direction;
                    cameraPickDistance += this.managerParameters.MouseManager.GetDeltaFromScrollWheel() * 0.1f;
                    Vector3 result = this.cameraManager.ActiveCamera.Transform.Translation + cameraPickDistance * direction;
                    //set the desired world position
                    objectController.WorldPosition = this.cameraManager.ActiveCamera.Transform.Translation + cameraPickDistance * direction;
                    objectController.Body.SetActive();
                }
            }
            else //releasing object
            {
                if (this.bCurrentlyPicking)
                {
                    //release object from constraints and allow to behave as defined by gravity etc
                    objectController.DisableConstraint();
                    damperController.DisableConstraint();

                    //notify listeners that we're no longer picking
                    object[] additionalParameters = { NoObjectSelectedText };
                    EventDispatcher.Publish(new EventData(EventActionType.OnNonePicked, EventCategoryType.ObjectPicking, additionalParameters));

                    this.bCurrentlyPicking = false;
                }
            }
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
        }

        protected override void HandleGamePad(GameTime gameTime)
        {
        }

        //called when over collidable/pickable object
        //protected virtual bool IsValidCollision(CollidableObject collidableObject, Vector3 pos, Vector3 normal)
        //{
        //    //if not null then call method to see if its an object that conforms to our predicate (e.g. ActorType::CollidablePickup), otherwise return false
        //    return (collidableObject != null) ? this.collisionPredicate(collidableObject) : false;
        //}
    }
}
