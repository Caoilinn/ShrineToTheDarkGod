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

        public TextboxManager(Game game, ManagerParameters managerParameters,
            SpriteBatch spriteBatch, EventDispatcher eventDispatcher,
            StatusType statusType, string textboxtext)
            : base(game, statusType, eventDispatcher)
        {
            this.uiDictionary = new Dictionary<string, List<DrawnActor2D>>();

            //used to listen for input
            this.managerParameters = managerParameters;

            //used to render menu and UI elements
            this.spriteBatch = spriteBatch;

            this.textboxtext = textboxtext;

        }

        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.TextboxChanged += EventDispatcher_TextboxChanged;
            base.RegisterForEventHandling(eventDispatcher);
        }

        #region Event Handling|

        protected virtual void EventDispatcher_TextboxChanged(EventData eventData)
        {

            if (eventData.EventType == EventActionType.OnStart)
            {
                this.StatusType = StatusType.Update | StatusType.Drawn;
                this.TextboxText = eventData.AdditionalParameters[0] as string + " is fighting " + eventData.AdditionalParameters[1] as string;

            }
            else if (eventData.EventType == EventActionType.OnPause)
                this.StatusType = StatusType.Off;

            if (eventData.EventType == EventActionType.OnInitiateBattle)
            {

                this.textboxtext = "Battle start!";
                Console.WriteLine("it workkssss dcxaadasfava");
                this.StatusType = StatusType.Update;
            }
        }
        #endregion



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

            //if the user forgets to set the active list then set to the sceneID of the last added item

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

        //e.g. return all the actor2D objects associated with the "health ui" or "inventory ui"
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
                //update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.activeList)
                {
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != 0) //if update flag is set
                        currentUIObject.Update(gameTime);
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

    }
}
