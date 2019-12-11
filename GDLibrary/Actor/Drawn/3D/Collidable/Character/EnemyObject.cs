using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    public class EnemyObject : CharacterObject
    {
        #region Fields
        static private bool moveStarted;
        static private bool moveMidpoint;
        static private double startMoveTime;
        private Color originalColor;

        //Should construct this later - use AppData to detemine move interval time
        private double moveInterval = 0.5f;
        #endregion

        #region Properties
        public bool MoveStarted
        {
            get
            {
                return EnemyObject.moveStarted;
            }
            set
            {
                EnemyObject.moveStarted = value;
            }
        }

        public bool MoveMidpoint
        {
            get
            {
                return EnemyObject.moveMidpoint; 
            }
            set
            {
                EnemyObject.moveMidpoint = value;
            }
        }

        public double StartMoveTime
        {
            get
            {
                return EnemyObject.startMoveTime;
            }
            set
            {
                EnemyObject.startMoveTime = value;
            }
        }

        public Color OriginalColor
        {
            get
            {
                return this.originalColor;
            }
            set
            {
                this.originalColor = value;
            }
        }

        public double MoveInterval
        {
            get
            {
                return this.moveInterval;
            }
            set
            {
                this.moveInterval = value;
            }
        }
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
            this.MoveStarted = false;
        }
        #endregion

        #region Methods
        public void TrackPlayer(GameTime gameTime)
        {
            EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
        }

        public override void TakeTurn(GameTime gameTime)
        {                
            //If it is not currently the enemys' turn, return
            if (!StateManager.EnemyTurn) return;

            //If in combat, return
            if (StateManager.InCombat) return;

            //Set some interval turn timer
            TurnTimer(gameTime);
        }

        public virtual void TurnTimer(GameTime gameTime)
        {
            //If first time called
            if (!this.MoveStarted)
            {
                //Start move
                this.MoveStarted = true;
                this.StartMoveTime = gameTime.TotalGameTime.TotalMilliseconds;
                return;
            }

            //If move at midpoint
            if (gameTime.TotalGameTime.TotalMilliseconds > (this.StartMoveTime + (this.moveInterval / 2.0f)))
            {
                //Mark midpoint
                this.EffectParameters.DiffuseColor = this.OriginalColor;
                this.MoveMidpoint = true;
                return;
            }

            //If move has ended
            if (gameTime.TotalGameTime.TotalMilliseconds > (this.StartMoveTime + this.moveInterval))
            {
                //Stop move
                EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                this.MoveStarted = false;
                return;
            }
        }

        public override void TakeDamage(float damage)
        {
            //Make Enemy Flash Red
            this.OriginalColor = this.EffectParameters.DiffuseColor;
            this.EffectParameters.DiffuseColor = new Color(1.0f, 0.2f, 0.2f);

            //Update Player XP
            EventDispatcher.Publish(new EventData(EventActionType.EnemyHealthUpdate, EventCategoryType.UIMenu, new object[] { damage }));

            //Update Enemy Health
            base.TakeDamage(damage);
        }

        public override void Update(GameTime gameTime)
        {
            TakeTurn(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}