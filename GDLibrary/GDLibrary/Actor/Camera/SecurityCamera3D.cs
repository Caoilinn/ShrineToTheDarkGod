using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class SecurityCamera3D : Camera3D
    {
        public SecurityCamera3D(
            string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            ProjectionParameters projectionParameters,
            Viewport viewport,
            float drawDepth,
            Keys[] moveKeys, 
            float moveSpeed, 
            float rotateSpeed
        ) : base(id, actorType, statusType, transform, projectionParameters, viewport, drawDepth) {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
