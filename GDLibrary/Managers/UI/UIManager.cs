using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class UIManager : PausableDrawableGameComponent
    {
        #region Fields
        private Dictionary<string, List<DrawnActor2D>> uiDictionary;
        private List<DrawnActor2D> activeList = null;
        private CameraManager cameraManager;
        private MouseManager mouseManager;
        private SpriteBatch spriteBatch;
        private bool isVisible;
        #endregion

        #region Properties
        public Dictionary<string, List<DrawnActor2D>> ÙIDictionary
        {
            get
            {
                return this.uiDictionary;
            }
            set
            {
                this.uiDictionary = value;
            }
        }

        public List<DrawnActor2D> ActiveList
        {
            get
            {
                return this.activeList;
            }
            set
            {
                this.activeList = value;
            }
        }

        public CameraManager CameraManager
        {
            get
            {
                return this.cameraManager;
            }
            set
            {
                this.cameraManager = value;
            }
        }

        public MouseManager MouseManager
        {
            get
            {
                return this.mouseManager;
            }
            set
            {
                this.mouseManager = value;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return this.spriteBatch;
            }
            set
            {
                this.spriteBatch = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
            }
        }
        #endregion

        #region Constructors
        public UIManager(
            Game game,
            StatusType statusType,
            EventDispatcher eventDispatcher,
            CameraManager cameraManager,
            MouseManager mouseManager,
            SpriteBatch spriteBatch
        ) : base(game, statusType, eventDispatcher) {
            this.cameraManager = cameraManager;
            this.mouseManager = mouseManager;
            this.spriteBatch = spriteBatch;

            this.uiDictionary = new Dictionary<string, List<DrawnActor2D>>();
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.UIChanged += EventDispatcher_MenuChanged;
            base.RegisterForEventHandling(eventDispatcher);
        }

        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //We need to override this method because the menu is OFF when other components are ON and vice versa
            if (eventData.EventType == EventActionType.OnStart)
            {
                this.StatusType = StatusType.Off;
                this.isVisible = false;
            }
            else if (eventData.EventType == EventActionType.OnPause)
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
            }
            else
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

        //Return all the actor2D objects associated with the "health ui" or "inventory ui"
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
                    //If drawn flag is set
                    if ((currentUIObject.GetStatusType() & StatusType.Drawn) != 0)
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