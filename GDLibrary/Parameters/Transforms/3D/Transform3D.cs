/*
Function: 		Encapsulates the transformation and World matrix specific parameters for any 3D entity that can have a position (e.g. a player, a prop, a camera)
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class Transform3D : ICloneable
    {
        #region Statics
        public static Transform3D Zero
        {
            get
            {
                return new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, -Vector3.UnitZ, Vector3.UnitY);
            }
        }
        #endregion

        #region Fields
        private Vector3 translation, rotation, scale;
        private Vector3 look, up;
        private Matrix world;
        private bool isDirty;

        private Transform3D originalTransform3D;
        private double distanceToCamera;
        #endregion

        #region Properties
        public Matrix Orientation
        {
            get
            {
                return Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) 
                    * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) 
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));
            }
        }

        public Matrix World
        {
            set
            {
                this.world = value;
            }
            get
            {
                if (this.isDirty)
                {
                    this.world = Matrix.Identity
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                        * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z))
                        * Matrix.CreateTranslation(translation);

                    this.isDirty = false;
                }
                return this.world;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return this.translation;
            }
            set
            {
                this.translation = value;
                this.isDirty = true;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
                this.isDirty = true;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
                this.isDirty = true;
            }
        }

        public Vector3 Target
        {
            get
            {
                return this.translation + this.look;
            }
        }

        public Vector3 Up
        {
            get
            {
                return this.up;
            }
            set
            {
                this.up = Vector3.Normalize(value);
                this.isDirty = true;
            }
        }

        public Vector3 Look
        {
            get
            {
                return this.look;
            }
            set
            {
                this.look = Vector3.Normalize(value);
                this.isDirty = true;
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(this.look, this.up));
            }
        }

        public Transform3D OriginalTransform3D
        {
            get
            {
                return this.originalTransform3D;
            }
        }

        public double DistanceToCamera
        {
            get
            {
                return this.distanceToCamera;
            }
            set
            {
                this.distanceToCamera = value;
            }
        }
        #endregion

        #region Constructors
        //Used by drawn objects
        public Transform3D(Vector3 translation, Vector3 rotation, Vector3 scale, Vector3 look, Vector3 up)
        {
            Initialize(translation, rotation, scale, look, up);

            //Store original values in case of reset
            this.originalTransform3D = new Transform3D();
            this.originalTransform3D.Initialize(translation, rotation, scale, look, up);
        }

        //used internally when creating the originalTransform object
        private Transform3D()
        {
        }
        #endregion

        #region Methods
        protected void Initialize(Vector3 translation, Vector3 rotation, Vector3 scale, Vector3 look, Vector3 up)
        {
            this.Translation = translation;
            this.Rotation = rotation;
            this.Scale = scale;

            this.Look = Vector3.Normalize(look);
            this.Up = Vector3.Normalize(up);
        }

        public void Reset()
        {
            this.translation = this.originalTransform3D.Translation;
            this.rotation = this.originalTransform3D.Rotation;
            this.scale = this.originalTransform3D.Scale;
            this.look = this.originalTransform3D.Look;
            this.up = this.originalTransform3D.Up;
        }

        public void RotateBy(Vector3 rotateBy) //in degrees
        {
            //Rotate
            this.rotation += rotateBy;

            //X = Pitch, Y = Yaw, Z = roll
            Matrix rot = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(this.rotation.Y),
                MathHelper.ToRadians(this.rotation.X), 
                MathHelper.ToRadians(this.rotation.Z)
            );

            //Update the look and up
            this.Look = Vector3.Transform(this.originalTransform3D.Look, rot);
            this.Up = Vector3.Transform(this.originalTransform3D.Up, rot);
        }

        public void RotateAroundYBy(float magnitude) //in degrees
        {
            this.rotation.Y += magnitude;
            this.look = Vector3.Normalize(
                Vector3.Transform(
                    this.originalTransform3D.Look, 
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                )
            );
            this.isDirty = true;
        }

        //Set position
        public void TranslateTo(Vector3 translate) //set
        {
            Translation = translate;
        }

        //Shift, Move
        public void TranslateBy(Vector3 translateBy)
        {
            Translation += translateBy;
        }

        //(1,2,1) - Set scale as (x, 2y, z)
        public void ScaleTo(Vector3 scale)
        {
            Scale = scale;
        }

        //(1, 2, 1) - Stretch along Y
        public void ScaleBy(Vector3 scaleBy)
        {
            Scale *= scaleBy;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Transform3D other))
                return false;
            else if (this == other)
                return true;

            return Vector3.Equals(this.translation, other.Translation)
                && Vector3.Equals(this.rotation, other.Rotation)
                && Vector3.Equals(this.scale, other.Scale)
                && Vector3.Equals(this.look, other.Look)
                && Vector3.Equals(this.up, other.Up);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.translation.GetHashCode();
            hash = hash * 17 + this.look.GetHashCode();
            hash = hash * 13 + this.up.GetHashCode();
            return hash;
        }

        public object Clone()
        {
            //Deep because all variables are either C# types (primitives, structs, or enums) or XNA types
            return this.MemberwiseClone();
        }
        #endregion
    }
}