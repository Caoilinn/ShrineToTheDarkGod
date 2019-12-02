using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDApp
{
    public class MyUIManager : UIManager
    {
        public MyUIManager(
            Game game,
            SpriteBatch spriteBatch, 
            EventDispatcher eventDispatcher,
            int initialSize,
            StatusType statusType
        ) : base(game, spriteBatch, eventDispatcher, initialSize, statusType) {
        }

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

        //protected override void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        //{
        //    base.HandleMouseClick(uiObject, gameTime);
        //}

        //protected override void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        //{
        //    base.HandleMouseOver(uiObject, gameTime);
        //}

        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            base.RegisterForEventHandling(eventDispatcher);
        }
    }
}