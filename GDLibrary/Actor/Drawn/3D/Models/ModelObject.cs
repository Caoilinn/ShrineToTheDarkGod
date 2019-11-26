/*
Function: 		Allows us to draw models objects. These are the FBX files we export from 3DS Max. 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class ModelObject : DrawnActor3D, ICloneable
    {
        #region Fields
        private Model model;
        private Matrix[] boneTransforms;
        #endregion

        #region Properties
        public Model Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        public Matrix[] BoneTransforms
        {
            get
            {
                return this.boneTransforms;
            }
            set
            {
                this.boneTransforms = value;
            }
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                //Bug fix for disappearing skybox plane - scale the bounding sphere up by 10%
                return this.model.Meshes[model.Root.Index].BoundingSphere.Transform(Matrix.CreateScale(1.1f) * this.GetWorldMatrix());
            }
        }
        #endregion

        #region Constructors
        public ModelObject(
            string id, 
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model
        ) : this(id, actorType, StatusType.Update | StatusType.Drawn, transform, effectParameters, model) {

        }

        public ModelObject(
            string id, 
            ActorType actorType, 
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model
        ) : base(id, actorType, statusType, transform, effectParameters) {
            this.model = model;

            /* 
            * 3DS Max models contain meshes (e.g. a table might have 5 meshes i.e. a top and 4 legs) and each mesh contains a bone.
            * A bone holds the transform that says "move this mesh to this position". 
            * Without 5 bones in a table all the meshes would collapse down to be centred on the origin our table, wouldnt look very much like a table!
            */

            //Load bone transforms and copy transfroms to transform array (transforms)
            if (this.model != null)
            {
                this.boneTransforms = new Matrix[this.model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(this.boneTransforms);
            }

            InitializeBoneTransforms();
        }
        #endregion

        #region Methods
        private void InitializeBoneTransforms()
        {
            //Load bone transforms and copy transfroms to transform array (transforms)
            if (this.model != null)
            {
                this.boneTransforms = new Matrix[this.model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(this.boneTransforms);
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj as ModelObject).Model == null || this.model == null) return false;

            if (!(obj is ModelObject other))
                return false;
            else if (this == other)
                return true;

            return this.model.Equals(other.Model) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            if (model != null) hash = hash * 11 + this.model.GetHashCode();
            hash = hash * 17 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            ModelObject actor = new ModelObject(
                "clone - " + ID,                                    //Deep
                this.ActorType,                                     //Deep
                this.StatusType,                                    //Deep
                (Transform3D) this.Transform.Clone(),               //Deep
                (EffectParameters) this.EffectParameters.Clone(),   //Hybrid - shallow (texture and effect) and deep (all other fields) 
                this.model                                          //Shallow i.e. reference
            );

            return actor;
        }
        #endregion
    }
}