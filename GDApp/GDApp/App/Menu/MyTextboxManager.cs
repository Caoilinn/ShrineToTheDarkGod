using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDApp
{
    public class MyTextboxManager : TextboxManager
    {
        public MyTextboxManager(Game game, ManagerParameters managerParameters, SpriteBatch spriteBatch, EventDispatcher eventDispatcher, StatusType statusType, string textboxtext)
            : base(game, managerParameters, spriteBatch, eventDispatcher, statusType, textboxtext)
        {
            this.TextboxText = "combat manager";
        }


        // private void changeText(EventData eventdata)
        // {
        //     switch(eventdata.ID)
        //     {
        //         
        //     }
        // }

        #region Event Handling
        protected override void EventDispatcher_TextboxChanged(EventData eventData)
        {
            //Call base method to show/hide the menu
            base.EventDispatcher_TextboxChanged(eventData);

            //Then generate sound events particular to your game e.g. play background music in a menu
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Add event to stop background menu music here...
                //Object[] additionalParameters = { "in-game background music", 1 };
                //EventDispatcher.Publish(new EventData(EventActionType.OnStop, EventCategoryType.Sound2D, additionalParameters));
            }
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Add event to play background menu music here...
                //Object[] additionalParameters = { "menu elevator music" };
                //EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, additionalParameters));
            }
        }
        #endregion


        protected override void HandleGamePad(GameTime gameTime)
        {
            base.HandleGamePad(gameTime);
        }

        protected override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
            base.HandleKeyboard(gameTime);
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            base.HandleMouse(gameTime);
        }

        protected override void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            base.HandleMouseClick(uiObject, gameTime);
        }

        protected override void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
            base.HandleMouseOver(uiObject, gameTime);
        }

        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            base.RegisterForEventHandling(eventDispatcher);
        }

        private void clearTextbox()
        {
            this.TextboxText = "";
        }

        public void Update()
        {
            this.TextboxText = base.TextboxText;
        }
    }
}
