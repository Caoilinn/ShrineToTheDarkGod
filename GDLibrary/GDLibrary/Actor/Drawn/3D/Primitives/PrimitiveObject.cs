/*
Function: 		Allows us to draw primitives by explicitly defining the vertex data.              
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;
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

        public BoundingSphere BoundingSphere
        {
            get
            {
                return new BoundingSphere(this.Transform.Translation, this.Transform.Scale.Length());
            }
        }
        #endregion

        #region Constructors
        public PrimitiveObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            IVertexData vertexData
        ) : base(id, actorType, transform, effectParameters) {
            this.VertexData = vertexData;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is PrimitiveObject other))
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
            PrimitiveObject actor = new PrimitiveObject(
                "Clone - " + ID,                                    //Deep
                this.ActorType,                                     //Deep
                (Transform3D)this.Transform.Clone(),                //Deep
                (EffectParameters)this.EffectParameters.Clone(),    //Deep
                this.vertexData                                     //Shallow - its ok if objects refer to the same vertices
            );

            if (this.ControllerList != null) {

                //Clone each of the (behavioural) controllers
                foreach (IController controller in this.ControllerList)
                    actor.AttachController((IController)controller.Clone());
            }

            return actor;
        }
        #endregion
    }
}