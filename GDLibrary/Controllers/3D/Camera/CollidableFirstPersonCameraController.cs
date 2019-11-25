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
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector
        ) : base(id, controllerType, moveKeys, moveSpeed, rotateSpeed, managerParameters, movementVector, rotationVector) {
        }
        #endregion
        
        #region Methods
        //Handle Keyboard Input
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            return;

            if (parentActor != null)
            {
                #region Rotation
                //Anti-Clockwise
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.InMotion)
                {
                    //Calculate target heading, relative to the camera
                    this.TargetHeading = (parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }

                //Clockwise
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.InMotion)
                {
                    //Calculate target heading, relative to the camera
                    this.TargetHeading = -(parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                #endregion

                #region Translation
                //Forward
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Look * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }

                //Back
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Look * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }

                //Left
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Right * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }

                //Right
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Right * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                #endregion
            }
        }

        //Handle Movement
        public override void HandleMovement(Actor3D parentActor)
        {
            return;

            #region Rotation
            if (this.Rotation != Vector3.Zero)
            {
                //If the current heading is near the target heading
                if (Vector3.Distance(this.CurrentHeading, this.TargetHeading) <= 5)
                {
                    //Rotate to the target heading
                    parentActor.Transform.RotateBy((this.CurrentHeading - this.TargetHeading) * -Vector3.One);

                    //Reset vectors
                    this.Rotation = Vector3.Zero;
                    this.CurrentHeading = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;
                }
                else
                {
                    //Rotate actor
                    parentActor.Transform.RotateBy(this.Rotation);

                    //Update current heading
                    this.CurrentHeading += this.Rotation;

                    //Update motion state
                    this.InMotion = true;
                }

                //Prevents multiple movement actions from happening every update
                return;
            }
            #endregion

            #region Translation
            if (this.Translation != Vector3.Zero)
            {
                //If the current positon is near the target position
                if (Vector3.Distance(this.CurrentPosition, this.TargetPosition) <= 10)
                {
                    //Move to the target position
                    parentActor.Transform.TranslateBy((this.CurrentPosition - this.TargetPosition) * -Vector3.One);

                    //Reset Vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;
                }
                else
                {
                    //Translate actor
                    parentActor.Transform.TranslateBy(this.Translation);

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Update motion state
                    this.InMotion = true;
                }

                //Prevents multiple movement actions from happening every update
                return;
            }
            #endregion
        }

        //Update
        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
        }
        #endregion
    }
}