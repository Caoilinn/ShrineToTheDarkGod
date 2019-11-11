using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        public float Health
        {
            get {
                return health;
            }
        }
        public float Attack
        {
            get {
                return attack;
            }
        }
        public float Defence
        {
            get {
                return defence;
            }
        }
        #endregion

        public PlayerObject(
            string id, 
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters, 
            Model model, 
            Keys[] moveKeys, 
            float radius, 
            float height,
            float jumpHeight, 
            Vector3 translationOffset, 
            KeyboardManager keyboardManager
        ) : base(id, actorType, statusType, transform, effectParameters, model, radius, height) {
            this.moveKeys = moveKeys;
            this.translationOffset = translationOffset;
            this.keyboardManager = keyboardManager;
            this.jumpHeight = jumpHeight;
            this.health = 100;
            this.attack = 100;
            this.defence = 100;
        }

        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale) *
                this.Collision.GetPrimitiveLocal(0).Transform.Orientation *
                this.Body.Orientation *
                this.Transform.Orientation *
                Matrix.CreateTranslation(this.Body.Position + translationOffset);
        }


        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine("PlayerObject Update");
            HandleKeyboardInput(gameTime);
            HandleMouseInput(gameTime);
            base.Update(gameTime);
        }

        protected virtual void HandleMouseInput(GameTime gameTime)
        {
          //perhaps rotate using mouse pointer distance from centre?
        }

        protected virtual void HandleKeyboardInput(GameTime gameTime)
        {
            /*
            if (this.KeyboardManager.IsFirstKeyPress(this.moveKeys[8]))
            {
                Console.WriteLine("Attack");
            } else if(this.KeyboardManager.IsFirstKeyPress(this.moveKeys[9]))
            {
                Console.WriteLine("Defend");
            }else if(this.KeyboardManager.IsFirstKeyPress(this.moveKeys[10]))
            {
                Console.WriteLine("Dodge");
            }
            */
        }

        public void takeDamage(float damage)
        {
            this.health -= damage;
        }

        //add clone, equals, gethashcode, remove...
    }
}
