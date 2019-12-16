using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class CollidableObject : ModelObject
    {
        #region Fields
        private Body body;
        private CollisionSkin collision;
        private float mass;
        #endregion

        #region Properties
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }

        public CollisionSkin Collision
        {
            get
            {
                return collision;
            }
            set
            {
                collision = value;
            }
        }

        public Body Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
            }
        }
        #endregion

        #region Constructors
        public CollidableObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model
        ) : base(id, actorType, transform, effectParameters, model) {
            this.body = new Body {
                ExternalData = this
            };

            this.collision = new CollisionSkin(this.body);
            this.body.CollisionSkin = this.collision;
        }
        #endregion

        #region Methods
        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale)
                * this.collision.GetPrimitiveLocal(0).Transform.Orientation
                * this.body.Orientation
                * this.Transform.Orientation
                * Matrix.CreateTranslation(this.body.Position);
        }

        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Density,
                mass
            );

            this.collision.GetMassProperties(
                primitiveProperties,
                out float junk,
                out Vector3 com,
                out Matrix it,
                out Matrix itCoM
            );

            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }

        public void AddPrimitive(Primitive primitive, MaterialProperties materialProperties)
        {
            this.collision.AddPrimitive(primitive, materialProperties);
        }

        public virtual void Enable(bool bImmovable, float mass)
        {
            this.mass = mass;

            //Set whether the object can move
            this.body.Immovable = bImmovable;

            //Calculate the centre of mass
            Vector3 com = SetMass(mass);

            //Adjust skin so that it corresponds to the 3D mesh as drawn on screen
            this.body.MoveTo(this.Transform.Translation, Matrix.Identity);

            //Set centre of mass
            this.collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            //Enable so that any applied forces (e.g. gravity) will affect the object
            this.body.EnableBody();
        }

        public new object Clone()
        {
            return new CollidableObject(
                "Clone - " + ID,                            //Deep
                this.ActorType,                             //Deep
                (Transform3D) this.Transform.Clone(),       //Deep
                this.EffectParameters.GetDeepCopy(),        //Hybrid - shallow (texture and effect) and deep (all other fields) 
                this.Model                                  //Shallow i.e. reference
            );
        }

        public override bool Remove()
        {
            this.body = null;
            return base.Remove();
        }
        #endregion
    }
}