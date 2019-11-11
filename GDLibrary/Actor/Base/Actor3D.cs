/*
Function: 		Represents the parent class for all updateable 3D game objects. Notice that Transform3D and List<IController> has been added.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using System;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class Actor3D : Actor, ICloneable
    {
        #region Fields
        private Transform3D transform;
        #endregion

        #region Properties
        public Transform3D Transform
        {
            get
            {
                return this.transform;
            }
            set
            {
                this.transform = value;
            }
        }
        #endregion

        #region Constructor
        public Actor3D(
            string id, 
            ActorType actorType,                
            StatusType statusType, 
            Transform3D transform
        ) : base(id, actorType, statusType) {
            this.Transform = transform;
        }
        #endregion

        #region Methods
        public override Matrix GetWorldMatrix()
        {
            //Returns the compound matrix transformation that will scale, rotate and place the actor in the 3D world of the game
            return this.transform.World;
        }

        public override bool Remove()
        {
            //Tag for garbage collection
            this.transform = null;
            return base.Remove();
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Actor3D other))
                return false;
            else if (this == other)
                return true;

            return this.Transform.Equals(other.Transform) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 31 + this.Transform.GetHashCode();
            hash = hash * 17 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            return new Actor3D("clone - " + ID, //deep
                this.ActorType, //deep
                this.StatusType, //shallow
                (Transform3D)this.transform.Clone()); //deep
                
        }
        #endregion
    }
}