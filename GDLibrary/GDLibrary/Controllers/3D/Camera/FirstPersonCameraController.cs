/*
Function: 		First person camera controller allows movementVector in any XZ terminal (no y-axis movementVector is allowed)
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
        Vector3 angle;
        Vector3 target;
        Vector3 rotation;
        Vector3 position;
        Vector3 distance;
        Vector3 terminal;
        Vector3 movementVector = new Vector3(100, 100, 100) * 2.54f;
        Vector3 rotationVector = new Vector3(0, 90, 0);
        bool inMotion = false;
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
            ////Forward, Back
            //if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.inMotion)
            //{
            //    //Move the camera in the terminal of look vector 
            //    terminal = (parentActor.Transform.Look * movementVector);
            //    this.inMotion = true;

            //}
            //else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.inMotion)
            //{
            //    //Move the camera in the opposite terminal of look vector
            //    terminal = -(parentActor.Transform.Look * movementVector);
            //    this.inMotion = true;
            //}

            ////Left, Right
            //if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.inMotion)
            //{
            //    //Move the camera in the opposite terminal of right vector
            //    terminal = -(parentActor.Transform.Right * movementVector);
            //    this.inMotion = true;
            //}
            //else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.inMotion)
            //{
            //    //Move the cmaera in the terminal of right vector
            //    terminal = (parentActor.Transform.Right * movementVector);
            //    this.inMotion = true;
            //}

            ////Rotate
            //if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.inMotion)
            //{
            //    parentActor.Transform.RotateBy(rotationVector);
            //    this.inMotion = true;
            //}
            //else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.inMotion)
            //{
            //    parentActor.Transform.RotateBy(-rotationVector);
            //    this.inMotion = true;
            //}

            //if (!this.ManagerParameters.KeyboardManager.IsAnyinMotion()) this.inMotion = false;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;

            //Forward, Back
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.inMotion)
            {
                //Move the camera in the terminal of look vector 
                terminal = (parentActor.Transform.Look * movementVector);
                distance = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.inMotion)
            {
                //Move the camera in the opposite terminal of look vector
                terminal = -(parentActor.Transform.Look * movementVector);
                distance = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
            }

            //Left, Right
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.inMotion)
            {
                //Move the camera in the opposite terminal of right vector
                terminal = -(parentActor.Transform.Right * movementVector);
                distance = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.inMotion)
            {
                //Move the cmaera in the terminal of right vector
                terminal = (parentActor.Transform.Right * movementVector);
                distance = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
            }

            //If the camera is moving
            if (distance != Vector3.Zero)
            {
                //If we have reached or passed the terminal
                if (Vector3.Distance(terminal, position) <= 10)
                {
                    //Move to the terminal
                    parentActor.Transform.TranslateBy(((position - terminal) * -Vector3.One));

                    //Reset Vectors
                    distance = Vector3.Zero;
                    position = Vector3.Zero;
                    
                    //Allow keypress
                    this.inMotion = false;
                }
                else
                {                    
                    //Move camera 
                    parentActor.Transform.TranslateBy(distance);

                    //Update position
                    position += distance;

                    //Prevent keypress
                    this.inMotion = true;
                }
            }

            //Rotate
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.inMotion)
            {
                //Rotate the camera anti-clockwise
                target = (parentActor.Transform.Up * rotationVector);
                rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.inMotion)
            {
                //Rotate the camera clockwise
                target = -(parentActor.Transform.Up * rotationVector);
                rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
            }

            //If the camera is rotating
            if (rotation != Vector3.Zero)
            {

                float ex = Vector3.Distance(angle, target);
                //If the camera is looking at or passed the target
                if (Vector3.Distance(angle, target) <= 5)
                {
                    //Point camera at target
                    parentActor.Transform.RotateBy((angle - target) * -Vector3.One);

                    //Reset vectors
                    rotation = Vector3.Zero;
                    angle = Vector3.Zero;

                    //Allow keypress
                    inMotion = false;
                }
                else
                {
                    //Rotate camera
                    parentActor.Transform.RotateBy(rotation);

                    //Update angle
                    angle += rotation;

                    //Prevent keypress
                    inMotion = true;
                }
            }
        }

        //Add Equals, Clone, ToString, GetHashCode...
        #endregion
    }
}