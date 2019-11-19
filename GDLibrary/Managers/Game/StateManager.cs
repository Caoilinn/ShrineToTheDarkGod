/*
Function: 		The "brain" for dynamic events in the game. This class listens for events and responds with changes to the game.
                For example, if the player wins/loses the game then this class will determine what happens as a consequence.
                It may, in this case, show certain UITextObjects, play sounds, reset controllers, generate new collidable objects.
Author: 		NMCG
Version:		1.0
Date Updated:	16/11/17
Bugs:			
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class StateManager : PausableGameComponent
    {
        #region Fields
        static public int currentLevel;
        static public bool playerTurn;
        static public bool enemyTurn;
        #endregion

        #region Properties
        public int CurrentLevel
        {
            get
            {
                return StateManager.currentLevel;
            }
            set
            {
                StateManager.currentLevel = value;
            }
        }

        public bool PlayerTurn
        {
            get
            {
                return StateManager.playerTurn;
            }
            set
            {
                StateManager.playerTurn = value;
            }
        }

        public bool EnemyTurn
        {
            get
            {
                return StateManager.enemyTurn;
            }
            set
            {
                StateManager.enemyTurn = value;
            }
        }
        #endregion

        #region Constructor
        public StateManager(
            Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : base(game, eventDispatcher, statusType) {
            this.PlayerTurn = true;
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.GameChanged += EventDispatcher_GameChanged;
        }

        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Did the event come from the main menu and is it a start game event
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Turn on update and draw i.e. hide the menu
                this.StatusType = StatusType.Update | StatusType.Drawn;
            }

            //Did the event come from the main menu and is it a pause game event
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Turn off update and draw i.e. show the menu since the game is paused
                this.StatusType = StatusType.Off;
            }
        }

        private void EventDispatcher_GameChanged(EventData eventData)
        {
            //Did the event come from the game being won?
            if (eventData.EventType == EventActionType.OnWin)
            {
                this.CurrentLevel++;
            }

            //Did the event come from the player making a move?
            else if (eventData.EventType == EventActionType.PlayerTurn)
            {
                this.PlayerTurn = true;
                this.EnemyTurn = false;
            }

            //Did the event come from an enemy making a move?
            else if (eventData.EventType == EventActionType.EnemyTurn)
            {
                //this.EnemyTurn = true;
                //this.PlayerTurn = false;
            }
        }
        #endregion
    }
}