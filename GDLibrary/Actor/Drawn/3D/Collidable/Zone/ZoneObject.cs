/*
Function: 		Represents a collidable zone within the game - subclass to make TriggerCameraChangeZone, WinLoseZone etc 
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class ZoneObject : CollidableObject
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public ZoneObject(
            string id, 
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model
        ) : base(id, actorType, transform, effectParameters, model) {

            //Register for callback on CDCR
            this.Body.CollisionSkin.callbackFn += CollisionSkin_callbackFn;
        }
        #endregion

        #region Event Handling
        protected virtual bool CollisionSkin_callbackFn(CollisionSkin collider, CollisionSkin collidee)
        {
            HandleCollisions(collider.Owner.ExternalData as CollidableObject, collidee.Owner.ExternalData as CollidableObject);

            //Can walk through this object BUT it will still detect a collision
            return false;
        }

        //How do we want this object to respond to collisions?
        protected virtual void HandleCollisions(CollidableObject collider, CollidableObject collidee)
        {
            //If player hits me then remove me
            if (collider.ActorType == ActorType.CollidableZone)
            {
            }
        }
        #endregion
    }
}