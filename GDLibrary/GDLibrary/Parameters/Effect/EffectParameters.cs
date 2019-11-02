/*
Function: 		Encapsulates the effect, texture, color (diffuse etc ) and alpha fields for any drawn 3D object.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class EffectParameters : ICloneable
    {
        #region Fields
        //shader reference
        private BasicEffect effect;

        //texture
        private Texture2D texture;

        //transparency
        float alpha;

        //defaults in case the developer forgets to set these values when adding a model object (or child object).
        //setting these values prevents us from seeing only a black surface (i.e. no texture, no color) or no object at all (alpha = 0).
        private Color diffuseColor = Color.White;
        #endregion

        #region Properties
        public BasicEffect Effect
        {
            get
            {
                return this.effect;
            }
            set
            {
                this.effect = value;
            }
        }
        public Texture2D Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
            }
        }
        public Color DiffuseColor
        {
            get
            {
                return this.diffuseColor;
            }
            set
            {
                this.diffuseColor = value;
            }
        }

        public float Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                if (value < 0)
                    this.alpha = 0;
                else if (value > 1)
                    this.alpha = 1;
                else
                    this.alpha = (float)Math.Round(value, 2); //2 decimal places e.g. 0.99
            }
        }

        #endregion

        //objects with texture and alpha but no specular or emmissive
        public EffectParameters(BasicEffect effect, Texture2D texture, Color diffusecolor, float alpha)
        {
            Effect = effect;

            if (texture != null)
                Texture = texture;

            DiffuseColor = diffuseColor;

            //use Property to ensure values are inside correct ranges
            Alpha = alpha;
        }

        //to do...
        public override bool Equals(object obj)
        {
            EffectParameters other = obj as EffectParameters;

            if (other == null)
                return false;
            else if (this == other)
                return true;

            return this.Effect == other.Effect
                && this.Alpha.Equals(other.Alpha)
                    && this.Texture.Equals(other.Texture)
                        && this.DiffuseColor.Equals(other.DiffuseColor);
        }

        public override int GetHashCode()
        {
            int hash = 31 + this.Alpha.GetHashCode();
            hash = hash * 43 + this.Texture.GetHashCode();
            hash = hash * 51 + this.DiffuseColor.GetHashCode();
            return hash;
        }

        public /*new*/ object Clone()
        {
            return new EffectParameters(this.effect, //ref - shallow
                this.texture,  //ref - shallow
                this.diffuseColor, //in-built type - so deep
                this.alpha); //primitive type - so deep
        }

    }
}
