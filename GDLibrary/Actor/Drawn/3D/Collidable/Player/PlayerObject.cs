using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

        #region Constructors
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

        //HashSet<ModelObject> collisionSet = new HashSet<ModelObject>();

        //private void CheckForCollisionWithPickup(float distanceToCollision, CollidableObject pickup)
        //{
        //    //If the pickup is in an adjacent cell
        //    if (distanceToCollision >= 0.5f && distanceToCollision <= 1.0f)
        //    {
        //        //If the pickup has not yet been realised
        //        if (!this.collisionSet.Contains(pickup))
        //        {
        //            //Add pickup to the collision set
        //            this.collisionSet.Add(pickup);

        //            //Publish event to play sound effect
        //            EventDispatcher.Publish(
        //                new EventData(
        //                    EventActionType.OnPlay,
        //                    EventCategoryType.Sound2D,
        //                    new object[] { "boing" }
        //                )
        //            );
        //        }
        //    }

        //    //If the pickup is in the current cell
        //    if (distanceToCollision <= 0.5f)
        //    {
        //        //Publish event to remove pickup
        //        EventDispatcher.Publish(
        //            new EventData(
        //                pickup,
        //                EventActionType.OnRemoveActor,
        //                EventCategoryType.SystemRemove
        //            )
        //        );

        //        //Publish event to add to inventory
        //        EventDispatcher.Publish(
        //            new EventData(
        //                EventActionType.OnAddToInventory,
        //                EventCategoryType.Pickup,
        //                new object[] { pickup }
        //            )
        //        );

        //        //Publish event to update hud
        //        EventDispatcher.Publish(
        //            new EventData(
        //                EventActionType.OnUpdateHud,
        //                EventCategoryType.Pickup,
        //                new object[] { pickup }
        //            )
        //        );
        //    }
        //}

        //private void CheckForCollisionWithEnemy(float distanceToCollision)
        //{
        //    //If the enemy is an adjacent cell
        //    if (distanceToCollision >= 0.5f && distanceToCollision <= 1.0f)
        //    {
        //        //Publish event to prevent movement
        //        //Publish event to initiate battle
        //        //Publish event to play music
        //    }
        //}

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