/*
Function: 		First person camera controller allows distanceVector in any XZ direction (no y-axis distanceVector is allowed)
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    public class FirstPersonCameraController : UserInputController
    {
        #region Fields
        Vector3 translation;
        Vector3 distanceVector = new Vector3(100, 100, 100) * 2.54f;
        Vector3 rotateVector = new Vector3(0, 90, 0);
        bool keyPressed = false;
        #endregion

        #region Properties
        #endregion

        #region Methods
        public FirstPersonCameraController(
            string id, 
            ControllerType controllerType, 
            Keys[] moveKeys, 
            float moveSpeed, 
            float strafeSpeed, 
            float rotationSpeed, 
            ManagerParameters managerParameters
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotationSpeed, managerParameters) {

        }

        public override void HandleGamePadInput(GameTime gameTime, Actor3D parentActor)
        {
            //only override this method if we want to use the gamepad
            //if (this.gamePadManager.IsButtonPressed(PlayerIndex.One, Buttons.RightTrigger))
            //{
            //    //do something....
            //}
        }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parentActor)
        {
        }

        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            translation = Vector3.Zero;

            //Forward, Back
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.keyPressed)
            {
                //Move the camera in the direction of look vector
                translation = (parentActor.Transform.Look * distanceVector);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.keyPressed)
            {
                //Move the camera in the opposite direction of look vector 
                translation = -(parentActor.Transform.Look * distanceVector);
            }

            //Left, Right
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.keyPressed)
            {
                //What's the significance of the +=? Remove it and see if we can move forward/backward AND strafe.
                translation = -(parentActor.Transform.Right * distanceVector);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.keyPressed)
            {
                //What's the significance of the +=? Remove it and see if we can move forward/backward AND strafe.
                translation = (parentActor.Transform.Right * distanceVector);
            }

            //Rotate
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.keyPressed)
            {
                parentActor.Transform.RotateBy(rotateVector);
                this.keyPressed = true;
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.keyPressed)
            {
                parentActor.Transform.RotateBy(-rotateVector);
                this.keyPressed = true;
            }

            //Was a move button(s) pressed?
            if (translation != Vector3.Zero)
            {
                translation.Y = 0;
                parentActor.Transform.TranslateBy(translation);
                this.keyPressed = true;
            }

            if (!this.ManagerParameters.KeyboardManager.IsAnyKeyPressed()) this.keyPressed = false;
        }

        //Add Equals, Clone, ToString, GetHashCode...
        #endregion
    }
}