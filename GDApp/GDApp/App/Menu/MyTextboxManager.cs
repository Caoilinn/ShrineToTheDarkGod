using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDApp
{
    public class MyTextboxManager : TextboxManager
    {
        public MyTextboxManager(
            Game game, 
            ManagerParameters managerParameters, 
            SpriteBatch spriteBatch, 
            EventDispatcher eventDispatcher, 
            StatusType statusType, 
            string textboxtext
        ) : base(game, managerParameters, spriteBatch, eventDispatcher, statusType, textboxtext) {
            this.TextboxText = "Game Log";
        }

        #region Event Handling
        protected override void EventDispatcher_TextboxChanged(EventData eventData)
        {
            //Call base method to show/hide the menu
            base.EventDispatcher_TextboxChanged(eventData);
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

        public void Update()
        {
            this.TextboxText = base.TextboxText;
        }
    }
}
