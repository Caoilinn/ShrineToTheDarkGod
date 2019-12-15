using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Math;

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

        private ManagerParameters managerParameters;
        private HashSet<Vector3> blockedDirections;
        #endregion

        #region Properties
        public Vector3 Translation
        {
            get {
                return this.translation;
            }
            set {
                this.translation = value;
            }
        }

        public Vector3 Rotation
        {
            get {
                return this.rotation;
            }
            set {
                this.rotation = value;
            }
        }

        public Vector3 TargetPosition
        {
            get {
                return this.targetPosition;
            }
            set {
                this.targetPosition = value;
            }
        }

        public Vector3 CurrentPosition
        {
            get {
                return this.currentPosition;
            }
            set {
                this.currentPosition = value;
            }
        }

        public Vector3 TargetHeading
        {
            get {
                return this.targetHeading;
            }
            set {
                this.targetHeading = value;
            }
        }

        public Vector3 CurrentHeading
        {
            get {
                return this.currentHeading;
            }
            set {
                this.currentHeading = value;
            }
        }

        public Vector3 MovementVector
        {
            get {
                return this.movementVector;
            }
            set {
                this.movementVector = value;
            }
        }

        public Vector3 RotationVector
        {
            get {
                return this.rotationVector;
            }
            set {
                this.rotationVector = value;
            }
        }

        public bool InMotion
        {
            get {
                return this.inMotion;
            }
            set {
                this.inMotion = value;
            }
        }

        public float MoveSpeed
        {
            get {
                return this.moveSpeed;
            }
            set {
                this.moveSpeed = value;
            }
        }

        public float RotateSpeed
        {
            get {
                return this.rotateSpeed;
            }
            set {
                this.rotateSpeed = value;
            }
        }

        public float Health
        {
            get {
                return this.health;
            }
            set {
                this.health = value;
            }
        }

        public float Attack
        {
            get {
                return this.attack;
            }
            set {
                this.attack += value;
            }
        }

        public float Defence
        {
            get {
                return this.defence;
            }
            set {
                this.defence = value;
            }
        }

        public HashSet<Vector3> BlockedDirections
        {
            get {
                return this.blockedDirections;
            }
            set {
                this.blockedDirections = value;
            }
        }

        public ManagerParameters ManagerParameters
        {
            get {
                return this.managerParameters;
            }
            set {
                this.managerParameters = value;
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
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            ManagerParameters managerParameters
        ) : base(id, actorType, transform, effectParameters, model) {
            this.MovementVector = movementVector;
            this.RotationVector = rotationVector;
            this.MoveSpeed = moveSpeed;
            this.RotateSpeed = rotateSpeed;

            this.Health = health;
            this.Attack = attack;
            this.Defence = defence;

            this.ManagerParameters = managerParameters;
            this.BlockedDirections = new HashSet<Vector3>();
        }
        #endregion

        #region Methods
        public virtual void HandleMovement()
        {
            #region Rotation
            if (this.Rotation != Vector3.Zero)
            {
                //Create audio emitter
                AudioEmitter characterAudioEmitter = new AudioEmitter {
                    Position = this.Transform.Translation
                };

                //Play turn sound
                if (!this.InMotion) EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "player_turn", characterAudioEmitter }));
                
                //If the current heading is near the target heading
                if (Vector3.Distance(this.CurrentHeading, this.TargetHeading) <= 5)
                {
                    //Rotate to the target heading
                    this.Transform.RotateBy((this.CurrentHeading - this.TargetHeading) * -Vector3.One);

                    //Reset vectors
                    this.Rotation = Vector3.Zero;
                    this.CurrentHeading = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;
                } else
                {
                    //Rotate actor
                    this.Transform.RotateBy(this.Rotation);

                    //Update current heading
                    this.CurrentHeading += this.Rotation;

                    //Update motion state
                    this.InMotion = true;
                }

                //Prevents multiple movement actions from happening every update
                return;
            }
            #endregion

            #region Translation
            if (this.Translation != Vector3.Zero)
            {
                //Create audio emitter
                AudioEmitter audioEmitter = new AudioEmitter {
                    Position = (this.Transform.Translation + this.TargetPosition)
                };

                //Prevent movement while in combat
                if (StateManager.InCombat && !StateManager.Dodged)
                {
                    //Display info
                    EventDispatcher.Publish(new EventData(EventActionType.OnDisplayInfo, EventCategoryType.Textbox, new object[] { "Cannot move while in combat" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "player_bump_001", audioEmitter }));
                    this.Translation = Vector3.Zero;
                    return;
                }

                //If the direction of movement is blocked
                if (this.BlockedDirections.Contains(Vector3.Normalize(this.Translation)))
                {
                    //Display info
                    EventDispatcher.Publish(new EventData(EventActionType.OnDisplayInfo, EventCategoryType.Textbox, new object[] { "Can't go that way!" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "player_bump_001", audioEmitter }));
                    this.Translation = Vector3.Zero;
                    return;
                }

                //Play move sound
                if (!this.InMotion) EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "player_footsteps", audioEmitter }));

                //If the current positon is near the target position
                if (Vector3.Distance(this.CurrentPosition, this.TargetPosition) <= 10)
                {
                    //Move to the target position
                    this.Transform.TranslateBy((this.CurrentPosition - this.TargetPosition) * -Vector3.One);

                    //Reset Vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Update game state
                    this.UpdateGameState();

                    //Update motion state
                    this.InMotion = false;

                    //Reset dodge
                    StateManager.Dodged = false;
                }
                else
                {
                    //Translate actor
                    this.Transform.TranslateBy(this.Translation);

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Update motion state
                    this.InMotion = true;
                }

                //Prevents multiple movement actions from happening every update
                return;
            }
            #endregion
        }

        private void UpdateGameState()
        {
            #region Enemy Turn
            if (this.ActorType.Equals(ActorType.Player))
            {
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
                return;
            }
            #endregion

            #region Player Turn
            if (this.ActorType.Equals(ActorType.Enemy))
            {
                EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                return;
            }
            #endregion
        }

        public virtual void ResetCollision()
        {
            this.BlockedDirections.Clear();
            this.UpdateCollision();
        }

        //Update Collision
        public virtual void UpdateCollision()
        {
            //Set up origin
            Vector3 position = this.Transform.Translation;

            //Set up cast directions
            Vector3 north = this.Transform.Look;
            Vector3 south = -this.Transform.Look;
            Vector3 east = this.Transform.Right;
            Vector3 west = -this.Transform.Right;

            //Set up ray length
            float length = 254;

            //Detect collision north of the character
            if (DetectCollision(position, north, length))
                this.blockedDirections.Add(north);

            //Detect collision south of the character
            if (DetectCollision(position, south, length))
                this.blockedDirections.Add(south);

            //Detect collision east of the character
            if (DetectCollision(position, east, length))
                this.blockedDirections.Add(east);

            //Detect collision west of the character
            if (DetectCollision(position, west, length))
                this.blockedDirections.Add(west);
        }

        //Detect Collision
        public virtual bool DetectCollision(Vector3 position, Vector3 direction, float length)
        {
            //Create a segment ray
            Vector3 start = position;
            Vector3 delta = direction * length;
            Segment seg = new Segment(start, delta);

            //Returns true if segment intersects with collidable architecture
            ImpassableObjectPredicate pred = new ImpassableObjectPredicate();

            //Use JigLib's in-built SegmentIntersect() method to check for collision with the ray
            this.managerParameters.PhysicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
                out float frac,
                out CollisionSkin skin,
                out Vector3 pos,
                out Vector3 normal,
                seg,
                pred
            );

            //Returns true if the ray has collided with a wall
            return (skin != null);
        }

        public virtual void TakeTurn(GameTime gameTime)
        {
        }

        public virtual void TakeDamage(float damage)
        {
            this.Health -= damage;
        }

        public override Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(this.Transform.Scale)
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

        public override void Update(GameTime gameTime)
        {
            HandleMovement();
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

    class ImpassableObjectPredicate : CollisionSkinPredicate1
    {
        #region Methods
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            //If the collision skin has not yet been removed
            if ((skin0.Owner.ExternalData as CollidableObject).Body != null)

                //Returns true if the actor is a piece of collidable architecture, or a gate
                if ((skin0.Owner.ExternalData as Actor3D).ActorType.Equals(ActorType.CollidableArchitecture) || (skin0.Owner.ExternalData as Actor3D).ActorType.Equals(ActorType.Gate))

                    //Return true
                    return true;

            //Return false
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
            get {
                return this.isJumping;
            }
        }

        public bool IsCrouching
        {
            get {
                return this.isCrouching;
            }
            set {
                this.isCrouching = value;
            }
        }
        #endregion

        #region Constructor
        public Character(
            float accelerationRate,
            float decelerationRate
        ) : base()
        {
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