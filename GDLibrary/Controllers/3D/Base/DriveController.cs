/*
Function: 		Creates a simple controller for drivable objects taking input from the keyboard
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class DriveController : UserInputController
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        public DriveController(
            string id, 
            ControllerType controllerType, 
            Keys[] moveKeys, 
            float moveSpeed, 
            float strafeSpeed, 
            float rotationSpeed, 
            ManagerParameters managerParameters
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotationSpeed, managerParameters) {

        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
        }

        Vector3 translation;
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            translation = Vector3.Zero;

            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]))
            {
                translation = gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look;
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]))
            {
                translation = -gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look;
            }

            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]))
            {
                //What's the significance of the +=? Remove it and see if we can move forward/backward AND strafe.
                translation += -gameTime.ElapsedGameTime.Milliseconds * this.StrafeSpeed * parentActor.Transform.Right;
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]))
            {
                //What's the significance of the +=? Remove it and see if we can move forward/backward AND strafe.
                translation += gameTime.ElapsedGameTime.Milliseconds * this.StrafeSpeed * parentActor.Transform.Right;
            }

            //rotate
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]))
            {
                parentActor.Transform.RotateBy(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]))
            {
                parentActor.Transform.RotateBy(-gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }

            //Was a move button(s) pressed?
            if (translation != Vector3.Zero)
            {
                //remove y-axis component of the translation
                translation.Y = 0;
                parentActor.Transform.TranslateBy(translation);
            }
        }

        //Add Equals, Clone, ToString, GetHashCode...
    }
}