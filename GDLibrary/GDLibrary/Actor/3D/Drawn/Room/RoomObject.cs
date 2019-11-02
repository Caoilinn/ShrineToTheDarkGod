using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class RoomObject : DrawnActor3D, ICloneable
    {
        #region Fields
        private double x;
        private double y;
        private double z;
        #endregion

        #region Properties
        public double X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }
        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        public double Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }
        #endregion

        #region Methods
        public RoomObject(
            string id, 
            ActorType actorType,
            StatusType statusType, 
            Transform3D transform,
            EffectParameters effectParameters, 
            double x,
            double y,
            double z
        ) : base(id, actorType, statusType, transform, effectParameters) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override void Draw(GameTime gameTime, Camera3D camera)
        {
            BasicEffect effect = this.EffectParameters.Effect;
            EffectParameters effectParameters = this.EffectParameters;

            //set object position, rotation, scale and set camera
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = this.Transform.World;

            //set surface properties for drawn object
            effect.Texture = effectParameters.Texture;
            effect.Alpha = effectParameters.Alpha;
            effect.DiffuseColor = effectParameters.DiffuseColor.ToVector3();

            //set all the variables above for the next pass that we draw!
            effect.CurrentTechnique.Passes[0].Apply();

            //move vertices to GPU on VRAM and draw the damn thing!!!
            //this.vertexData.Draw(gameTime, effect);

            //nothing happens in the base, so we dont bother calling this method
            //   base.Draw(gameTime, camera);
        }
        #endregion
    }
}
