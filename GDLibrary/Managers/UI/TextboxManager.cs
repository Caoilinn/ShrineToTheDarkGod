using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class TextboxManager : PausableDrawableGameComponent
    {
        #region Fields
        //stores the actors shown for a particular menu scene (e.g. for the "main menu" scene we would have actors: startBtn, ExitBtn, AudioBtn)
        private Dictionary<string, List<DrawnActor2D>> uiDictionary;
        private List<DrawnActor2D> activeList = null;

        private SpriteBatch spriteBatch;
        private ManagerParameters managerParameters;

        private string textboxtext;
        #endregion

        #region Properties
        public ManagerParameters ManagerParameters
        {
            get
            {
                return this.managerParameters;
            }

        }
        public string TextboxText
        {
            get
            {
                return this.textboxtext;
            }
            set
            {
                this.textboxtext = value;
            }
        }
        public List<DrawnActor2D> ActiveList
        {
            get
            {
                return this.activeList;
            }
        }
        #endregion

        #region Constructors
        public TextboxManager(
            Game game, 
            ManagerParameters managerParameters,
            SpriteBatch spriteBatch, 
            EventDispatcher eventDispatcher,
            StatusType statusType, 
            string textboxtext
        ) : base(game, statusType, eventDispatcher) {
            this.uiDictionary = new Dictionary<string, List<DrawnActor2D>>();
            this.managerParameters = managerParameters;
            this.spriteBatch = spriteBatch;
            this.textboxtext = textboxtext;
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.TextboxChanged += EventDispatcher_TextboxChanged;
            eventDispatcher.TextboxChanged += EventDispatcher_UICombat;
            base.RegisterForEventHandling(eventDispatcher);
        }

        protected virtual void EventDispatcher_TextboxChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnPause)
            {
                this.StatusType = StatusType.Off;
            }
        }
        #endregion

        #region Methods
        private void ClearTextbox()
        {
            this.TextboxText = "";
        }

        protected void EventDispatcher_UICombat(EventData eventData)
        {
            float damage = 0;
            float playerHealth = 0;
            float enemyHealth = 0;
            string item = "";
            string info = "";

            List<EventActionType> combatEventActionTypes = new List<EventActionType>();
            combatEventActionTypes.Add(EventActionType.OnStart);
            combatEventActionTypes.Add(EventActionType.OnPlayerAttack);
            combatEventActionTypes.Add(EventActionType.OnPlayerDefend);
            combatEventActionTypes.Add(EventActionType.OnPlayerDodge);
            combatEventActionTypes.Add(EventActionType.OnEnemyAttack);

            if (combatEventActionTypes.Contains(eventData.EventType))
            {
                damage = (float) eventData.AdditionalParameters[0];
                playerHealth = (float) eventData.AdditionalParameters[1];
                enemyHealth = (float) eventData.AdditionalParameters[2];
            } 
            else if (eventData.EventType.Equals(EventActionType.PlayerHealthPickup))
            {
                playerHealth = (float) eventData.AdditionalParameters[0];
            }
            else if (eventData.EventType.Equals(EventActionType.OnItemAdded) || eventData.EventType.Equals(EventActionType.OnItemRemoved))
            {
                item = eventData.AdditionalParameters[0] as string;
            }
            else if (eventData.EventType.Equals(EventActionType.OnDisplayInfo))
            {
                info = eventData.AdditionalParameters[0] as string;
            }

            switch (eventData.EventType)
            {
                case EventActionType.OnStart:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText =
                        "Game Log" + "\n\n" +
                        "Battle commenced!" + "\n\n" +
                        "Player health " + playerHealth + "\n" +
                        "Enemy health " + enemyHealth;
                    break;

                case EventActionType.OnPlayerAttack:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        "Player attacked, dealing damage of " + damage + "\n\n" +
                        "Player health " + playerHealth + "\n" +
                        "Enemy health " + enemyHealth;
                    break;

                case EventActionType.OnPlayerDefend:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        "Player blocked, taking damage of " + damage + "\n\n" +
                        "Player health " + playerHealth + "\n" +
                        "Enemy health " + enemyHealth;
                    break;

                case EventActionType.OnPlayerDodge:
                    if (damage <= 0)
                    {
                        ClearTextbox();
                        this.StatusType = StatusType.Update | StatusType.Drawn;
                        this.TextboxText = 
                            "Game Log" + "\n\n" + 
                            "Player Dodged" + "\n\n" +
                            "Player health " + playerHealth + "\n" +
                            "Enemy health " + enemyHealth;
                    }
                    else
                    {
                        ClearTextbox();
                        this.StatusType = StatusType.Update | StatusType.Drawn;
                        this.TextboxText = 
                            "Game Log" + "\n\n" +
                            "Player dodge failed, the player took damage of " + damage + "\n\n" +
                            "Player health " + playerHealth + "\n" +
                            "Enemy health " + enemyHealth;
                    }
                    break;

                case EventActionType.OnEnemyAttack:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        "Enemy attatcked with damage of " + damage + "\n\n" +
                        "Player health " + playerHealth + "\n" +
                        "Enemy health " + enemyHealth;
                    break;

                case EventActionType.OnInitiateBattle:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText =
                        "Game Log" + "\n\n" + 
                        "Battle starts!";
                    break;

                case EventActionType.OnBattleEnd:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        "Battle over";
                    break;

                case EventActionType.OnEnemyDeath:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        "Enemy defeated!" + "\n\n" +
                        "You'll never stop the dark god!";
                    break;

                case EventActionType.PlayerHealthPickup:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText =
                        "Game Log" + "\n\n" +
                        "Health regained!" + "\n\n" +
                        "Player health " + playerHealth;
                    break;

                case EventActionType.OnItemAdded:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText = 
                        "Game Log" + "\n\n" + 
                        item + " added to inventory";
                    break;

                case EventActionType.OnItemRemoved:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText =
                        "Game Log" + "\n\n" +
                        item + " removed from inventory";
                    break;

                case EventActionType.OnDisplayInfo:
                    ClearTextbox();
                    this.StatusType = StatusType.Update | StatusType.Drawn;
                    this.TextboxText =
                        "Game Log" + "\n\n" +
                        info;
                    break;
            }
        }

        public void Add(string sceneID, DrawnActor2D actor)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
            {
                this.uiDictionary[sceneID].Add(actor);
            }
            else
            {
                List<DrawnActor2D> newList = new List<DrawnActor2D>();
                newList.Add(actor);
                this.uiDictionary.Add(sceneID, newList);
            }

            //If the user forgets to set the active list then set to the sceneID of the last added item
            SetActiveList(sceneID);
        }

        public DrawnActor2D Find(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
            {
                return this.uiDictionary[sceneID].Find(predicate);
            }
            return null;
        }

        public bool Remove(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D foundUIObject = Find(sceneID, predicate);

            if (foundUIObject != null)
                return this.uiDictionary[sceneID].Remove(foundUIObject);

            return false;
        }

        //Return all the actor2D objects associated with the "health ui" or "inventory ui"
        public List<DrawnActor2D> FindAllBySceneID(string sceneID)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
            {
                return this.uiDictionary[sceneID];
            }
            return null;
        }

        public bool SetActiveList(string sceneID)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
            {
                this.activeList = this.uiDictionary[sceneID];
                return true;
            }

            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.activeList != null)
            {
                //Update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.activeList)
                {
                    //If update flag is set
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != StatusType.Update)
                    {
                        (currentUIObject as UITextObject).Text = this.TextboxText;
                        currentUIObject.Update(gameTime);
                    }
                }
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.activeList != null)
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                foreach (DrawnActor2D currentUIObject in this.activeList)
                {
                    if ((currentUIObject.GetStatusType() & StatusType.Drawn) != 0) //if drawn flag is set
                    {
                        currentUIObject.Draw(gameTime, spriteBatch);
                        HandleMouseOver(currentUIObject, gameTime);
                    }
                }
                spriteBatch.End();
            }
        }

        protected virtual void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
            //developer implements in subclass of MenuManager - see MyMenuManager.cs
        }

        protected virtual void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            //developer implements in subclass of MenuManager - see MyMenuManager.cs
        }
        #endregion
    }
}
