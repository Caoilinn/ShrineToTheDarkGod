/*
Function: 		Enables CDCR through JibLibX by integrating forces applied to each collidable object within the scene
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			
Fixes:			None
*/

using JigLibX.Physics;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class PhysicsManager : PausableGameComponent
    {
        #region Fields
        private PhysicsSystem physicSystem;
        private PhysicsController physCont;
        private float timeStep = 0;
        private List<CollidableObject> removeList;
        #endregion

        #region Properties
        public PhysicsSystem PhysicsSystem
        {
            get
            {
                return physicSystem;
            }
        }

        public PhysicsController PhysicsController
        {
            get
            {
                return physCont;
            }
        }
        #endregion

        #region Constructors
        //Pre-defined gravity
        public PhysicsManager(
            Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : this(game, eventDispatcher, statusType, -10 * Vector3.UnitY) {

        }

        //User-defined gravity
        public PhysicsManager(
            Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType, 
            Vector3 gravity
        ) : base(game, eventDispatcher, statusType) {
            this.physicSystem = new PhysicsSystem();

            //Add cd/cr system
            this.physicSystem.CollisionSystem = new CollisionSystemSAP();

            //Allows us to define the direction and magnitude of gravity - default is (0, -9.8f, 0)
            this.physicSystem.Gravity = gravity;

            //Prevents bug where objects would show correct CDCR response when velocity == Vector3.Zero
            this.physicSystem.EnableFreezing = false;

            this.physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            this.physicSystem.CollisionSystem.UseSweepTests = true;

            //Affect accuracy and the overhead == time required
            this.physicSystem.NumCollisionIterations = 8; //8
            this.physicSystem.NumContactIterations = 8; //8
            this.physicSystem.NumPenetrationRelaxtionTimesteps = 12; //15          
            
            //Affect accuracy of the collision detection
            this.physicSystem.AllowedPenetration = 0.000025f;
            this.physicSystem.CollisionTollerance = 0.00005f;

            this.physCont = new PhysicsController();
            this.physicSystem.AddController(physCont);

            //Batch removal - as in ObjectManager
            this.removeList = new List<CollidableObject>();
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.RemoveActorChanged += EventDispatcher_RemoveActorChanged;
            base.RegisterForEventHandling(eventDispatcher);
        }

        private void EventDispatcher_RemoveActorChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnRemoveActor)
            {
                //Using the "sender" property of the event to pass reference to object to be removed - use "as" to access Body since sender is defined as a raw object.
                Remove(eventData.Sender as CollidableObject);
            }
        }

        //See MenuManager::EventDispatcher_MenuChanged to see how it does the reverse i.e. they are mutually exclusive
        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Did the event come from the main menu and is it a start game event
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Turn on update and draw i.e. hide the menu
                this.StatusType = StatusType.Update | StatusType.Drawn;
            }

            //Did the event come from the main menu and is it a pause game event
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Turn off update and draw i.e. show the menu since the game is paused
                this.StatusType = StatusType.Off;
            }
        }
        #endregion

        #region Methods
        //Call when we want to remove a drawn object from the scene
        public void Remove(CollidableObject collidableObject)
        {
            this.removeList.Add(collidableObject);
        }

        //Batch remove on all objects that were requested to be removed
        protected virtual void ApplyRemove()
        {
            foreach (CollidableObject collidableObject in this.removeList)
            {
                //What would happen if we did not remove the physics body? would the CD/CR skin remain?
                this.PhysicsSystem.RemoveBody(collidableObject.Body);
            }

            this.removeList.Clear();
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            ApplyRemove();

            timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

            //If the time between updates indicates a FPS of close to 60 fps or less then update CD/CR engine
            if (timeStep < 1.0f / 60.0f)
                physicSystem.Integrate(timeStep);
            else
                //Else fix at 60 updates per second
                physicSystem.Integrate(1.0f / 60.0f);
        }
        #endregion
    }
}