/*
Function: 		First person COLLIDABLE camera controller.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GDLibrary
{
    //Inner class used for ray picking
    class CollidableObjectPredicate : CollisionSkinPredicate1
    {
        private ActorType actorType;

        public CollidableObjectPredicate(ActorType actorType)
        {
            this.actorType = actorType;
        }

        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (skin0.Owner.ExternalData is Actor3D)
                if ((skin0.Owner.ExternalData as Actor3D).ActorType.Equals(actorType))
                    return true;

            return false;
        }
    }

    /// <summary>
    /// A collidable camera has a body and collision skin from a player object but it has no modeldata or texture
    /// </summary>
    public class CollidableFirstPersonCameraController : FirstPersonCameraController
    {
        #region Fields
        private PlayerObject playerObject;
        private float width;
        private float height;
        private float mass;
        private float jumpHeight;
        private float accelerationRate;
        private float decelerationRate;
        private Vector3 translationOffset;
        private EventDispatcher eventDispatcher;
        #endregion

        #region Properties
        public PlayerObject PlayerObject
        {
            get
            {
                return this.playerObject;
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
                this.width = (value > 0) ? value : 1;
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
                this.height = (value > 0) ? value : 1;
            }
        }

        public float Mass
        {
            get
            {
                return this.mass;
            }
            set
            {
                this.mass = (value > 0) ? value : 1;
            }
        }

        public float JumpHeight
        {
            get
            {
                return this.jumpHeight;
            }
            set
            {
                this.jumpHeight = (value > 0) ? value : 1;
            }
        }

        public float AccelerationRate
        {
            get
            {
                return this.accelerationRate;
            }
            set
            {
                this.accelerationRate = (value != 0) ? value : 1;
            }
        }

        public float DecelerationRate
        {
            get
            {
                return this.decelerationRate;
            }
            set
            {
                this.decelerationRate = (value != 0) ? value : 1;
            }
        }
        #endregion

        #region Constructors 
        //Uses the default PlayerObject as the collidable object for the camera
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            float depth,
            float accelerationRate,
            float decelerationRate,
            float mass,
            float jumpHeight,
            Vector3 translationOffset,
            EventDispatcher eventDispatcher
        ) : this(
            id,
            controllerType,
            moveKeys,
            moveSpeed,
            strafeSpeed,
            rotateSpeed,
            managerParameters,
            movementVector,
            rotationVector,
            parentActor,
            width,
            height,
            depth,
            accelerationRate,
            decelerationRate,
            mass,
            jumpHeight,
            translationOffset,
            null,
            eventDispatcher
        ) {
        }

        //Allows developer to specify the type of collidable object to be used as basis for the camera
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector,
            IActor parentActor,
            float width,
            float height,
            float depth,
            float accelerationRate, 
            float decelerationRate,
            float mass, 
            float jumpHeight, 
            Vector3 translationOffset,
            PlayerObject collidableObject,
            EventDispatcher eventDispatcher
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotateSpeed, managerParameters, movementVector, rotationVector) {
            this.width = width;
            this.height = height;
            this.AccelerationRate = accelerationRate;
            this.DecelerationRate = decelerationRate;
            this.Mass = mass;
            this.JumpHeight = jumpHeight;

            //Allows us to tweak the camera position within the player object 
            this.translationOffset = translationOffset;

            /* Create the collidable player object which comes with a collision skin and position the parentActor (i.e. the camera) inside the player object.
             * notice that we don't pass any effect, model or texture information, since in 1st person perspective we dont want to look from inside a model.
             * Therefore, we wont actually render any drawn object - the effect, texture, model (and also Color) information are unused.
             * 
             * This code allows the user to pass in their own PlayerObject (e.g. HeroPlayerObject) to be used for the collidable object basis for the camera.
             */
            if (collidableObject != null)
            {
                this.playerObject = collidableObject;
            }
            else
            {
                Transform3D transform = (parentActor as Actor3D).Transform;

                float health = 100;
                float attack = 100;
                float defence = 100;

                this.playerObject = new PlayerObject(
                    this.ID + " - Player Object",
                    ActorType.CollidableCamera,
                    transform,
                    null,
                    null,
                    width,
                    height,
                    depth,
                    accelerationRate,
                    decelerationRate,
                    movementVector,
                    rotationVector,
                    moveSpeed,
                    rotateSpeed,
                    this.MoveKeys,
                    translationOffset,
                    this.ManagerParameters.KeyboardManager,
                    jumpHeight,
                    health,
                    attack,
                    defence
                );
            }

            this.playerObject.Enable(true, 1);

            this.eventDispatcher = eventDispatcher;
            RegisterForEventHandling(eventDispatcher);
        }
        #endregion

        #region Event Handling
        public void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher.PlayerChanged += EventDispatcher_PlayerChanged;
            this.eventDispatcher.GameChanged += EventDispatcher_GameChanged;
        }

        private void EventDispatcher_GameChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.PlayerTurn)
            {
            }
        }

        private void EventDispatcher_PlayerChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnWayBlocked)
            {
                Vector3 direction = (Vector3) eventData.AdditionalParameters[0];
                if (!this.blockedPaths.Contains(direction))
                {
                    this.blockedPaths.Add(direction);
                }
            }
        }
        #endregion

        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            /* Notice in the code below that we are NO LONGER simply changing the camera translation value. 
             * Since this is now a collidable camera we, instead, modify the camera position by calling the PlayerObject move methods.
             * 
             * Q. Why do we still use the rotation methods of Transform3D? 
             * A. Rotating the camera doesnt affect CD/CR since the camera is modelled by a player object which has a capsule shape.
             *    A capsule's collision response won't alter as a result of any rotation since its cross-section is spherical across the XZ-plane.
             */

            //If it is not the players turn, return
            if (!StateManager.playerTurn) return;

            if (parentActor != null)
            {
                #region Translation
                //Forward
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[0]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Look * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }

                //Back
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[1]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Look * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Look);
                }

                //Left
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[2]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = -(parentActor.Transform.Right * this.MovementVector);
                    this.Translation = -(gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }

                //Right
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[3]) && !this.InMotion)
                {
                    //Calculate target position, relative to the camera
                    this.TargetPosition = (parentActor.Transform.Right * this.MovementVector);
                    this.Translation = (gameTime.ElapsedGameTime.Milliseconds * this.MoveSpeed * parentActor.Transform.Right);
                }
                #endregion

                #region Rotation
                //Anti-Clockwise
                if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[4]) && !this.InMotion)
                {
                    //Calculate target heading, relative to the camera
                    this.TargetHeading = (parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = (gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }

                //Clockwise
                else if (this.ManagerParameters.KeyboardManager.IsKeyDown(this.MoveKeys[5]) && !this.InMotion)
                {
                    //Calculate target heading, relative to the camera
                    this.TargetHeading = -(parentActor.Transform.Up * this.RotationVector);
                    this.Rotation = -(gameTime.ElapsedGameTime.Milliseconds * this.RotationSpeed * parentActor.Transform.Up);
                }
                #endregion
            }
        }

        public override void HandleMovement(Actor3D parentActor)
        {
            #region Translation
            if (this.Translation != Vector3.Zero)
            {
                //Prevenet the player from walking into a wall
                if (PreventMovement(parentActor)) return;

                //If the current positon is near the target position
                if (Vector3.Distance(this.CurrentPosition, this.TargetPosition) <= 10)
                {
                    //Move to the target position
                    parentActor.Transform.TranslateBy((this.CurrentPosition - this.TargetPosition) * -Vector3.One);
                    
                    //Reset Vectors
                    this.Translation = Vector3.Zero;
                    this.CurrentPosition = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;

                    //Update game state
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.EnemyTurn,
                            EventCategoryType.Game
                        )
                    );

                    //Clear blocked paths
                    this.blockedPaths.Clear();

                    //Clear collision set
                    this.collisionSet.Clear();

                    //Check for collision
                    DetectCollision(parentActor);
                }
                else
                {
                    //Translate actor
                    parentActor.Transform.TranslateBy(this.Translation);

                    //Update current position
                    this.CurrentPosition += this.Translation;

                    //Update motion state
                    this.InMotion = true;
                }
            }
            #endregion

            #region Rotation
            if (this.Rotation != Vector3.Zero)
            {
                //If the user is not already in motion
                if (!this.InMotion)
                {
                    //Publish a sound event
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "turn_around" }
                        )
                    );
                }

                //If the current heading is near the target heading
                if (Vector3.Distance(this.CurrentHeading, this.TargetHeading) <= 5)
                {
                    //Rotate to the target heading
                    parentActor.Transform.RotateBy((this.CurrentHeading - this.TargetHeading) * -Vector3.One);

                    //Reset vectors
                    this.Rotation = Vector3.Zero;
                    this.CurrentHeading = Vector3.Zero;

                    //Update motion state
                    this.InMotion = false;
                }
                else
                {
                    //Rotate actor
                    parentActor.Transform.RotateBy(this.Rotation);

                    //Update current heading
                    this.CurrentHeading += this.Rotation;

                    //Update motion state
                    this.InMotion = true;
                }
            }
            #endregion
        }

        private bool PreventMovement(Actor3D parentActor)
        {
            //If the player is stationary
            if (!this.InMotion)
            {
                //Detect collisions around the player
                DetectCollision(parentActor);

                //If the player is about to walk into a wall
                if (this.blockedPaths.Contains(Vector3.Normalize(this.Translation)))
                {
                    //Reset vector
                    this.Translation = Vector3.Zero;

                    //If this is the firs time that the player has pressed a move key
                    if (this.isFirstTimePressed)
                    {
                        //Publish sound event
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlay,
                                EventCategoryType.Sound2D,
                                new object[] { "wall_bump" }
                            )
                        );

                        //Update key state
                        this.isFirstTimePressed = false;
                    }

                    //If the player has released (or changed) keys
                    if (this.ManagerParameters.KeyboardManager.IsStateChanged())
                    {
                        //Update key state
                        this.isFirstTimePressed = true;
                    }

                    //Prevent the player from moving
                    return true;
                }

                //If the player is free to move
                else
                {
                    //Publish sound event
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "environment_stone_steps" }
                        )
                    );
                }
            }

            //Allow the player to move
            return false;
        }

        #region Collision Detection
        //Setup sets for keeping track of blocked paths and current collisions
        readonly HashSet<Vector3> blockedPaths = new HashSet<Vector3>();
        readonly HashSet<CollidableObject> collisionSet = new HashSet<CollidableObject>();

        //Setup variables for extracting data from the SegmentIntersect() method
        //See CheckForCollisionWithRay() method
        private CollisionSkin skin;
        private float frac;
        private Vector3 pos;
        private Vector3 normal;

        //Setup keystate bool
        private bool isFirstTimePressed = true;

        //Checks for collision around the player (north, south, east & west)
        public void DetectCollision(Actor3D parentActor)
        {
            float length = 254;
            Vector3 position = parentActor.Transform.Translation;

            List<Vector3> directions = new List<Vector3>
            {
                parentActor.Transform.Look,
                -parentActor.Transform.Look,
                parentActor.Transform.Right,
                -parentActor.Transform.Right
            };

            foreach (Vector3 direction in directions)
            {
                CheckForCollisionWithWall(position, direction, length);
                CheckForCollisionWithPickup(position, direction, length);
                CheckForCollisionWithEnemy(position, direction, length);
            }
        }

        //Uses a ray to check for collision with a wall, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithWall(Vector3 position, Vector3 direction, float length)
        {
            //Create a segment ray
            Vector3 start = position;
            Vector3 delta = direction * length;
            Segment seg = new Segment(start, delta);

            //Returns true if segment intersects with a collidable object (that is not the player object)
            CollidableObjectPredicate pred = new CollidableObjectPredicate(ActorType.CollidableArchitecture);

            //Use JigLib's in-built SegmentIntersect() method to check for collision with the ray
            this.ManagerParameters.PhysicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
                out frac,
                out skin,
                out pos,
                out normal,
                seg,
                pred
            );

            //If a collision has taken place
            if (skin != null && skin.Owner != null)
            {
                //Marked path as blocked
                if (!this.blockedPaths.Contains(direction))
                {
                    this.blockedPaths.Add(direction);
                }
            }
        }

        //Uses a ray to check for collision with a pickup, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithPickup(Vector3 position, Vector3 direction, float length)
        {
            //Create a segment ray
            Vector3 start = position;
            Vector3 delta = direction * length;
            Segment seg = new Segment(start, delta);

            //Returns true if segment intersects with a collidable object (that is not the player object)
            CollidableObjectPredicate pred = new CollidableObjectPredicate(ActorType.CollidablePickup);

            //Use JigLib's in-built SegmentIntersect() method to check for collision with the ray
            this.ManagerParameters.PhysicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
                out frac,
                out skin,
                out pos,
                out normal,
                seg,
                pred
            );

            //If a collision has taken place
            if (skin != null && skin.Owner != null)
            {
                //Cast to collidable object
                CollidableObject pickup = skin.Owner.ExternalData as CollidableObject;

                //If the pickup is within the range (adjacent cell)
                if (frac >= 0.5f && frac <= 1.0f)
                {
                    //If the pickup has not yet been realised
                    if (!this.collisionSet.Contains(pickup))
                    {
                        //Add pickup to the collision set
                        this.collisionSet.Add(pickup);

                        //Publish event to play sound effect
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlay,
                                EventCategoryType.Sound2D,
                                new object[] { "item_twinkle" }
                            )
                        );
                    }
                }

                //If the pickup is within the range (current cell)
                if (frac <= 0.5f)
                {
                    //Publish event to remove pickup
                    EventDispatcher.Publish(
                        new EventData(
                            pickup,
                            EventActionType.OnRemoveActor,
                            EventCategoryType.SystemRemove
                        )
                    );

                    //Publish event to add to inventory
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnAddToInventory,
                            EventCategoryType.Pickup,
                            new object[] { pickup }
                        )
                    );

                    //Publish event to update hud
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnUpdateHud,
                            EventCategoryType.Pickup,
                            new object[] { pickup }
                        )
                    );
                }
            }
        }

        //Uses a ray to check for collision with a pickup, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithEnemy(Vector3 position, Vector3 direction, float length)
        {
            //Create a segment ray
            Vector3 start = position;
            Vector3 delta = direction * length;
            Segment seg = new Segment(start, delta);

            //Returns true if segment intersects with a collidable object (that is not the player object)
            CollidableObjectPredicate pred = new CollidableObjectPredicate(ActorType.Enemy);

            //Use JigLib's in-built SegmentIntersect() method to check for collision with the ray
            this.ManagerParameters.PhysicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
                out frac,
                out skin,
                out pos,
                out normal,
                seg,
                pred
            );

            //If a collision has taken place
            if (skin != null && skin.Owner != null)
            {
                //Cast to collidable object
                CollidableObject enemy = skin.Owner.ExternalData as CollidableObject;

                //If the enemy is within the range (adjacent cell)
                if (frac >= 0.0f && frac <= 1.0f)
                {
                    //Marked path as blocked
                    if (!this.blockedPaths.Contains(direction))
                    {
                        this.blockedPaths.Add(direction);
                    }

                    //If the enemy has not yet been realised
                    if (!this.collisionSet.Contains(enemy))
                    {
                        //Add enemy to the collision set
                        this.collisionSet.Add(enemy);

                        //Publish event to play music
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlay,
                                EventCategoryType.Sound2D,
                                new object[] { "battle_theme" }
                            )
                        );

                        //
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnInitiateBattle,
                                EventCategoryType.Combat
                            )
                        );
                    }
                }
            }
        }
        #endregion

        public void CheckSounds()
        {
            if (this.ManagerParameters.SoundManager == null) return;

            bool isEnemyFound = false;
            bool isPickupFound = false;

            foreach (CollidableObject collidableObject in this.collisionSet)
                if (collidableObject.ActorType.Equals(ActorType.Enemy))
                    isEnemyFound = true;
                else if (collidableObject.ActorType.Equals(ActorType.CollidablePickup))
                    isPickupFound = true;

            if (!isEnemyFound)
                this.ManagerParameters.SoundManager.PauseCue("battle_theme");

            if (!isPickupFound)
                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnPause,
                        EventCategoryType.Sound2D,
                        new object[] { "item_twinkle" }
                    )
                );
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
            CheckSounds();
        }
        #endregion
    }
}