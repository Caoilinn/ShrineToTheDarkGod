using System;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public interface IEffectParameters : ICloneable
    {
        void SetParameters(Camera3D camera);
        void SetWorld(Matrix world);
    }
}