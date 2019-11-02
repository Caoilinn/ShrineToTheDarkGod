using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class RailController : TargetController
    {
        private RailParameters rail;
        private bool isFirstTime;
        public RailController(string id, 
            ControllerType controllerType, 
            IActor targetActor, RailParameters rail) 
            : base(id, controllerType, targetActor)
        {
            this.rail = rail;       
            this.isFirstTime = true; 
        }
        public override void Update(GameTime gameTime, IActor actor)
        {
            Camera3D parent = actor as Camera3D;
            Actor3D target = this.Target as Actor3D;
            if (this.isFirstTime)
            {
                //put camera on rail
                parent.Transform.Translation = rail.MidPoint;
                this.isFirstTime = false;
            }
            
            if(parent != null && target != null)
            {
                //vector between camera and target
                Vector3 parentToTarget = target.Transform.Translation -
                                                parent.Transform.Translation;
                parentToTarget.Normalize();

                //if negative move towards "start", if positive move "end"
                float dot = Vector3.Dot(parentToTarget, this.rail.Look);

                //calculate where the new position of camera will be
                Vector3 projectedPosition
                    = dot * this.rail.Look + parent.Transform.Translation;

                //dont let the camera move past rail end points
                if (this.rail.InsideRail(projectedPosition))
                {
                    //move the camera so that its "in-front" of object
                    parent.Transform.TranslateTo(projectedPosition);
                }

                //point the camera at the target
                parent.Transform.Look = parentToTarget;
            }

            base.Update(gameTime, actor);
        }
    }
}
