/*
Function: 		First person camera controller allows movementVector in any XZ targetHeadingPosition (no y-axis movementVector is allowed)
Author: 		NMCG
Adapted:        JF
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class FirstPersonCameraController : UserInputController
    {
        #region Fields
        private Vector3 translation;
        private Vector3 rotation;
        private Vector3 targetPosition;
        private Vector3 currentPosition;
        private Vector3 targetHeading;
        private Vector3 currentHeading;
        private Vector3 movementVector;
        private Vector3 rotationVector;
        private bool inMotion = false;
        #endregion

        #region Properties
        public Vector3 Translation
        {
            get
            {
                return this.translation;
            }
            set
            {
                this.translation = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                return this.targetPosition;
            }
            set
            {
                this.targetPosition = value;
            }
        }

        public Vector3 CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.currentPosition = value;
            }
        }

        public Vector3 TargetHeading
        {
            get
            {
                return this.targetHeading;
            }
            set
            {
                this.targetHeading = value;
            }
        }

        public Vector3 CurrentHeading
        {
            get
            {
                return this.currentHeading;
            }
            set
            {
                this.currentHeading = value;
            }
        }

        public Vector3 MovementVector
        {
            get
            {
                return this.movementVector;
            }
            set
            {
                this.movementVector = value;
            }
        }

        public Vector3 RotationVector
        {
            get
            {
                return this.rotationVector;
            }
            set
            {
                this.rotationVector = value;
            }
        }

        public bool InMotion
        {
            get
            {
                return this.inMotion;
            }
            set
            {
                this.inMotion = value;
            }
        }
        #endregion

        #region Constructor
        public FirstPersonCameraController(
            string id, 
            ControllerType controllerType, 
            Keys[] moveKeys, 
            float moveSpeed,
            float rotationSpeed, 
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector
        ) : base(id, controllerType, moveKeys, moveSpeed, rotationSpeed, managerParameters) {
            this.movementVector = movementVector;
            this.rotationVector = rotationVector;
        }
        #endregion

        #region Methods
        public override void HandleGamePadInput(GameTime gameTime, Actor3D parentActor) { }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parentActor) { }

        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor) {

            #region Translation
            //Forward, Back
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.inMotion)
            {
                //Calculate target position, relative to the camera
                targetPosition = (parentActor.Transform.Look * movementVector);
                translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.inMotion)
            {
                //Calculate target position, relative to the camera
                targetPosition = -(parentActor.Transform.Look * movementVector);
                translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
            }

            //Left, Right
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.inMotion)
            {
                //Calculate target position, relative to the camera
                targetPosition = -(parentActor.Transform.Right * movementVector);
                translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.inMotion)
            {
                //Calculate target position, relative to the camera
                targetPosition = (parentActor.Transform.Right * movementVector);
                translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
            }
            #endregion

            #region Rotation
            //Rotate
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.inMotion)
            {
                //Rotate the camera anti-clockwise
                targetHeading = (parentActor.Transform.Up * rotationVector);
                rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.inMotion)
            {
                //Rotate the camera clockwise
                targetHeading = -(parentActor.Transform.Up * rotationVector);
                rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }
            #endregion
        }

        public virtual void HandleMovement(Actor3D parentActor)
        {
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            HandleMovement(parentActor);

            base.Update(gameTime, actor);
        }
        #endregion
    }
}