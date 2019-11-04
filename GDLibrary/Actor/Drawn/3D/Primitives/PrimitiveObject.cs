/*
Function: 		Allows us to draw primitives by explicitly defining the vertex data.              
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class PrimitiveObject : DrawnActor3D, ICloneable
    {
        #region Fields
        private IVertexData vertexData;
        #endregion

        #region Properties     
        public IVertexData VertexData
        {
            get
            {
                return this.vertexData;
            }
            set
            {
                this.vertexData = value;
            }
        }
        #endregion

        public PrimitiveObject(string id, ActorType actorType, 
            StatusType statusType, Transform3D transform, 
            EffectParameters effectParameters, IVertexData vertexData) 
            : base(id, actorType, statusType, transform, effectParameters)
        {
            this.VertexData = vertexData;
        }

        public override void Draw(GameTime gameTime, Camera3D camera)
        {
          
                //added these two local variables to make the code a little less dense and more readable
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
                this.vertexData.Draw(gameTime, effect);
    

            //nothing happens in the base, so we dont bother calling this method
         //   base.Draw(gameTime, camera);
        }

        public override bool Equals(object obj)
        {
            PrimitiveObject other = obj as PrimitiveObject;

            if (other == null)
                return false;
            else if (this == other)
                return true;

            return this.VertexData.Equals(other.VertexData);
        }

        public override int GetHashCode()
        {
            int hash = 31 + this.VertexData.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            return new DrawnActor3D("clone - " + ID, //deep
                this.ActorType, //deep
                  this.StatusType, //deep - a simple numeric type
                    (Transform3D)this.Transform.Clone(), //deep - calls the clone for Transform3D explicitly
                        (EffectParameters)this.EffectParameters.Clone()); //hybrid - shallow (texture and effect) and deep (all other fields)            
        }

    }
}
