using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.GDLibrary.Actor._3D.Camera
{
    public class SecurityCamera3D : Camera3D
    {
        public SecurityCamera3D(
            string id, ActorType actorType, StatusType statusType, 
            Transform3D transform, 
            ProjectionParameters projectionParameters,
            Keys[] moveKeys, float moveSpeed, float rotateSpeed) 
            : base(id, actorType, statusType, transform, projectionParameters)
        {
        }

        public override void Update(GameTime gameTime)
        {
            //keys

            //update transform


            base.Update(gameTime);
        }

    }
}
