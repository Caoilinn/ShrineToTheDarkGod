using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    public class EnemyObject : CharacterObject
    {
        #region Fields
        //Local vars
        private double startMoveTime;
        private double moveInterval = 2;
        bool moveStarted;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public EnemyObject(
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
            ManagerParameters managerParameters
        ) : base(id, actorType, transform, effectParameters, model, accelerationRate, decelerationRate, movementVector, rotationVector, moveSpeed, rotateSpeed, health, attack, defence, managerParameters) {    
        }
        #endregion

        #region Methods
        public void TrackPlayer(GameTime gameTime)
        {
            EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
        }

        public override void TakeTurn(GameTime gameTime)
        {
            ////Set up move time
            //if (!moveStarted) this.startMoveTime = gameTime.TotalGameTime.TotalSeconds;

            //If it is not currently the enemys' turn, return
            if (!StateManager.EnemyTurn) return;

            //If the enemy is in combat
            if (StateManager.InCombat) return;

            ////Set some interval turn timer
            //TurnTimer(gameTime);

            TrackPlayer(gameTime);
        }

        public virtual void TurnTimer(GameTime gameTime)
        {
            if ((this.startMoveTime + this.moveInterval) < gameTime.TotalGameTime.TotalSeconds)
            {
                this.moveStarted = false;
                TrackPlayer(gameTime);
            }
            else
            {
                this.moveStarted = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            TakeTurn(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}