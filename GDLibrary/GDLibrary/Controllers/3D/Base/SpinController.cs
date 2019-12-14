using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class SpinController : Controller
    {
        #region Fields
        private float rotationMagnitude;
        #endregion

        public SpinController(
            string id,
            ControllerType controllerType,
            float rotationMagnitude
        ) : base(id, controllerType) {
            this.rotationMagnitude = rotationMagnitude;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;
            if (actor != null) parent.Transform.TranslateBy((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * new Vector3(0, 0.05f, 0));
            if (actor != null) parent.Transform.RotateBy(new Vector3(0, 0.5f, 0));

            base.Update(gameTime, actor);
        }
    }
}