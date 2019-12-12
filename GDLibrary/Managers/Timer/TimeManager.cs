using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GDLibrary.Managers.Timer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TimeManager : GameComponent
    {
        #region Fields
        private static float startTime;
        private static float moveInterval;
        #endregion

        #region Properties
        #endregion

        public TimeManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public virtual void TurnTimer(GameTime gameTime)
        {
            //Allow free movement until in combat
            if (!StateManager.InCombat)
            {
                EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                return;
            }

            //If first time called
            if (!this.MoveStarted)
            {
                //Start move
                this.MoveStarted = true;
                this.StartMoveTime = gameTime.TotalGameTime.TotalMilliseconds;
                return;
            }

            //If move has ended
            if (gameTime.TotalGameTime.TotalMilliseconds > (this.StartMoveTime + (this.moveInterval / 1.0f)))
            {
                //Track player
                this.EffectParameters.DiffuseColor = this.OriginalColor;
                TrackPlayer(gameTime);

                this.MoveStarted = false;
            }

            //If move at midpoint
            else if (gameTime.TotalGameTime.TotalMilliseconds > (this.StartMoveTime + (this.moveInterval / 2.0f)))
            {
                //Mark midpoint
                this.EffectParameters.DiffuseColor = this.OriginalColor;
                this.MoveMidpoint = true;
            }
        }
    }
}
