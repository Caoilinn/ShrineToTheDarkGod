/*
Function: 		Represents the parent class for all updateable AND drawn 3D game objects. Notice that Effect has been added.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class DrawnActor3D : Actor3D, ICloneable
    {
        #region Fields
        private EffectParameters effectParameters;
        #endregion

        #region Properties
        public EffectParameters EffectParameters
        {
            get
            {
                return this.effectParameters;
            }
            set
            {
                this.effectParameters = value;
            }
        }
        #endregion

        #region Constructors
        public DrawnActor3D(
            string id, 
            ActorType actorType, 
            StatusType statusType, 
            Transform3D transform, 
            EffectParameters effectParameters
        ) : base(id, actorType, statusType, transform) {
            this.effectParameters = effectParameters;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            DrawnActor3D other = obj as DrawnActor3D;

            if (other == null)
                return false;
            else if (this == other)
                return true;

            return this.effectParameters.Equals(other.EffectParameters) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 31 + this.effectParameters.GetHashCode();
            hash = hash * 43 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            return new DrawnActor3D(
                "Clone - " + ID,                                //Deep
                this.ActorType,                                 //Deep
                this.StatusType,                                //Deep - a simple numeric type
                (Transform3D)this.Transform.Clone(),            //Deep - calls the clone for Transform3D explicitly
                (EffectParameters)this.EffectParameters.Clone() //Hybrid - shallow (texture and effect) and deep (all other fields)
            );
        }

        //Notice we add a Draw() method since this will be the parent class for ModelObject, PRimitiveObject, CollidableObject
        public virtual void Draw(GameTime gameTime, Camera3D camera)
        {

        }
        #endregion
    }
}