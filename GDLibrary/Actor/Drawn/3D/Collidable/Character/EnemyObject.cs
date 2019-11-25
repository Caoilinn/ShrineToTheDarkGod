using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class EnemyObject : CharacterObject
    {
        #region Fields
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
            //If it is not currently the enemys' turn, return
            if (!StateManager.EnemyTurn) return;

            //If the enemy is in combat
            if (StateManager.InCombat) return;

            TrackPlayer(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            TakeTurn(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}