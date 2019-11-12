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

        private Vector3 translation;
        private Vector3 rotation;

        private Vector3 targetHeading;
        private Vector3 targetPosition;
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
        #endregion

        #region Constructors 
        //Uses the default PlayerObject as the collidable object for the camera
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotationSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            Vector3 translationOffset
        ) : this(
            id,
            controllerType,
            moveKeys,
            moveSpeed,
            strafeSpeed,
            rotationSpeed,
            managerParameters,
            movementVector,
            rotationVector,
            parentActor,
            width,
            height,
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
            float rotationSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            Vector3 translationOffset,
            PlayerObject collidableObject
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotationSpeed, managerParameters, movementVector, rotationVector) {
            this.width = width;
            this.height = height;

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
                this.playerObject = new PlayerObject(
                    this.ID + " - Player Object",
                    ActorType.CollidableCamera,
                    StatusType.Off,
                    (parentActor as Actor3D).Transform,
                    null,
                    null,
                    this.MoveKeys,
                    width,
                    height,
                    jumpHeight,
                    translationOffset,
                    this.ManagerParameters.KeyboardManager
                );
            }

            playerObject.Enable(false, mass);
        }
        #endregion

        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
        }

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
                    targetPosition = (parentActor.Transform.Look * this.MovementVector);
                    this.playerObject.CharacterBody.Velocity += (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    targetPosition = -(parentActor.Transform.Look * this.MovementVector);
                    this.playerObject.CharacterBody.Velocity -= (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }
                else
                {
                    this.playerObject.CharacterBody.DesiredVelocity = Vector3.Zero;
                }

                //Left, Right
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    targetPosition = -(parentActor.Transform.Right * this.MovementVector);
                    this.playerObject.CharacterBody.Velocity -= (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    targetPosition = (parentActor.Transform.Right * this.MovementVector);
                    this.playerObject.CharacterBody.Velocity += (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                else
                {
                    this.playerObject.CharacterBody.DesiredVelocity = Vector3.Zero;
                }
                #endregion

                //Update the camera position to reflect the collision skin position
                //Use the offset to move camera from centre of collision skin (capsule)
                //To higher within the capsule (i.e. eye level versus belly button level).
                parentActor.Transform.Translation = this.playerObject.CharacterBody.Position + translationOffset;

                #region Rotation
                //Rotate
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.InMotion)
                {
                    //Rotate the camera anti-clockwise
                    targetHeading = (parentActor.Transform.Up * this.RotationVector);
                    rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.InMotion)
                {
                    //Rotate the camera clockwise
                    targetHeading = -(parentActor.Transform.Up * this.RotationVector);
                    rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                #endregion
            }
        }
    }
}