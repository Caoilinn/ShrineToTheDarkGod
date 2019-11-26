using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class CollidableArchitecture : TriangleMeshObject
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        public CollidableArchitecture(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            Model lowPolygonModel,
            MaterialProperties materialProperties
        ) : base(id, actorType, transform, effectParameters, model, lowPolygonModel, materialProperties) {

            //Register for callback on CDCR
            this.Body.CollisionSkin.callbackFn += CollisionSkin_callbackFn;
        }

        #region Event Handling
        protected override bool CollisionSkin_callbackFn(CollisionSkin collider, CollisionSkin collidee)
        {
            HandleCollisions(collider.Owner.ExternalData as CollidableObject, collidee.Owner.ExternalData as CollidableObject);
            return true;
        }

        //How do we want this object to respond to collisions?
        protected override void HandleCollisions(CollidableObject collidableObjectCollider, CollidableObject collidableObjectCollidee)
        {
        }
        #endregion
    }
}