using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class DollyCameraController : Controller
    {
        public DollyCameraController(string id, 
            ControllerType controllerType) 
            : base(id, controllerType)
        {

        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            //cast to access the Transform3D
            Actor3D parent = actor as Actor3D;

            if(parent != null)
            {
                Vector3 move = parent.Transform.Look
                    * gameTime.ElapsedGameTime.Milliseconds
                            * 0.01f;

                move += parent.Transform.Up
                    * gameTime.ElapsedGameTime.Milliseconds
                            * 0.005f;

                parent.Transform.TranslateBy(move);
            }

            base.Update(gameTime, actor);
        }
    }
}
