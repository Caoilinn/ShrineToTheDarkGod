/*
Function: 		Represents an immoveable, collectable and collidable object within the game that can be picked up (e.g. a sword on a heavy stone altar that cannot be knocked over) 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class PickupObject : ModelObject
    {
        #region Fields
        private PickupParameters pickupParameters;
        #endregion

        #region Properties
        public PickupParameters PickupParameters
        {
            get
            {
                return this.pickupParameters;
            }
            set
            {
                this.pickupParameters = value;
            }
        }
        #endregion

        #region Constructors
        public PickupObject(
            string id, 
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            PickupParameters pickupParameters
        ) : base(id, actorType, transform, effectParameters, model) {
            this.pickupParameters = pickupParameters;
        }
        #endregion
    }
}
