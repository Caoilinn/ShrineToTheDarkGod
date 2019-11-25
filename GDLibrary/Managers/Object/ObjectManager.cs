/*
Function: 		Store, update, and draw all visible objects
Author: 		NMCG
Version:		1.1
Date Updated:	
Bugs:			None
Fixes:			None
Mods:           Added support for opaque and semi-transparent rendering
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class ObjectManager : PausableDrawableGameComponent
    {
        #region Fields
        private CameraManager cameraManager;
        private List<DrawnActor3D> removeList, opaqueDrawList, transparentDrawList;
        private RasterizerState rasterizerStateOpaque;
        private RasterizerState rasterizerStateTransparent;
        #endregion

        #region Properties   
        public List<DrawnActor3D> OpaqueDrawList
        {
            get
            {
                return this.opaqueDrawList;
            }
        }
        public List<DrawnActor3D> TransparentDrawList
        {
            get
            {
                return this.transparentDrawList;
            }
        }
        #endregion

        #region Constructors
        //Creates an ObjectManager with lists at initialSize == 20
        public ObjectManager(
            Game game,
            CameraManager cameraManager,
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : this(game, cameraManager, 10, 10, eventDispatcher, statusType) {

        }

        public ObjectManager(
            Game game,
            CameraManager cameraManager,
            int transparentInitialSize,
            int opaqueInitialSize,
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : base(game, statusType, eventDispatcher) {
            this.cameraManager = cameraManager;

            //Create two lists - opaque and transparent
            this.opaqueDrawList = new List<DrawnActor3D>(opaqueInitialSize);
            this.transparentDrawList = new List<DrawnActor3D>(transparentInitialSize);

            //Create list to store objects to be removed at start of each update
            this.removeList = new List<DrawnActor3D>(0);

            //Set up graphic settings
            InitializeGraphics();

            //Register with the event dispatcher for the events of interest
            RegisterForEventHandling(eventDispatcher);
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.OpacityChanged += EventDispatcher_OpacityChanged;
            eventDispatcher.RemoveActorChanged += EventDispatcher_RemoveActorChanged;
            eventDispatcher.AddActorChanged += EventDispatcher_AddActorChanged;
            eventDispatcher.DoorEvent += EventDispatcher_DoorOpen;
            eventDispatcher.EnemyDeathEvent += EventDispacter_EnemyDeath;

            //Dont forget to call the base method to register for OnStart, OnPause events!
            base.RegisterForEventHandling(eventDispatcher);
        }

        private void EventDispacter_EnemyDeath(EventData eventData)
        {
            (eventData.AdditionalParameters[0] as EnemyObject).Remove();
            this.Remove(eventData.AdditionalParameters[0] as EnemyObject);
        }

        private void EventDispatcher_AddActorChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnAddActor)
                if (eventData.Sender is DrawnActor3D actor)
                    this.Add(actor);
        }

        private void EventDispatcher_RemoveActorChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnRemoveActor)
            {
                if (eventData.Sender is DrawnActor3D actor)
                    this.Remove(actor);
                else
                    this.Remove(eventData.AdditionalParameters[0] as DrawnActor3D);
            }
        }

        private void EventDispatcher_OpacityChanged(EventData eventData)
        {
            if (eventData.Sender is DrawnActor3D actor)
            {
                if (eventData.EventType == EventActionType.OnOpaqueToTransparent)
                {
                    //Remove from opaque and add to transparent
                    this.opaqueDrawList.Remove(actor);
                    this.transparentDrawList.Add(actor);
                }
                else if (eventData.EventType == EventActionType.OnTransparentToOpaque)
                {
                    //Remove from transparent and add to opaque
                    this.transparentDrawList.Remove(actor);
                    this.opaqueDrawList.Add(actor);
                }
            }
        }

        private void EventDispatcher_DoorOpen(EventData eventData)
        {
            if(eventData.EventType == EventActionType.OnDoorOpen)
            {
                object[] additionalParameters = eventData.AdditionalParameters;
                string gateID = (string) additionalParameters[0];

                InteractableGate gate = GetGate(gateID);

                if(gate != null)
                {
                    gate.OpenGate();
                    this.opaqueDrawList.Remove(gate);
                }
            }
        }
        #endregion

        #region Methods
        private InteractableGate GetGate(string gateID)
        {
            return this.opaqueDrawList.Find(x => x.ID == gateID) as InteractableGate;
        }

        private void InitializeGraphics()
        {
            //Set the graphics card to repeat the end pixel value for any UV value outside 0-1
            //See http://what-when-how.com/xna-game-studio-4-0-programmingdeveloping-for-windows-phone-7-and-xbox-360/samplerstates-xna-game-studio-4-0-programming/
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Mirror;
            samplerState.AddressV = TextureAddressMode.Mirror;
            Game.GraphicsDevice.SamplerStates[0] = samplerState;

            //Opaque objects
            this.rasterizerStateOpaque = new RasterizerState();
            this.rasterizerStateOpaque.CullMode = CullMode.None;

            //Transparent objects
            this.rasterizerStateTransparent = new RasterizerState();
            this.rasterizerStateTransparent.CullMode = CullMode.None;
        }

        private void SetGraphicsStateObjects(bool isOpaque)
        {
            //Remember this code from our initial aliasing problems with the Sky box?
            //Enable anti-aliasing along the edges of the quad i.e. to remove jagged edges to the primitive
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            if (isOpaque)
            {
                //Set the appropriate state for opaque objects
                Game.GraphicsDevice.RasterizerState = this.rasterizerStateOpaque;

                //Disable to see what happens when we disable depth buffering - look at the boxes
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }

            //Semi-transparent
            else
            {
                //Set the appropriate state for transparent objects
                Game.GraphicsDevice.RasterizerState = this.rasterizerStateTransparent;

                //Enable alpha blending for transparent objects i.e. trees
                Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                //Disable to see what happens when we disable depth buffering - look at the boxes
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }
        }

        public void Add(DrawnActor3D actor)
        {
            if (actor.GetAlpha() == 1)
                this.opaqueDrawList.Add(actor);
            else
                this.transparentDrawList.Add(actor);
        }

        //Call when we want to remove a drawn object from the scene
        public void Remove(DrawnActor3D actor)
        {
            this.removeList.Add(actor);
        }

        public DrawnActor3D Find(Predicate<DrawnActor3D> predicate)
        {
            DrawnActor3D drawnActor = null;

            //Look in opaque
            drawnActor = this.opaqueDrawList.Find(predicate);

            if (drawnActor != null)
                return drawnActor;

            //Look in transparent
            drawnActor = this.transparentDrawList.Find(predicate);

            if (drawnActor != null)
                return drawnActor;

            return null;
        }

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> resultList = new List<DrawnActor3D>();

            //Look in opaque
            resultList.AddRange(this.opaqueDrawList.FindAll(predicate));

            //Look in transparent
            resultList.AddRange(this.transparentDrawList.FindAll(predicate));

            return resultList.Count == 0 ? null : resultList;
        }

        public int Remove(Predicate<DrawnActor3D> predicate)
        {
            List<DrawnActor3D> resultList = null;
            resultList = this.opaqueDrawList.FindAll(predicate);

            //The actor(s) were found in the opaque list
            if ((resultList != null) && (resultList.Count != 0))
            {
                foreach (DrawnActor3D actor in resultList)
                    this.removeList.Add(actor);
            }

            //The actor(s) were found in the transparent list
            else
            {
                resultList = this.transparentDrawList.FindAll(predicate);

                if ((resultList != null) && (resultList.Count != 0))
                    foreach (DrawnActor3D actor in resultList)
                        this.removeList.Add(actor);
            }

            //Returns how many objects will be removed in the next update() call
            return removeList != null ? removeList.Count : 0;
        }

        public void Clear()
        {
            this.opaqueDrawList.Clear();
            this.transparentDrawList.Clear();
            this.removeList.Clear();
        }

        //Batch remove on all objects that were requested to be removed
        protected virtual void ApplyRemove()
        {
            foreach (DrawnActor3D actor in this.removeList)
            {
                if (actor.GetAlpha() == 1)
                    this.opaqueDrawList.Remove(actor);
                else
                    this.transparentDrawList.Remove(actor);
            }

            this.removeList.Clear();
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //Remove any outstanding objects since the last update
            ApplyRemove();

            //Update all your opaque objects
            foreach (DrawnActor3D actor in this.opaqueDrawList)

                //If update flag is set
                if ((actor.GetStatusType() & StatusType.Update) == StatusType.Update)
                    actor.Update(gameTime);

            //Update all your transparent objects
            foreach (DrawnActor3D actor in this.transparentDrawList)
            {
                //If update flag is set
                if ((actor.GetStatusType() & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);

                    //Used to sort objects by distance from the camera so that proper depth representation will be shown
                    MathUtility.SetDistanceFromCamera(actor as Actor3D, this.cameraManager.ActiveCamera);
                }
            }

            //Sort so that the transparent objects closest to the camera are the LAST transparent objects drawn
            SortTransparentByDistance();
            base.ApplyUpdate(gameTime);
        }
    
        private void SortTransparentByDistance()
        {
            //Sorting in descending order
            this.transparentDrawList.Sort((a, b) => (b.Transform.DistanceToCamera.CompareTo(a.Transform.DistanceToCamera)));
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            //Draw the scene for all of the cameras in the cameramanager
            foreach (Camera3D activeCamera in this.cameraManager)
            {
                //Set the viewport dimensions to the size defined by the active camera
                Game.GraphicsDevice.Viewport = activeCamera.Viewport;

                //Set the gfx to render opaque objects
                SetGraphicsStateObjects(true);
                foreach (DrawnActor3D actor in this.opaqueDrawList)
                    DrawActor(gameTime, actor, activeCamera);

                //Set the gfx to render semi-transparent objects
                SetGraphicsStateObjects(false);
                foreach (DrawnActor3D actor in this.transparentDrawList)
                    DrawActor(gameTime, actor, activeCamera);
            }

            base.ApplyDraw(gameTime);
        }

        //Calls the DrawObject() based on underlying object type
        private void DrawActor(GameTime gameTime, DrawnActor3D actor, Camera3D activeCamera)
        {
            //Was the drawn enum value set?
            if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                if (actor is ModelObject)
                    DrawObject(gameTime, actor as ModelObject, activeCamera);

            //Add additional elseif statements here to render other object types (e.g model, animated, billboard etc)
        }

        //Draw a model object 
        private void DrawObject(GameTime gameTime, ModelObject modelObject, Camera3D activeCamera)
        {
            if (modelObject.Model != null)
            {
                modelObject.EffectParameters.SetParameters(activeCamera);
                foreach (ModelMesh mesh in modelObject.Model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = modelObject.EffectParameters.Effect;
                    }

                    modelObject.EffectParameters.SetWorld(modelObject.BoneTransforms[mesh.ParentBone.Index] * modelObject.GetWorldMatrix());
                    mesh.Draw();
                }
            }
        }

        public void OpenDoor(String gateID)
        {
            EventDispatcher.Publish(
                new EventData(
                    EventActionType.OnDoorOpen,
                    EventCategoryType.Door,
                    new object[] { gateID }
                )
            );
        }
        #endregion
    }
}