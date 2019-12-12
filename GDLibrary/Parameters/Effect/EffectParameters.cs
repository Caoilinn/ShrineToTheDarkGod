/*
Function: 		Encapsulates the effect, texture, color (ambient, diffuse, specular, emmissive), and alpha fields for any drawn 3D object.
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class EffectParameters : IEffectParameters
    {
        #region Statics
        //Rounding on alpha setter
        protected static readonly int DefaultAlphaRoundPrecision = 3;
        protected static readonly float DefaultAlpha = 1;
        #endregion

        #region Fields
        //Shader reference
        private Effect effect;
        
        //Texture
        private Texture2D texture;
        
        //Color specific
        //Defaults in case the developer forgets to set these values when adding a model object (or child object).
        //Setting these values prevents us from seeing only a black surface (i.e. no texture, no color) or no object at all (alpha = 0).
        private Color diffuseColor = Color.White;
        private float alpha = DefaultAlpha;

        //Reset
        private EffectParameters originalEffectParameters;
        #endregion

        #region Properties
        public Effect Effect
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
                    this.alpha = (float)Math.Round(value, DefaultAlphaRoundPrecision);
            }
        }

        public EffectParameters OriginalEffectParameters
        {
            get
            {
                return this.originalEffectParameters;
            }
        }
        #endregion

        #region Constructors
        //Used to set originalEffectParameters only
        private EffectParameters()
        {
        }

        //Used to create the simplest instance of the class - fields will be set by each instanciating object - see Main::InitializeEffects()
        public EffectParameters(
            Effect effect
        ) : this(effect, null, Color.White, DefaultAlpha) {

        }

        //For objects with colour but no texture e.g. a glass cube
        public EffectParameters(
            Effect effect, 
            Color diffusecolor, 
            float alpha
        ) : this(effect, null, diffusecolor, alpha) {

        }

        //For objects with texture and alpha but no specular or emmissive
        public EffectParameters(Effect effect, Texture2D texture, Color diffusecolor, float alpha)
        {
            Initialize(effect, texture, diffuseColor, alpha);

            //Store original values in case of reset
            this.originalEffectParameters = new EffectParameters();
            this.originalEffectParameters.Initialize(effect, texture, diffuseColor, alpha);
        }
        #endregion

        #region Methods
        protected void Initialize(Effect effect, Texture2D texture, Color diffuseColor, float alpha)
        {
            this.effect = effect;
            if (texture != null)
                this.texture = texture;

            //Use property to ensure values are inside correct ranges
            Alpha = alpha;
        }

        protected virtual void Reset()
        {
            Initialize(
                this.originalEffectParameters.Effect,
                this.originalEffectParameters.Texture,
                this.originalEffectParameters.DiffuseColor,
                this.originalEffectParameters.Alpha
            );
        }

        public virtual void SetParameters(Camera3D camera)
        {
            //Apply or serialise the variables above to the GFX card
            this.effect.CurrentTechnique.Passes[0].Apply();
        }

        //Used by animated models
        public virtual void SetParameters(Camera3D camera, Matrix[] bones)
        {
            //Apply or serialise the variables above to the GFX card
            this.effect.CurrentTechnique.Passes[0].Apply();
        }

        //Used by billboards
        public virtual void SetParameters(Camera3D camera, BillboardOrientationParameters billboardParameters)
        {
        }

        public virtual void SetWorld(Matrix world)
        {
        }
        
        public virtual EffectParameters GetDeepCopy()
        {
            return new EffectParameters(
                this.effect,        //Shallow - a reference
                this.texture,       //Shallow - a reference
                this.diffuseColor,  //Deep
                this.alpha          //Deep
            );
        }

        public virtual object Clone()
        {
            return GetDeepCopy();
        }
        #endregion
    }
}