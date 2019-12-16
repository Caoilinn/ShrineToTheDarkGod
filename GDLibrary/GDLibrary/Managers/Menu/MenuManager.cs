using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class MenuManager : PausableDrawableGameComponent
    {
        #region Fields
        private Dictionary<string, List<DrawnActor2D>> menuDictionary;
        private List<DrawnActor2D> activeList = null;
        private CameraManager cameraManager;
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private SpriteBatch spriteBatch;
        private bool isVisible;

        //Tracks last object mouse-ed over by the cursor
        private UIObject oldUIObjectMouseOver;
        #endregion

        #region Properties
        public Dictionary<string, List<DrawnActor2D>> MenuDictionary
        {
            get
            {
                return this.menuDictionary;
            }
            set
            {
                this.menuDictionary = value;
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

        public KeyboardManager KeyboardManager
        {
            get
            {
                return this.keyboardManager;
            }
            set
            {
                this.keyboardManager = value;
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
        public MenuManager(
            Game game,
            StatusType statusType,
            EventDispatcher eventDispatcher,
            CameraManager cameraManager,
            MouseManager mouseManager,
            KeyboardManager keyboardManager,
            SpriteBatch spriteBatch
        ) : base(game, statusType, eventDispatcher) {
            this.cameraManager = cameraManager;
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
            this.spriteBatch = spriteBatch;

            this.menuDictionary = new Dictionary<string, List<DrawnActor2D>>();
        }
        #endregion

        #region Event Handling
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
        public void Add(string menuSceneID, DrawnActor2D actor)
        {
            if (this.menuDictionary.ContainsKey(menuSceneID))
            {
                this.menuDictionary[menuSceneID].Add(actor);
            }
            else
            {
                this.menuDictionary.Add(menuSceneID, new List<DrawnActor2D>() { actor });
            }

            //If the user forgets to set the active list then set to the sceneID of the last added item
            if (this.activeList == null)
            {
                SetActiveList(menuSceneID);
            }
        }

        public DrawnActor2D Find(string menuSceneID, Predicate<DrawnActor2D> predicate)
        {
            if (this.menuDictionary.ContainsKey(menuSceneID))
                return this.menuDictionary[menuSceneID].Find(predicate);

            return null;
        }

        public bool Remove(string menuSceneID, Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D foundUIObject = Find(menuSceneID, predicate);

            if (foundUIObject != null)
                return this.menuDictionary[menuSceneID].Remove(foundUIObject);

            return false;
        }

        //Return all the actor2D objects associated with the "main menu" or "audio menu"
        public List<DrawnActor2D> FindAllBySceneID(string menuSceneID)
        {
            if (this.menuDictionary.ContainsKey(menuSceneID))
            {
                return this.menuDictionary[menuSceneID];
            }
            return null;
        }

        public bool SetActiveList(string menuSceneID)
        {
            if (this.menuDictionary.ContainsKey(menuSceneID))
            {
                this.activeList = this.menuDictionary[menuSceneID];
                return true;
            }

            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.activeList != null)
            {
                //update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.activeList)
                {
                    //If update flag is set
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != 0)
                        currentUIObject.Update(gameTime);
                }
            }

            //Check for mouse over and mouse click on a menu item
            CheckMouseOverAndClick(gameTime);
        }

        private void CheckMouseOverAndClick(GameTime gameTime)
        {
            foreach (UIObject currentUIObject in this.activeList)
            {
                //only handle mouseover and mouse click for buttons
                if (currentUIObject.ActorType == ActorType.UIButton)
                {
                    //add an if to check that this is a interactive UIButton object
                    if (currentUIObject.Transform.Bounds.Intersects(this.mouseManager.Bounds))
                    {
                        //if mouse is over a new ui object then set old to "IsMouseOver=false"
                        if (this.oldUIObjectMouseOver != null && this.oldUIObjectMouseOver != currentUIObject)
                        {
                            oldUIObjectMouseOver.MouseOverState.Update(false);
                        }

                        //update the current state of the currently mouse-over'ed ui object
                        currentUIObject.MouseOverState.Update(true);

                        //apply any mouse over or mouse click actions
                        HandleMouseOver(currentUIObject, gameTime);
                        if (this.mouseManager.IsLeftButtonClickedOnce())
                            HandleMouseClick(currentUIObject, gameTime);

                        if (this.keyboardManager.IsAnyKeyPressed())
                            HandleKeyboardInput();

                        //store the current as old for the next update
                        this.oldUIObjectMouseOver = currentUIObject;
                    }
                    else
                    {
                        //set the mouse as not being over the current ui object
                        currentUIObject.MouseOverState.Update(false);
                    }
                }
            }
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

        protected virtual void HandleKeyboardInput()
        {

        }
        #endregion
    }
}