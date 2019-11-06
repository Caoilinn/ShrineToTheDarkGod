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
        #endregion

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
        }

        public override bool Equals(object obj)
        {
            ModelObject other = obj as ModelObject;

            if (other == null)
                return false;
            else if (this == other)
                return true;

            return this.model.Equals(other.Model) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 11 + this.model.GetHashCode();
            hash = hash * 17 + base.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            ModelObject actor = new ModelObject(
                "clone - " + ID, //deep
                this.ActorType,   //deep
                this.StatusType, //deep
                (Transform3D) this.Transform.Clone(),  //deep
                (EffectParameters) this.EffectParameters.Clone(), //hybrid - shallow (texture and effect) and deep (all other fields) 
                this.model //shallow i.e. a reference
            );

            return actor;
        }

        public override void Draw(GameTime gameTime, Camera3D camera)
        {
            //added these two local variables to make the code a little less dense and more readable
            BasicEffect effect = this.EffectParameters.Effect;
            EffectParameters effectParameters = this.EffectParameters;

            //set object position, rotation, scale and set camera
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = this.Transform.World;

            //set surface properties for drawn object
            effect.Texture = effectParameters.Texture;
            effect.Alpha = effectParameters.Alpha;
            effect.DiffuseColor = effectParameters.DiffuseColor.ToVector3();

            //set all the variables above for the next pass that we draw!
            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = this.EffectParameters.Effect;
                }
                this.EffectParameters.Effect.World
                    = this.BoneTransforms[mesh.ParentBone.Index] * this.Transform.World;
                mesh.Draw();
            }
        }
    }
}