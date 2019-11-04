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
        #region Fields
        private Vector3 translation, rotation, scale;
        private Vector3 originalRotation;
        private Vector3 look, up;
        private Matrix world;
        private bool isDirty;
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

        private Vector3 originalLook;
        private Vector3 originalUp;

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
        #endregion

        #region Methods
        //Used by drawn objects
        public Transform3D(Vector3 translation, Vector3 rotation, Vector3 scale, Vector3 look, Vector3 up)
        {
            this.Translation = translation;
            this.originalRotation = this.Rotation = rotation;
            this.Scale = scale;

            this.originalLook = this.Look = Vector3.Normalize(look);
            this.originalUp = this.Up = Vector3.Normalize(up);
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

            //update the look and up
            this.Look = Vector3.Transform(this.originalLook, rot);
            this.Up = Vector3.Transform(this.originalUp, rot);
        }

        public void TranslateTo(Vector3 translate) //set
        {
            Translation = translate;
        }

        public void TranslateBy(Vector3 translateBy) //shift/move/delta
        {
            Translation += translateBy;
        }

        public void ScaleTo(Vector3 scale) //(1,2,1) set scale as x, 2Y, z
        {
            Scale = scale;
        }

        public void ScaleBy(Vector3 scaleBy) //(1, 2, 1) stretch along Y
        {
            Scale *= scaleBy;
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new Transform3D(this.translation, this.rotation, this.scale,
                this.look, this.up);

            //If class contains primitive or in-built types (e.g. Vector3, float)
            //Return this.MemberwiseClone();
        }
        #endregion
    }
}