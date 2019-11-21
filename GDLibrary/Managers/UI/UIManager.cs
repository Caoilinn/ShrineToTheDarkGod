using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class UIManager : PausableDrawableGameComponent
    {
        #region Fields
        //Stores the actors shown for a particular menu scene (e.g. for the "main menu" scene we would have actors: startBtn, ExitBtn, AudioBtn)
        private Dictionary<string, List<DrawnActor2D>> uiDictionary;
        private List<DrawnActor2D> activeList = null;

        private SpriteBatch spriteBatch;
        private ManagerParameters managerParameters;
        private CameraManager cameraManager;
        private bool isVisible;
        #endregion

        #region Properties
        public bool IsVisible
        {
            get {
                return this.isVisible;
            }
        }

        public ManagerParameters ManagerParameters
        {
            get {
                return this.managerParameters;
            }
        }

        public List<DrawnActor2D> ActiveList
        {
            get {
                return this.activeList;
            }
        }
        #endregion

        #region Constructors
        public UIManager(
            Game game,
            ManagerParameters managerParameters,
            CameraManager cameraManager,
            SpriteBatch spriteBatch,
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : base(game, eventDispatcher, statusType)
        {
            this.uiDictionary = new Dictionary<string, List<DrawnActor2D>>();

            //Used to listen for input
            this.managerParameters = managerParameters;
            this.cameraManager = cameraManager;

            //Used to render menu and UI elements
            this.spriteBatch = spriteBatch;
        }
        #endregion

        #region Event Handling

        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.UIChanged += EventDispatcher_MenuChanged;
            eventDispatcher.UIChanged += EventDispatcher_UICombat;
            eventDispatcher.UIChanged += EventDispatcher_UIHealth;
            base.RegisterForEventHandling(eventDispatcher);
        }

        protected void EventDispatcher_UICombat(EventData eventData)
        {
            float damage = 0;

            if (eventData.EventType != EventActionType.OnEnemyDeath)
            {
                damage = (float)eventData.AdditionalParameters[0];
            }
            switch (eventData.EventType)
            {
                case EventActionType.OnPlayerAttack:
                    Console.WriteLine("Player Attatcked with damage of " + damage);
                    break;
                case EventActionType.OnPlayerDefend:
                    Console.WriteLine("Player Defended taking damage of " + damage);
                    break;
                case EventActionType.OnPlayerDodge:
                    if (damage <= 0)
                        Console.WriteLine("Player Dodged");
                    else
                        Console.WriteLine("Player Dodge Failed, the player took damage of " + damage);
                    break;
                case EventActionType.OnEnemyAttack:
                    Console.WriteLine("Enemy Attatcked with damage of " + damage);
                    break;
                case EventActionType.OnEnemyDeath:
                    Console.WriteLine("YOU HAVE BEATEN THE ENEMY");
                    break;

            }



        }

        protected void EventDispatcher_UIHealth(EventData eventData)
        {
            if(eventData.EventType == EventActionType.PlayerHealthUpdate)
            {
                float playerHealth = (float)eventData.AdditionalParameters[0];

                Console.WriteLine("Player Health: " + playerHealth);

            } else
            {
                float enemyHealth = (float)eventData.AdditionalParameters[0];

                Console.WriteLine("Enemy Health: " + enemyHealth);
            }
        }

        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //We need to override this method because the menu is OFF when other components are ON and vice versa
            if (eventData.EventType == EventActionType.OnStart)
            {
                this.StatusType = StatusType.Off;
                this.isVisible = false;
            } else if (eventData.EventType == EventActionType.OnPause)
            {
                this.StatusType = StatusType.Drawn | StatusType.Update;
                this.isVisible = true;
            }
        }



        #endregion

        #region Methods
        public void Add(string sceneID, DrawnActor2D actor)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
            {
                this.uiDictionary[sceneID].Add(actor);
            } else
            {
                List<DrawnActor2D> newList = new List<DrawnActor2D>();
                newList.Add(actor);
                this.uiDictionary.Add(sceneID, newList);
            }

            //If the user forgets to set the active list then set to the sceneID of the last added item
            if (this.activeList == null)
                SetActiveList(sceneID);
        }

        public DrawnActor2D Find(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
                return this.uiDictionary[sceneID].Find(predicate);

            return null;
        }

        public bool Remove(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D foundUIObject = Find(sceneID, predicate);

            if (foundUIObject != null)
                return this.uiDictionary[sceneID].Remove(foundUIObject);

            return false;
        }

        //e.g. return all the actor2D objects associated with the "health ui" or "inventory ui"
        public List<DrawnActor2D> FindAllBySceneID(string sceneID)
        {
            if (this.uiDictionary.ContainsKey(sceneID))
                return this.uiDictionary[sceneID];

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

                //Update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.activeList)

                    //If update flag is set
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != 0)
                        currentUIObject.Update(gameTime);
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.activeList != null)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
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
            //Developer implements in subclass of MenuManager - see MyMenuManager.cs
        }

        protected virtual void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            //Developer implements in subclass of MenuManager - see MyMenuManager.cs
        }
        #endregion
    }
}