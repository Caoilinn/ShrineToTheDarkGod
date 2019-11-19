using JigLibX.Collision;
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
        private float health;
        private float attack;
        private float defence;
        private Keys[] moveKeys;
        private Vector3 translationOffset;
        private KeyboardManager keyboardManager;
        private float jumpHeight;
        #endregion

        #region Properties
        public KeyboardManager KeyboardManager
        {
            get
            {
                return keyboardManager;
            }
            set
            {
                this.keyboardManager = value;
            }
        }

        public float JumpHeight
        {
            get
            {
                return jumpHeight;
            }
            set
            {
                jumpHeight = (value > 0) ? value : 1;
            }
        }

        public Vector3 TranslationOffset
        {
            get
            {
                return translationOffset;
            }
            set
            {
                translationOffset = value;
            }
        }

        public Keys[] MoveKeys
        {
            get
            {
                return moveKeys;
            }
            set
            {
                moveKeys = value;
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
            float width,
            float height,
            float depth,
            float accelerationRate,
            float dececelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            Keys[] moveKeys,
            Vector3 translationOffset,
            KeyboardManager keyboardManager,
            float jumpHeight,
            float health,
            float attack,
            float defence
        ) : base(id, actorType, transform, effectParameters, model, width, height, depth, 0, 0, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence) {
            this.MoveKeys = moveKeys;
            this.TranslationOffset = translationOffset;
            this.KeyboardManager = keyboardManager;
            this.JumpHeight = jumpHeight;
        }
        #endregion

        #region Methods
        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale)
                * this.Collision.GetPrimitiveLocal(0).Transform.Orientation
                * this.Body.Orientation
                * this.Transform.Orientation
                * Matrix.CreateTranslation(this.Body.Position + translationOffset);
        }

        public void TakeDamage(float damage)
        {
            this.Health -= damage;
        }

        protected virtual void HandleMouseInput(GameTime gameTime)
        {
        }

        protected virtual void HandleKeyboardInput(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyboardInput(gameTime);
            HandleMouseInput(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}