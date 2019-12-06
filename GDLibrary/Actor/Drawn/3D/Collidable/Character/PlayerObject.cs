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
        private Keys[] moveKeys;
        #endregion

        #region Properties
        public Keys[] MoveKeys
        {
            get {
                return this.moveKeys;
            }
            set {
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
            Keys[] moveKeys
        ) : base(id, actorType, transform, effectParameters, model, accelerationRate, decelerationRate, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence, managerParameters)
        {
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

        public override void TakeTurn(GameTime gameTime)
        {
            //If it is not currently the players turn, return
            if (!StateManager.PlayerTurn) return;

            //If the player is in battle, return
            if (StateManager.InCombat) return;

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