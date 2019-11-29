using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// Represents your MOVEABLE player in the game. 
    /// </summary>
    public class PlayerObject : CharacterObject
    {
        #region Variables
        private PlayerIndex playerIndex;
        private Buttons[] moveButtons;
        private Keys[] moveKeys;
        #endregion

        #region Properties
        public PlayerIndex PlayerIndex
        {
            get
            {
                return this.playerIndex;
            }
            set
            {
                this.playerIndex = value;
            }
        }

        public Buttons[] MoveButtons
        {
            get
            {
                return this.moveButtons;
            }
            set
            {
                this.moveButtons = value;
            }
        }

        public Keys[] MoveKeys
        {
            get
            {
                return this.moveKeys;
            }
            set
            {
                this.moveKeys = value;
            }
        }
        #endregion

        #region Constructor
        public PlayerObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            ManagerParameters managerParameters,
            PlayerIndex playerIndex,
            Buttons[] moveButtons,
            Keys[] moveKeys
        ) : base(id, actorType, transform, effectParameters, model, accelerationRate, decelerationRate, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence, managerParameters) {
            this.PlayerIndex = playerIndex;
            this.MoveButtons = moveButtons;
            this.MoveKeys = moveKeys;
        }
        #endregion

        #region Methods
        public virtual void HandleKeyboardInput(GameTime gameTime)
        {
            #region Rotation
            //Anti-Clockwise
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.InMotion)
            {
                //Calculate target heading, relative to the camera
                this.TargetHeading = (this.Transform.Up * this.RotationVector);
                this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                return;
            }

            //Clockwise
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.InMotion)
            {
                //Calculate target heading, relative to the camera
                this.TargetHeading = -(this.Transform.Up * this.RotationVector);
                this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                return;
            }
            #endregion

            #region Translation
            //Forward
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = (this.Transform.Look * this.MovementVector);
                this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Look);
                return;
            }

            //Back
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = -(this.Transform.Look * this.MovementVector);
                this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Look);
                return;
            }

            //Left
            if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = -(this.Transform.Right * this.MovementVector);
                this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                return;
            }

            //Right
            else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = (this.Transform.Right * this.MovementVector);
                this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                return;
            }
            #endregion
        }

        public virtual void HandleGamepadInput(GameTime gameTime)
        {
            #region Rotation
            //Anti-Clockwise
            if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[4]) && !this.InMotion)
            {
                //Calculate target heading, relative to the camera
                this.TargetHeading = (this.Transform.Up * this.RotationVector);
                this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                return;
            }

            //Clockwise
            else if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[5]) && !this.InMotion)
            {
                //Calculate target heading, relative to the camera
                this.TargetHeading = -(this.Transform.Up * this.RotationVector);
                this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotateSpeed * this.Transform.Up);
                return;
            }
            #endregion

            #region Translation
            //Forward
            if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[0]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = (this.Transform.Look * this.MovementVector);
                this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Look);
                return;
            }

            //Back
            else if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[1]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = -(this.Transform.Look * this.MovementVector);
                this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Look);
                return;
            }

            //Left
            if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[2]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = -(this.Transform.Right * this.MovementVector);
                this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                return;
            }

            //Right
            else if (this.ManagerParameters.GamepadManager.IsButtonPressed(this.PlayerIndex, this.MoveButtons[3]) && !this.InMotion)
            {
                //Calculate target position, relative to the camera
                this.TargetPosition = (this.Transform.Right * this.MovementVector);
                this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * this.Transform.Right);
                return;
            }
            #endregion
        }

        public override void TakeTurn(GameTime gameTime)
        {
            //If it is not currently the players turn, return
            if (!StateManager.PlayerTurn) return;

            //If a controller is connected for the current player
            if (this.ManagerParameters.GamepadManager.IsPlayerConnected(this.PlayerIndex))

                //Handle Gamepad Input
                HandleGamepadInput(gameTime);

            //Otherwise
            else

                //Handle keyboard input
                HandleKeyboardInput(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            TakeTurn(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}