using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class UIButtonObject : UITextureObject
    {
        #region Fields
        private string text;
        private SpriteFont spriteFont;
        private Color textColor;
        private Vector2 textOrigin, textOffset;
        #endregion

        #region Properties
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = (value.Length >= 0) ? value : "Default";
                this.textOrigin = this.spriteFont.MeasureString(text) / 2.0f;
            }
        }

        public SpriteFont SpriteFont
        {
            get
            {
                return this.spriteFont;
            }
            set
            {
                this.spriteFont = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
            }
        }
        #endregion

        #region Constructors
        public UIButtonObject(
            string id,
            ActorType actorType,
            StatusType statusType,
            Transform2D transform,
            Color color,
            SpriteEffects spriteEffects,
            float layerDepth,
            Texture2D texture,
            string text,
            SpriteFont spriteFont,
            Color textColor,
            Vector2 textOffset
        ) : base(id, actorType, statusType, transform, color, spriteEffects, layerDepth, texture) {
            this.spriteFont = spriteFont;

            //Set using the property to also set the text origin
            this.Text = text;
            this.textColor = textColor;
            this.textOffset = textOffset;
        }
        #endregion

        #region
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the texture first
            base.Draw(gameTime, spriteBatch);

            //Draw the overlay text
            spriteBatch.DrawString(
                this.spriteFont,
                this.text,
                this.Transform.Translation + this.textOffset,
                this.textColor,
                0,
                this.textOrigin,
                this.Transform.Scale,
                SpriteEffects.None,
                0.9f * this.LayerDepth //Reduce the layer depth slightly so text is always in front of the texture (remember that 0 = front, 1 = back)
            );
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UIButtonObject other))
                return false;
            else if (this == other)
                return true;

            return this.text.Equals(other.Text)
                && this.spriteFont.Equals(other.SpriteFont)
                && this.textColor.Equals(other.TextColor)
                && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.text.GetHashCode();
            hash = hash * 17 + this.spriteFont.GetHashCode();
            hash = hash * 11 + this.textColor.GetHashCode();
            hash = hash * 7 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            IActor actor = new UIButtonObject(
                "Clone - " + ID,                        //Deep
                this.ActorType,                         //Deep
                this.StatusType,                        //Deep - enum type
                (Transform2D)this.Transform.Clone(),    //Deep - calls the clone for Transform3D explicitly
                this.Color,                             //Deep 
                this.SpriteEffects,                     //Deep - enum type
                this.LayerDepth,                        //Deep
                this.Texture,                           //Shallow
                this.text,                              //Deep
                this.spriteFont,                        //Dhallow
                this.textColor,                         //Deep
                this.textOffset                         //Deep
            );

            //Clone each of the (behavioural) controllers, if we have any controllers attached
            if (this.ControllerList != null)
                foreach (IController controller in this.ControllerList)
                    actor.AttachController((IController)controller.Clone());

            return actor;
        }

        public override bool Remove()
        {
            this.text = null;
            return base.Remove();
        }
        #endregion
    }
}