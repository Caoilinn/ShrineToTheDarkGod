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
        private float radius;
        private float height;
        private float mass;
        private float jumpHeight;
        private float accelerationRate;
        private float decelerationRate;
        private Vector3 translationOffset;
        private EventDispatcher eventDispatcher;

        //Consts for collision range calculations
        private const float MIN_CURRENT_CELL = 0.0f;
        private const float MAX_CURRENT_CELL = 0.5f;
        private const float MIN_ADJACENT_CELL = 0.5f;
        private const float MAX_ADJACENT_CELL = 1.0f;
        #endregion

        #region Properties
        public PlayerObject PlayerObject
        {
            get
            {
                return this.playerObject;
            }
        }

        public float Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = (value > 0) ? value : 1;
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
            float radius,
            float height,
            float accelerationRate,
            float decelerationRate,
            float health,
            float attack,
            float defence,
            float mass,
            float jumpHeight,
            Vector3 translationOffset
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
            translationOffset,
            null,
            health, 
            attack,
            defence,
            mass,
            radius,
            height,
            accelerationRate,
            decelerationRate,
            jumpHeight
        ) {
        }
        
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
            Vector3 translationOffset,
            PlayerObject collidableObject,
            float health,
            float attack,
            float defence,
            float mass,
            float radius,
            float height,
            float accelerationRate,
            float decelerationRate,
            float jumpHeight
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotateSpeed, managerParameters, movementVector, rotationVector) {
            this.radius = radius;
            this.height = height;
            this.AccelerationRate = accelerationRate;
            this.DecelerationRate = decelerationRate;
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

                this.playerObject = new PlayerObject(
                    this.ID + " - Player Object",
                    ActorType.CollidableCamera,
                    transform,
                    null,
                    null,
                    radius,
                    height,
                    accelerationRate,
                    decelerationRate,
                    movementVector,
                    rotationVector,
                    moveSpeed,
                    rotateSpeed,
                    health,
                    attack,
                    defence,
                    moveKeys,
                    translationOffset,
                    this.ManagerParameters.KeyboardManager,
                    this.ManagerParameters,
                    jumpHeight
                );
            }

            this.playerObject.Enable(true, 1);
        }
        #endregion
        
        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        { 
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

            //Take turns!
            if (!StateManager.playerTurn)
            {
                this.Translation = Vector3.Zero;
                this.Rotation = Vector3.Zero;
                return;
            }
            
            #region Translation
            if (this.Translation != Vector3.Zero)
            {    
                //Finish the fight!
                if (CombatManager.inCombat)
                {
                    this.Translation = Vector3.Zero;
                    return;
                }

                this.blockedPaths.Clear();

                //Prevent the player from walking into a wall
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


                    this.collisionSet.Clear();

                    //Check for collision around the players new position
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
                //Detect collision around the player
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

            //Create a list of directions - n, s, e, w
            List<Vector3> directions = new List<Vector3>
            {
                parentActor.Transform.Look,
                -parentActor.Transform.Look,
                parentActor.Transform.Right,
                -parentActor.Transform.Right
            };

            //Checks for collision around the player
            foreach (Vector3 direction in directions)
            {
                CheckForCollisionWithWall(position, direction, length);
                CheckForCollisionWithPickup(position, direction, length);
                CheckForCollisionWithEnemy(position, direction, length);
                CheckForCollisionWithTrigger(position, direction, length);
                CheckForCollisionWithGate(position, direction, length);
            }
        }

        public object[] CheckForCollision(Vector3 position, Vector3 direction, float length, ActorType actorType)
        {
            //Create a segment ray
            Vector3 start = position;
            Vector3 delta = direction * length;
            Segment seg = new Segment(start, delta);
            Vector3 x = seg.GetEnd();

            //Returns true if segment intersects with a collidable object (that is not the player object)
            CollidableObjectPredicate pred = new CollidableObjectPredicate(actorType);

            //Use JigLib's in-built SegmentIntersect() method to check for collision with the ray
            this.ManagerParameters.PhysicsManager.PhysicsSystem.CollisionSystem.SegmentIntersect(
                out frac,
                out skin,
                out pos,
                out normal,
                seg,
                pred
            );

            //Return an array of collision info
            return new object[] { frac, skin, pos, normal };
        }

        //Uses a ray to check for collision with a wall, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithWall(Vector3 position, Vector3 direction, float length)
        {
            //Check for collision
            object[] collisionInfo = CheckForCollision(position, direction, length, ActorType.CollidableArchitecture);

            //If a collision has taken place
            if (collisionInfo[1] != null && (collisionInfo[1] as CollisionSkin).Owner != null)
            
                #region Current Cell
                //If the collision takes place in the current cell
                if ((float) collisionInfo[0] >= MIN_CURRENT_CELL && (float) collisionInfo[0] <= MAX_CURRENT_CELL)
                {   
                    //If we have not yet marked this path as blocked
                    if (!this.blockedPaths.Contains(direction))

                        //Mark the path as blocked
                        this.blockedPaths.Add(direction);
                }
                #endregion
        }

        //Uses a ray to check for collision with a pickup, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithPickup(Vector3 position, Vector3 direction, float length)
        {
            //Check for collision
            object[] collisionInfo = CheckForCollision(position, direction, length, ActorType.CollidablePickup);

            //If a collision has taken place
            if (collisionInfo[1] != null && (collisionInfo[1] as CollisionSkin).Owner != null)
            {
                //Cast the collision object to a collidable object
                CollidableObject pickup = (collisionInfo[1] as CollisionSkin).Owner.ExternalData as CollidableObject;

                if (pickup.Transform == null) return;

                //If a collision has already taken place with this object
                if (this.collisionSet.Contains(pickup))

                    //Return
                    return;

                //Otherwise, add pickup to the collision set
                this.collisionSet.Add(pickup);

                #region Current Cell
                //If the collision takes place in the current cell
                if ((float) collisionInfo[0] >= MIN_CURRENT_CELL && (float) collisionInfo[0] <= MAX_CURRENT_CELL)
                {
                    //Remove pickup from the collision set
                    this.collisionSet.Remove(pickup);

                    //Remove pickup skin
                    pickup.Remove();
                    this.ManagerParameters.ObjectManager.Remove(pickup);

                    string sound = "";
                    if ((pickup as ImmovablePickupObject).PickupParameters.PickupType == PickupType.Sword)
                    {
                        sound = "equip_sword";
                    }
                    else if ((pickup as ImmovablePickupObject).PickupParameters.PickupType == PickupType.Key)
                    {
                        sound = "equip_sword";
                    }
                    else if ((pickup as ImmovablePickupObject).PickupParameters.PickupType == PickupType.Health)
                    {
                        sound = "drink_potion";
                    }

                    //Publish an event to play a sound once a pickup is encountered
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { sound }
                        )
                    );

                    //Publish an event to add the pickup to inventory
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnItemAdded,
                            EventCategoryType.Inventory,
                            new object[] { pickup }
                        )
                    );

                    //Publish an event to update the hud
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnUpdateHud,
                            EventCategoryType.Pickup,
                            new object[] { pickup }
                        )
                    );
                }
                #endregion

                #region Adjacent Cell
                //If the collision takes place in an adjacent cell
                if ((float) collisionInfo[0] >= MIN_ADJACENT_CELL && (float) collisionInfo[0] <= MAX_ADJACENT_CELL)
                {
                    //Publish event to play sound effect
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "item_twinkle" }
                        )
                    );
                }
                #endregion
            }
        }

        //Uses a ray to check for collision with an enemy, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithEnemy(Vector3 position, Vector3 direction, float length)
        {
            //Check for collision
            object[] collisionInfo = CheckForCollision(position, direction, length, ActorType.Enemy);

            //If a collision has taken place
            if (collisionInfo[1] != null && (collisionInfo[1] as CollisionSkin).Owner != null)
            {
                //Cast the collision object to a collidable object
                CollidableObject enemy = (collisionInfo[1] as CollisionSkin).Owner.ExternalData as CollidableObject;

                if (enemy.Transform == null) return;

                //If a collision has already taken place with this object
                if (this.collisionSet.Contains(enemy))

                    //Return
                    return;

                //Otherwise, add pickup to the collision set
                this.collisionSet.Add(enemy);

                #region Adjacent Cell
                //If the collision takes place in this cell, or in an adjacent cell
                if ((float) collisionInfo[0] >= MIN_ADJACENT_CELL && (float) collisionInfo[0] <= MAX_ADJACENT_CELL)
                {
                    //If we have not yet marked this path as blocked
                    if (!this.blockedPaths.Contains(direction))

                        //Mark the path as blocked
                        this.blockedPaths.Add(direction);

                    //Publish an event to play battle music
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "battle_theme" }
                        )
                    );

                    //Publish an event to initiate battle
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnInitiateBattle,
                            EventCategoryType.Combat,
                            new object[] { enemy }
                        )
                    );
                }
                #endregion
            }
        }

        //Uses a ray to check for collision with a gate, given a starting position, a cast direction, and a ray length
        public void CheckForCollisionWithGate(Vector3 position, Vector3 direction, float length)
        {
            //Check for collision
            object[] collisionInfo = CheckForCollision(position, direction, length, ActorType.Gate);

            //If a collision has taken place
            if (collisionInfo[1] != null && (collisionInfo[1] as CollisionSkin).Owner != null)
            {
                //Cast the collision object to a collidable object
                CollidableObject gate = (collisionInfo[1] as CollisionSkin).Owner.ExternalData as CollidableObject;

                if (gate.Transform == null) return;

                #region Adjacent Cell
                //If the collision takes place in an adjacent cell
                if ((float) collisionInfo[0] >= MIN_CURRENT_CELL && (float) collisionInfo[0] <= MAX_ADJACENT_CELL)
                {
                    //If we have not yet marked this path as blocked
                    if (!this.blockedPaths.Contains(direction))

                        //Mark the path as blocked
                        this.blockedPaths.Add(direction);

                    if (this.ManagerParameters.InventoryManager.HasItem("Gate Key"))
                    {
                        //Remove Gate
                        gate.Remove();
                        this.ManagerParameters.ObjectManager.Remove(gate);
                        this.blockedPaths.Remove(direction);

                        //Remove key from inventory
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnItemRemoved,
                                EventCategoryType.Inventory,
                                new object[] { this.ManagerParameters.InventoryManager.GetItemByDescription("Gate Key") }
                            )
                        );

                        //Publish an event to play a sound once the gate is opened
                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlay,
                                EventCategoryType.Sound2D,
                                new object[] { "gate_open" }
                            )
                        );
                    }
                }
                #endregion
            }
        }
        bool gameWon = false;
        public void CheckForCollisionWithTrigger(Vector3 position, Vector3 direction, float length)
        {
            if (gameWon) return;

            //Check for collision
            object[] collisionInfo = CheckForCollision(position, direction, length, ActorType.Trigger);

            //If a collision has taken place
            if (collisionInfo[1] != null && (collisionInfo[1] as CollisionSkin).Owner != null)
            {
                //Cast the collision object to a collidable object
                CollidableObject trigger = (collisionInfo[1] as CollisionSkin).Owner.ExternalData as CollidableObject;

                #region Current Cell
                //If the collision takes place in the current cell
                if ((float) collisionInfo[0] >= MIN_CURRENT_CELL && (float) collisionInfo[0] <= MAX_CURRENT_CELL)
                {
                    //Publish an event to play music
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "battle_theme" }
                        )
                    );

                    gameWon = true;

                    //End the players turn
                    StateManager.playerTurn = false;
                    
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                }
                #endregion
            }
        }
        #endregion

        public void CheckSounds(Actor3D actor)
        {
            if (this.ManagerParameters.SoundManager == null) return;

            bool isPickupFound = false;
            foreach (CollidableObject collidableObject in this.collisionSet)
                if (collidableObject.ActorType.Equals(ActorType.CollidablePickup))
                    isPickupFound = true;
            
            if (!isPickupFound)
                this.ManagerParameters.SoundManager.PauseCue("item_twinkle");
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            CheckSounds(actor as Actor3D);
            base.Update(gameTime, actor);
        }
        #endregion
    }
}