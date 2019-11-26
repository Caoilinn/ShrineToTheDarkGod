using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class TrackController : Controller
    {
        #region Fields
        private Transform3DCurve transform3DCurve;
        #endregion

        public TrackController(
            string id, 
            ControllerType controllerType,   
            Transform3DCurve transform3DCurve
        ) : base(id, controllerType) {
            this.transform3DCurve = transform3DCurve;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if(parent != null)
            {
                Vector3 translation, look, up;

                this.transform3DCurve.Evalulate(
                    (float) gameTime.TotalGameTime.TotalMilliseconds, 
                    3,
                    out translation, 
                    out look, 
                    out up
                ); //pass-by-ref

                parent.Transform.TranslateTo(translation);
                parent.Transform.Look = look;
                parent.Transform.Up = up;
            }

            base.Update(gameTime, actor);
        }
    }
}