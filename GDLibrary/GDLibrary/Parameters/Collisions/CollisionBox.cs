using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDLibrary
{
    public class CollisionBox
    {
        #region Statics
        public static CollisionBox Zero
        {
            get
            {
                return new CollisionBox(0, 0, 0, 0, 0, 0);
            }
        }
        #endregion

        #region Fields
        private float x;
        private float y;
        private float z;
        private float width;
        private float height;
        private float depth;
        private float maxX;
        private float maxY;
        private float maxZ;
        #endregion

        #region Properties
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public float Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public float Depth
        {
            get
            {
                return this.depth;
            }
            set
            {
                this.depth = value;
            }
        }

        public float MaxX
        {
            get
            {
                return this.maxX;
            }
            set
            {
                this.maxX = value;
            }
        }

        public float MaxY
        {
            get
            {
                return this.maxY;
            }
            set
            {
                this.maxY = value;
            }
        }

        public float MaxZ
        {
            get
            {
                return this.maxZ;
            }
            set
            {
                this.maxZ = value;
            }
        }
        #endregion

        #region Constructors
        public CollisionBox(
            float x,
            float y,
            float z,
            float width,
            float height,
            float depth
        ) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.maxX = x + width;
            this.maxY = y + height;
            this.maxZ = z + depth;
        }
        #endregion

        #region Methods
        public bool Intersects(CollisionBox b)
        {
            return (

                //(If left side intersects || If right side intersects) - On X
                (this.maxX > b.x) && (this.maxX < b.maxX) || (this.x > b.x) && (this.x < b.maxX) &&

                //And - (If top side intersects || If bottom side intersects) - On Y
                (this.maxY > b.y) && (this.maxY < b.maxY) || (this.y > b.y) && (this.y < b.maxY) &&

                //And - (If front side intersects || If back side intersects) - On Z
                (this.maxZ > b.z) && (this.maxZ < b.maxZ) || (this.z > b.z) && (this.z < b.maxZ)

            );
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}