using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    /// <summary>
    /// Provides a parent class representing a player (or non-player) character
    /// with appropriate collision skin (i.e. capsule) and move rates (e.g. acceleration)
    /// 
    /// Inherit from this class to create a specific player (e.g. PlayerObject extends CharacterObject)
    /// then set Body::velocity and Transform3D::rotation to move and turn the character
    /// - See PlayerObject
    /// </summary>
    public class CharacterObject : CollidableObject
    {
        #region Fields
        private Vector3 currentPosition;
        private Vector3 currentHeading;
        private Vector3 targetPosition;
        private Vector3 targetHeading;
        private Vector3 translation;
        private Vector3 rotation;

        private Vector3 movementVector;
        private Vector3 rotationVector;
        private bool inMotion = false;

        private float moveSpeed;
        private float rotateSpeed;

        private float health;
        private float attack;
        private float defence;
        #endregion

        #region Properties
        public Character CharacterBody
        {
            get
            {
                return this.Body as Character;
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
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                return this.targetPosition;
            }
            set
            {
                this.targetPosition = value;
            }
        }

        public Vector3 CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.currentPosition = value;
            }
        }

        public Vector3 TargetHeading
        {
            get
            {
                return this.targetHeading;
            }
            set
            {
                this.targetHeading = value;
            }
        }

        public Vector3 CurrentHeading
        {
            get
            {
                return this.currentHeading;
            }
            set
            {
                this.currentHeading = value;
            }
        }

        public Vector3 MovementVector
        {
            get
            {
                return this.movementVector;
            }
            set
            {
                this.movementVector = value;
            }
        }

        public Vector3 RotationVector
        {
            get
            {
                return this.rotationVector;
            }
            set
            {
                this.rotationVector = value;
            }
        }

        public bool InMotion
        {
            get
            {
                return this.inMotion;
            }
            set
            {
                this.inMotion = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return this.moveSpeed;
            }
            set
            {
                this.moveSpeed = value;
            }
        }

        public float RotateSpeed
        {
            get
            {
                return this.rotateSpeed;
            }
            set
            {
                this.rotateSpeed = value;
            }
        }

        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                this.health = value;
            }
        }

        public float Attack
        {
            get
            {
                return attack;
            }
            set
            {
                this.attack = value;
            }
        }

        public float Defence
        {
            get
            {
                return defence;
            }
            set
            {
                this.defence = value;
            }
        }
        #endregion

        #region Constructors
        public CharacterObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float width,
            float height,
            float depth,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence
        ) : base(id, actorType, transform, effectParameters, model) {
            this.Body = new Character(
                accelerationRate,
                decelerationRate
            );

            this.Collision = new CollisionSkin(Body);
            this.Body.ExternalData = this;
            this.Body.CollisionSkin = this.Collision;

            Capsule capsule = new Capsule(translation, Matrix.Identity, 154, 154);

            this.Collision.AddPrimitive(
                capsule,
                (int) MaterialTable.MaterialID.NormalSmooth
            );

            this.MovementVector = movementVector;
            this.RotationVector = rotationVector;
            this.MoveSpeed = moveSpeed;
            this.RotateSpeed = rotateSpeed;
            this.Health = health;
            this.Attack = attack;
            this.Defence = defence;
        }
        #endregion

        #region Methods
        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale)
                * this.Collision.GetPrimitiveLocal(0).Transform.Orientation
                * this.Body.Orientation
                * this.Transform.Orientation
                * Matrix.CreateTranslation(this.Body.Position);
        }

        public override void Enable(bool bImmovable, float mass)
        {
            base.Enable(bImmovable, mass);
            Body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);
            Body.AllowFreezing = false;
            Body.EnableBody();
        }

        //#region Collision
        ////Checks each direction for a collision (north, south, east & west)
        //public void DemoCollision(Actor3D parentActor)
        //{
        //    float cellWidth = 254;
        //    Vector3 position = parentActor.Transform.Translation;
        //    Vector3 north = parentActor.Transform.Look;
        //    Vector3 south = -parentActor.Transform.Look;
        //    Vector3 east = parentActor.Transform.Right;
        //    Vector3 west = -parentActor.Transform.Right;

        //    CheckForCollision(position, north, cellWidth);
        //    CheckForCollision(position, south, cellWidth);
        //    CheckForCollision(position, east, cellWidth);
        //    CheckForCollision(position, west, cellWidth);
        //}

        //CollisionSkin skin;
        //private float frac;
        //private Vector3 pos;
        //private Vector3 normal;

        //public object[] CheckForCollisionWithRay(Vector3 start, Vector3 delta)
        //{
        //    Segment seg = new Segment(start, delta);
        //    ImmovableSkinPredicate pred = new ImmovableSkinPredicate();

        //    physicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
        //        out frac,
        //        out skin,
        //        out pos,
        //        out normal,
        //        seg,
        //        pred
        //    );

        //    //If a collision has taken place
        //    if (skin != null && skin.Owner != null)
        //    {
        //        //Return an array containing the collision skin and distance to collision
        //        return new object[] { frac, skin };
        //    }

        //    return null;
        //}

        //private void CheckForCollision(Vector3 position, Vector3 direction, float length)
        //{
        //    Vector3 start = position;
        //    Vector3 delta = direction * length;

        //    object[] collisionInfo = CheckForCollisionWithRay(start, delta);

        //    //If a collision has taken place
        //    if (collisionInfo != null)
        //    {
        //        //Cast distance to collision (frac) to a float
        //        float distanceToCollision = (float)collisionInfo[0];

        //        //Cast the parent actor of the collision skin to a collidable object
        //        CollidableObject collidableObject = ((collisionInfo[1] as CollisionSkin).Owner.ExternalData as CollidableObject);

        //        switch (collidableObject.ActorType)
        //        {
        //            case ActorType.CollidableArchitecture:
        //                CheckForCollisionWithWall(distanceToCollision, direction);
        //                break;

        //            case ActorType.CollidablePickup:
        //                CheckForCollisionWithPickup(distanceToCollision, collidableObject);
        //                break;

        //            case ActorType.Enemy:
        //                CheckForCollisionWithEnemy(distanceToCollision);
        //                break;
        //        }
        //    }
        //}

        //public virtual void CheckForCollisionWithWall(float distanceToCollision, Vector3 collisionDirection)
        //{
        //    //If the wall is in the current cell
        //    if (distanceToCollision <= 0.5f)
        //    {
        //        //Publish an event to say that they way is blocked
        //        EventDispatcher.Publish(
        //            new EventData(
        //                EventActionType.OnWayBlocked,
        //                EventCategoryType.Player,
        //                new object[] { collisionDirection }
        //            )
        //        );
        //    }
        //}

        //public virtual void CheckForCollisionWithPickup(float distanceToCollision, CollidableObject collidableObject)
        //{
        //}

        //public virtual void CheckForCollisionWithEnemy(float distanceToCollision)
        //{
        //}
        //#endregion

        public virtual void HandleMovement()
        {
        }

        public override void Update(GameTime gameTime)
        {
            this.HandleMovement();

            //Update actual position of the model e.g. used by rail camera controllers
            //this.Transform.Translation = this.Body.Transform.Position;
            base.Update(gameTime);
        }
        #endregion
    }

    class ASkinPredicate : CollisionSkinPredicate1
    {
        #region Methods
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (!(skin0.Owner is Character))
                return true;
            else
                return false;
        }
        #endregion
    }

    public class Character : Body
    {
        #region Fields
        private bool isJumping;
        private bool isCrouching;
        private float jumpHeight = 5;

        public float accelerationRate { get; set; }
        public float decelerationRate { get; set; }
        public Vector3 DesiredVelocity { get; set; }
        #endregion

        #region Properties
        public bool IsJumping
        {
            get
            {
                return this.isJumping;
            }
        }

        public bool IsCrouching
        {
            get
            {
                return this.isCrouching;
            }
            set
            {
                this.isCrouching = value;
            }
        }
        #endregion

        #region Constructor
        public Character(
            float accelerationRate, 
            float decelerationRate
        ) : base() {
            this.accelerationRate = accelerationRate;
            this.decelerationRate = decelerationRate;
        }
        #endregion

        #region Methods
        public void DoJump(float jumpHeight)
        {
            this.jumpHeight = jumpHeight;
            this.isJumping = true;
        }

        public override void AddExternalForces(float dt)
        {
            ClearForces();

            if (this.isJumping)
            {
                foreach (CollisionInfo info in CollisionSkin.Collisions)
                {
                    Vector3 N = info.DirToBody0;
                    if (this == info.SkinInfo.Skin1.Owner)
                        Vector3.Negate(ref N, out N);

                    if (Vector3.Dot(N, Orientation.Up) > 0.7f)
                    {
                        Vector3 vel = Velocity;
                        vel.Y = jumpHeight;
                        Velocity = vel;
                        break;
                    }
                }
            }

            Vector3 deltaVel = DesiredVelocity - Velocity;
            bool running = true;

            if (DesiredVelocity.LengthSquared() < JiggleMath.Epsilon)
                running = false;
            else
                deltaVel.Normalize();

            deltaVel.Y = -2.0f;

            //Start fast, slow down slower
            if (running)

                //Acceleration multiplier
                deltaVel *= accelerationRate;
            else

                //Deceleration multiplier
                deltaVel *= decelerationRate;

            float forceFactor = 500.0f;
            this.isJumping = false;

            AddBodyForce(deltaVel * Mass * dt * forceFactor);
            AddGravityToExternalForce();
        }
        #endregion
    }
}