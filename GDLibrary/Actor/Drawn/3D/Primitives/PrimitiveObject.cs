///*
//Function: 		Allows us to draw primitives by explicitly defining the vertex data.              
//Author: 		NMCG
//Version:		1.0
//Bugs:			None
//Fixes:			None
//*/
//using Microsoft.Xna.Framework;
//using System;

//namespace GDLibrary
//{
//    public class PrimitiveObject : DrawnActor3D, ICloneable
//    {
//        #region Fields
//        private IVertexData vertexData;
//        #endregion

//        #region Properties     
//        public IVertexData VertexData
//        {
//            get
//            {
//                return this.vertexData;
//            }
//            set
//            {
//                this.vertexData = value;
//            }
//        }
//        #endregion

//        #region Constructors
//        public PrimitiveObject(
//            string id,
//            ActorType actorType,
//            Transform3D transform,
//            EffectParameters effectParameters,
//            IVertexData vertexData
//        ) : base(id, actorType, transform, boundingBox, effectParameters)
//        {
//            this.VertexData = vertexData;
//        }
//        #endregion

//        #region Methods
//        public override void Draw(GameTime gameTime, Camera3D camera)
//        {
//            ////Added these two local variables to make the code a little less dense and more readable
//            //BasicEffect effect = this.EffectParameters.Effect;
//            //EffectParameters effectParameters = this.EffectParameters;

//            ////Set object position, rotation, scale and set camera
//            //effect.View = camera.View;
//            //effect.Projection = camera.Projection;
//            //effect.World = this.Transform.World;

//            ////Set surface properties for drawn object
//            //effect.Texture = effectParameters.Texture;
//            //effect.Alpha = effectParameters.Alpha;
//            //effect.DiffuseColor = effectParameters.DiffuseColor.ToVector3();

//            ////Set all the variables above for the next pass that we draw!
//            //effect.CurrentTechnique.Passes[0].Apply();

//            ////Move vertices to GPU on VRAM and draw the damn thing!!!
//            //this.vertexData.Draw(gameTime, effect);

//            ////nothing happens in the base, so we dont bother calling this method
//            ////base.Draw(gameTime, camera);
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is PrimitiveObject other))
//                return false;
//            else if (this == other)
//                return true;

//            return this.VertexData.Equals(other.VertexData);
//        }

//        public override int GetHashCode()
//        {
//            int hash = 31 + this.VertexData.GetHashCode();
//            return hash;
//        }

//        public new object Clone()
//        {
//            return new DrawnActor3D(
//                "Clone - " + ID,                                    //Deep
//                this.ActorType,                                     //Deep
//                this.StatusType,                                    //Deep - a simple numeric type
//                (Transform3D)this.Transform.Clone(),                //Deep - calls the clone for Transform3D explicitly
//                (EffectParameters)this.EffectParameters.Clone());   //Hybrid - shallow (texture and effect) and deep (all other fields)            
//        }
//        #endregion
//    }
//}