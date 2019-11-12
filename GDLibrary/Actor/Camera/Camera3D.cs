/*
Function: 		Represents a simple static camera in our 3D world to which we will later attach controllers. 
Author: 		NMCG
Version:		1.1
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    //Represents the base camera class to which controllers can be attached (to do...)
    public class Camera3D : Actor3D
    {
        #region Fields
        private ProjectionParameters projectionParameters;
        private Viewport viewport;
        private Vector2 viewportCentre;
        private float drawDepth;
        #endregion

        #region Properties
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(this.Transform.Translation,
                    this.Transform.Translation + this.Transform.Look,
                    this.Transform.Up);
            }
        }

        public Matrix Projection
        {
            get
            {
                return this.projectionParameters.Projection;
            }
        }

        public ProjectionParameters ProjectionParameters
        {
            get
            {
                return this.projectionParameters;
            }
            set
            {
                this.projectionParameters = value;
            }
        }

        public Viewport Viewport
        {
            get
            {
                return this.viewport;
            }
            set
            {
                this.viewport = value;
                this.viewportCentre = new Vector2((this.viewport.Width / 2.0f), (this.viewport.Height / 2.0f));
            }
        }

        public Vector2 ViewportCentre
        {
            get
            {
                return this.viewportCentre;
            }
        }

        public float DrawDepth
        {
            get
            {
                return this.drawDepth;
            }
            set
            {
                this.drawDepth = value;
            }
        }

        public BoundingFrustum BoundingFrustum
        {
            get
            {
                return new BoundingFrustum(this.View * this.projectionParameters.Projection);
            }
        }
        #endregion

        #region Constructors
        public Camera3D(
            string id, 
            ActorType actorType, 
            StatusType statusType,
            Transform3D transform, 
            ProjectionParameters projectionParameters,
            Viewport viewport
        ) : this (id, actorType, statusType, transform, projectionParameters, viewport, 0) {
        }

        public Camera3D(
            string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            ProjectionParameters projectionParameters,
            Viewport viewport,
            float drawDepth
        ) : base(id, actorType, statusType, transform) {
            this.projectionParameters = projectionParameters;
            this.Viewport = viewport;
            this.DrawDepth = drawDepth;
        }

        public Camera3D(
            string id,
            ActorType actorType,
            StatusType statusType,
            Viewport viewport,
            float drawDepth
        ) : this (id, actorType, statusType, Transform3D.Zero, ProjectionParameters.StandardMediumFourThree, viewport, 0) {
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is Camera3D other))
                return false;
            else if (this == other)
                return true;

            return Vector3.Equals(this.Transform.Translation, other.Transform.Translation)
                && Vector3.Equals(this.Transform.Look, other.Transform.Look)
                && Vector3.Equals(this.Transform.Up, other.Transform.Up)
                && this.ProjectionParameters.Equals(other.ProjectionParameters);
        }

        public override int GetHashCode() 
        {
            int hash = 31 + this.Transform.Translation.GetHashCode();
            hash = hash * 17 + this.Transform.Look.GetHashCode();
            hash = hash * 13 + this.Transform.Up.GetHashCode();
            hash = hash * 53 + this.ProjectionParameters.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            return new Camera3D(
                "Clone - " + this.ID,
                this.ActorType, 
                StatusType.Update,
                (Transform3D)this.Transform.Clone(),
                (ProjectionParameters)this.projectionParameters.Clone(),
                this.viewport,
                this.drawDepth
            );
        }

        public override string ToString()
        {
            return this.ID
                + ", Translation: " + MathUtility.Round(this.Transform.Translation, 0)
                + ", Look: " + MathUtility.Round(this.Transform.Look, 0)
                + ", Up: " + MathUtility.Round(this.Transform.Up, 0)
                + ", Depth: " + this.drawDepth;
        }

        public string GetDescription()
        {
            return "Camera: " + this.ID;
        }
        #endregion
    }
}