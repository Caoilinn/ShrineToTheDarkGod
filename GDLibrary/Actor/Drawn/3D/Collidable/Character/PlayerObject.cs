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

        private ManagerParameters managerParameters;

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
            float radius,
            float height,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            Keys[] moveKeys,
            Vector3 translationOffset,
            KeyboardManager keyboardManager,
            ManagerParameters managerParameters,
            float jumpHeight
        ) : base(id, actorType, transform, effectParameters, model, radius, height, 0, 0, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence) {
            this.MoveKeys = moveKeys;
            this.TranslationOffset = translationOffset;
            this.KeyboardManager = keyboardManager;
            this.managerParameters = managerParameters;
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

        /*
        public bool HasWeapon()
        {
            if (this.managerParameters.InventoryManager.HasSword("Sword"))
                return true;
            else
                return false;
        } */

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