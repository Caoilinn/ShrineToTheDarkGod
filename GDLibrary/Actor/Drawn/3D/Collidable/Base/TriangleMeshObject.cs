using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    /// <summary>
    /// TriangleMeshObjects should be used sparingly. This is because the collision surface is generated
    /// from the vertex data inside the original model. This makes collision detection relatively more
    /// expensive than say a CollidableObject with primitive(s) added.
    /// 
    /// You should note that ALL triangle mesh objects are STATIC after they are placed. 
    /// That is, they cannot move after initial placement.
    /// </summary>
    public class TriangleMeshObject : CollidableObject
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Consturctors
        public TriangleMeshObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            MaterialProperties materialProperties
        ) : this(id, actorType, transform, effectParameters, model, null, materialProperties) {

        }

        public TriangleMeshObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            Model lowPolygonModel,
            MaterialProperties materialProperties
        ) : base(id, actorType, transform, effectParameters, model) {
            this.Collision = new CollisionSkin(Body);
            this.Body.ExternalData = this;
            this.Body.CollisionSkin = this.Collision;

            //Get the primitive mesh which forms the skin - use low poly if it has been provided in the constructor
            TriangleMesh triangleMesh = null;

            if (lowPolygonModel != null)
                triangleMesh = GetTriangleMesh(lowPolygonModel, this.Transform);
            else
                triangleMesh = GetTriangleMesh(model, this.Transform);

            //Add the primitive mesh to the collision skin   
            this.Body.CollisionSkin.AddPrimitive(triangleMesh, materialProperties);
        }
        #endregion

        #region Event Handling
        protected virtual bool CollisionSkin_callbackFn(CollisionSkin collider, CollisionSkin collidee)
        {
            HandleCollisions(collider.Owner.ExternalData as CollidableObject, collidee.Owner.ExternalData as CollidableObject);
            return true;
        }

        //How do we want this object to respond to collisions?
        protected virtual void HandleCollisions(CollidableObject collidableObjectCollider, CollidableObject collidableObjectCollidee)
        {
        }
        #endregion

        #region Methods
        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale)
                * Body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation
                * Body.Orientation
                * this.Transform.Orientation
                * Matrix.CreateTranslation(Body.Position)
                * Matrix.CreateTranslation(this.Transform.Translation);
        }

        private TriangleMesh GetTriangleMesh(Model model, Transform3D transform)
        {
            TriangleMesh triangleMesh = new TriangleMesh();
            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();

            ExtractData(vertexList, indexList, model);

            for (int i = 0; i < vertexList.Count; i++)
                vertexList[i] = Vector3.Transform(vertexList[i], transform.World);

            //Create the collision mesh
            triangleMesh.CreateMesh(vertexList, indexList, 1, 1.0f);

            return triangleMesh;
        }

        public override void Enable(bool bImmovable, float mass)
        {
            this.Mass = mass;

            //Set whether the object can move
            this.Body.Immovable = true;

            //Calculate the centre of mass
            Vector3 com = SetMass(mass);

            //Set the centre of mass
            this.Body.CollisionSkin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            //Enable so that any applied forces (e.g. gravity) will affect the object
            this.Body.EnableBody();
        }

        public void ExtractData(List<Vector3> vertices, List<TriangleVertexIndices> indices, Model model)
        {
            Matrix[] bones_ = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones_);

            foreach (ModelMesh mm in model.Meshes)
            {
                int offset = vertices.Count;
                Matrix xform = bones_[mm.ParentBone.Index];
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    Vector3[] a = new Vector3[mmp.NumVertices];
                    int stride = mmp.VertexBuffer.VertexDeclaration.VertexStride;
                    mmp.VertexBuffer.GetData(mmp.VertexOffset * stride, a, 0, mmp.NumVertices, stride);

                    for (int i = 0; i != a.Length; ++i)
                        Vector3.Transform(ref a[i], ref xform, out a[i]);

                    vertices.AddRange(a);

                    if (mmp.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)
                        throw new Exception(String.Format("Model uses 32-bit indices, which are not supported."));

                    short[] s = new short[mmp.PrimitiveCount * 3];
                    mmp.IndexBuffer.GetData(mmp.StartIndex * 2, s, 0, mmp.PrimitiveCount * 3);

                    JigLibX.Geometry.TriangleVertexIndices[] tvi = new JigLibX.Geometry.TriangleVertexIndices[mmp.PrimitiveCount];
                    for (int i = 0; i != tvi.Length; ++i)
                    {
                        tvi[i].I0 = s[i * 3 + 2] + offset;
                        tvi[i].I1 = s[i * 3 + 1] + offset;
                        tvi[i].I2 = s[i * 3 + 0] + offset;
                    }
                    indices.AddRange(tvi);
                }
            }
        }
        #endregion
    }
}