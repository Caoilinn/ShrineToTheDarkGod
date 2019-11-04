using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
            Keys[] moveKeys, 
            float moveSpeed, 
            float rotateSpeed
        ) : base(id, actorType, statusType, transform, projectionParameters) {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
