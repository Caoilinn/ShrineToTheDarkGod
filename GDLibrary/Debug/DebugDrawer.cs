using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class DebugDrawer : PausableDrawableGameComponent
    {
        #region Fields
        private SpriteBatch spriteBatch;
        private CameraManager cameraManager;
        private SpriteFont spriteFont;
        private Vector2 position;
        private Color color;
        private int fpsRate;
        private int totalTime, count;
        private string strInfo = "[Cameras: WASD, TGFH]. [Use LMB and MMB to remove/pick. Set pick behavior in Main::Initalize()]";
        private Vector2 positionOffset = new Vector2(0, 20);
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public DebugDrawer(
            Game game,
            CameraManager cameraManager,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            Vector2 position,
            Color color
        ) : base(game, eventDispatcher, statusType) {
            this.spriteBatch = spriteBatch;
            this.cameraManager = cameraManager;
            this.spriteFont = spriteFont;
            this.position = position;
            this.color = color;
        }
        #endregion

        #region Methods
        protected override void ApplyUpdate(GameTime gameTime)
        {
            this.totalTime += gameTime.ElapsedGameTime.Milliseconds;
            this.count++;

            //1 second
            if(this.totalTime >= 1000)
            {
                this.fpsRate = count;
                this.totalTime = 0;
                this.count = 0;
            }

            base.ApplyUpdate(gameTime);
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            //Draw the debug info for all of the cameras in the cameramanager
            foreach (Camera3D activeCamera in this.cameraManager)
            {
                //Set the viewport dimensions to the size defined by the active camera
                Game.GraphicsDevice.Viewport = activeCamera.Viewport;
                this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

                //Framerate
                this.spriteBatch.DrawString(this.spriteFont, "FPS: " + this.fpsRate, this.position, this.color);

                //Camera info
                this.spriteBatch.DrawString(this.spriteFont, activeCamera.GetDescription(), this.position + this.positionOffset, this.color);

                //String info
                this.spriteBatch.DrawString(this.spriteFont, this.strInfo, this.position + 2 * this.positionOffset, this.color);
                this.spriteBatch.End();
            }

            base.ApplyDraw(gameTime);
        }
        #endregion
    }
}