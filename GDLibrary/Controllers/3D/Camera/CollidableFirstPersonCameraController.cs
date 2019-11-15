/*
Function: 		First person COLLIDABLE camera controller.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// A collidable camera has a body and collision skin from a player object but it has no modeldata or texture
    /// </summary>
    public class CollidableFirstPersonCameraController : FirstPersonCameraController
    {
        #region Fields
        private PlayerObject playerObject;
        private float width;
        private float height;
        private float mass;
        private float jumpHeight;
        private float accelerationRate;
        private float decelerationRate;
        private Vector3 translationOffset;
        #endregion

        #region Properties
        public PlayerObject PlayerObject
        {
            get
            {
                return this.playerObject;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = (value > 0) ? value : 1;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = (value > 0) ? value : 1;
            }
        }

        public float Mass
        {
            get
            {
                return this.mass;
            }
            set
            {
                this.mass = (value > 0) ? value : 1;
            }
        }

        public float JumpHeight
        {
            get
            {
                return this.jumpHeight;
            }
            set
            {
                this.jumpHeight = (value > 0) ? value : 1;
            }
        }

        public float AccelerationRate
        {
            get
            {
                return this.accelerationRate;
            }
            set
            {
                this.accelerationRate = (value != 0) ? value : 1;
            }
        }

        public float DecelerationRate
        {
            get
            {
                return this.decelerationRate;
            }
            set
            {
                this.decelerationRate = (value != 0) ? value : 1;
            }
        }
        #endregion

        #region Constructors 
        //Uses the default PlayerObject as the collidable object for the camera
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            float depth,
            float accelerationRate,
            float decelerationRate,
            float mass,
            float jumpHeight,
            Vector3 translationOffset
        ) : this(
            id,
            controllerType,
            moveKeys,
            moveSpeed,
            strafeSpeed,
            rotateSpeed,
            managerParameters,
            movementVector,
            rotationVector,
            parentActor,
            width,
            height,
            depth,
            accelerationRate,
            decelerationRate,
            mass,
            jumpHeight,
            translationOffset,
            null
        ) {
        }

        //Allows developer to specify the type of collidable object to be used as basis for the camera
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            float depth,
            float accelerationRate, 
            float decelerationRate,
            float mass, 
            float jumpHeight, 
            Vector3 translationOffset,
            PlayerObject collidableObject
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotateSpeed, managerParameters, movementVector, rotationVector) {
            this.width = width;
            this.height = height;
            this.AccelerationRate = accelerationRate;
            this.DecelerationRate = decelerationRate;
            this.Mass = mass;
            this.JumpHeight = jumpHeight;

            //Allows us to tweak the camera position within the player object 
            this.translationOffset = translationOffset;

            /* Create the collidable player object which comes with a collision skin and position the parentActor (i.e. the camera) inside the player object.
             * notice that we don't pass any effect, model or texture information, since in 1st person perspective we dont want to look from inside a model.
             * Therefore, we wont actually render any drawn object - the effect, texture, model (and also Color) information are unused.
             * 
             * This code allows the user to pass in their own PlayerObject (e.g. HeroPlayerObject) to be used for the collidable object basis for the camera.
             */
            if (collidableObject != null)
            {
                this.playerObject = collidableObject;
            }
            else
            {
                Transform3D transform = (parentActor as Actor3D).Transform;

                float health = 100;
                float attack = 100;
                float defence = 100;

                this.playerObject = new PlayerObject(
                    this.ID + " - Player Object",
                    ActorType.CollidableCamera,
                    transform,
                    null,
                    null,
                    width,
                    height,
                    depth,
                    accelerationRate,
                    decelerationRate,
                    movementVector,
                    rotationVector,
                    moveSpeed,
                    rotateSpeed,
                    this.MoveKeys,
                    translationOffset,
                    this.ManagerParameters.KeyboardManager,
                    jumpHeight,
                    health,
                    attack,
                    defence
                );
            }

            playerObject.Enable(false, mass);
        }
        #endregion

        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            /* Notice in the code below that we are NO LONGER simply changing the camera translation value. 
             * Since this is now a collidable camera we, instead, modify the camera position by calling the PlayerObject move methods.
             * 
             * Q. Why do we still use the rotation methods of Transform3D? 
             * A. Rotating the camera doesnt affect CD/CR since the camera is modelled by a player object which has a capsule shape.
             *    A capsule's collision response won't alter as a result of any rotation since its cross-section is spherical across the XZ-plane.
             */

            if (parentActor != null)
            {
                #region Translation
                //Forward, Back
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Look * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Look * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }

                //Left, Right
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Right * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Right * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                #endregion

                #region Rotation
                //Rotate
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.InMotion)
                {
                    //Rotate the camera anti-clockwise
                    this.TargetHeading = (parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.InMotion)
                {
                    //Rotate the camera clockwise
                    this.TargetHeading = -(parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                #endregion
            }
        }

        public override void HandleMovement(Actor3D parentActor)
        {
            #region Translation
            if (this.Translation != Vector3.Zero)
            {
                if (!this.InMotion)
                {
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "environment_stone_steps" }
                        )
                    );

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.EnemyTurn,
                            EventCategoryType.Game
                        )
                    );
                }

                //If the cameras current current positon is near the target position
                if (Vector3.Distance(this.TargetPosition, this.CurrentPosition) <= 10)
                {
                    //Move the camera to the target position
                    parentActor.Transform.TranslateBy(((this.CurrentPosition - this.TargetPosition) * -Vector3.One));

                    //Update collision skin
                    this.PlayerObject.Transform.Translation = parentActor.Transform.Translation;

                    //Reset Vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Allow keypress
                    this.InMotion = false;
                }
                else
                {
                    //Move camera 
                    parentActor.Transform.TranslateBy(this.Translation);

                    //Update collision skin
                    this.PlayerObject.Transform.Translation = parentActor.Transform.Translation;

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Prevent keypress
                    this.InMotion = true;
                }
            }
            #endregion

            #region Rotation
            if (this.Rotation != Vector3.Zero)
            {
                if (!this.InMotion)
                {
                    object[] additionalParameters = { "turn_around" };

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            additionalParameters
                        )
                    );
                }

                //If the cameras heading is near the target heading
                if (Vector3.Distance(this.CurrentHeading, this.TargetHeading) <= 5)
                {
                    //Point camera at the target
                    parentActor.Transform.RotateBy((this.CurrentHeading - this.TargetHeading) * -Vector3.One);

                    //Reset vectors
                    this.Rotation = Vector3.Zero;
                    this.CurrentHeading = Vector3.Zero;

                    //Allow keypress
                    this.InMotion = false;
                }
                else
                {
                    //Rotate camera
                    parentActor.Transform.RotateBy(this.Rotation);

                    //Update current heading
                    this.CurrentHeading += this.Rotation;

                    //Prevent keypress
                    this.InMotion = true;
                }
            }
            #endregion
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
        }
        #endregion
    }
}