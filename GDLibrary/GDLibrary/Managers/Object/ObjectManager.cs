///*
//Function: 		Store, update, and draw all visible objects
//Author: 		NMCG
//Version:		1.1
//Date Updated:	
//Bugs:			None
//Fixes:			None
//Mods:           Added support for opaque and semi-transparent rendering
//*/

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;

//namespace GDLibrary
//{
//    public class ObjectManager : PausableDrawableGameComponent
//    {
//        #region Fields
//        private Game game;
//        private CameraManager cameraManager;
//        private List<Actor3D> removeList;
//        private List<Actor3D> opaqueDrawList;
//        private List<Actor3D> transparentDrawList;
//        private RasterizerState rasterizerStateOpaque;
//        private RasterizerState rasterizerStateTransparent;
//        private Viewport fullViewport;
//        #endregion

//        #region Properties   
//        public List<Actor3D> OpaqueDrawList
//        {
//            get
//            {
//                return this.opaqueDrawList;
//            }
//        }

//        public List<Actor3D> TransparentDrawList
//        {
//            get
//            {
//                return this.transparentDrawList;
//            }
//        }
//        #endregion

//        #region Constructors
//        //Creates an ObjectManager with lists at initialSize == 20
//        public ObjectManager(
//            Game game,
//            CameraManager cameraManager,
//            EventDispatcher eventDispatcher,
//            StatusType statusType,
//            Viewport fullViewport
//        ) : this(game, cameraManager, 10, 10, eventDispatcher, statusType) {
//            this.fullViewport = fullViewport;
//        }

//        public ObjectManager(
//            Game game,
//            CameraManager cameraManager,
//            int transparentInitialSize,
//            int opaqueInitialSize,
//            EventDispatcher eventDispatcher,
//            StatusType statusType
//        ) : base(game, statusType, eventDispatcher) {
//            this.game = game;
//            this.cameraManager = cameraManager;

//            //Create two lists - opaque and transparent
//            this.opaqueDrawList = new List<Actor3D>(opaqueInitialSize);
//            this.transparentDrawList = new List<Actor3D>(transparentInitialSize);

//            //Create list to store objects to be removed at start of each update
//            this.removeList = new List<Actor3D>(0);

//            //Set up graphic settings
//            InitializeGraphics();

//            //Register with the event dispatcher for the events of interest
//            RegisterForEventHandling(eventDispatcher);
//        }

//        //public ObjectManager(Game game, CameraManager cameraManager, EventDispatcher eventDispatcher, int initialSize)
//        //{
//        //    this.game = game;
//        //    this.cameraManager = cameraManager;

//        //    //create two lists - opaque and transparent
//        //    this.opaqueDrawList = new List<Actor3D>(initialSize);
//        //    this.transparentDrawList = new List<Actor3D>(initialSize);
//        //    //create list to store objects to be removed at start of each update
//        //    this.removeList = new List<Actor3D>(initialSize);

//        //    //set up graphic settings
//        //    InitializeGraphics();

//        //    //register with the event dispatcher for the events of interest
//        //    RegisterForEventHandling(eventDispatcher);
//        //}
//        #endregion

//        #region Event Handling
//        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher) {
//            eventDispatcher.OpacityChanged += EventDispatcher_OpacityChanged;
//            eventDispatcher.RemoveActorChanged += EventDispatcher_RemoveActorChanged;
//            eventDispatcher.AddActorChanged += EventDispatcher_AddActorChanged;
//            eventDispatcher.EnemyDeathEvent += EventDispacter_EnemyDeath;
//            base.RegisterForEventHandling(eventDispatcher);
//        }

//        private void EventDispacter_EnemyDeath(EventData eventData) {
//            (eventData.AdditionalParameters[0] as EnemyObject).Remove();
//            this.Remove(eventData.AdditionalParameters[0] as EnemyObject);
//        }

//        private void EventDispatcher_AddActorChanged(EventData eventData) {
//            if (eventData.EventType == EventActionType.OnAddActor)
//                if (eventData.Sender is DrawnActor3D actor)
//                    this.Add(actor);
//        }

//        private void EventDispatcher_RemoveActorChanged(EventData eventData) {
//            if (eventData.EventType == EventActionType.OnRemoveActor) {
//                if (eventData.Sender is DrawnActor3D actor)
//                    this.Remove(actor);
//                else
//                    this.Remove(eventData.AdditionalParameters[0] as DrawnActor3D);
//            }
//        }

//        private void EventDispatcher_OpacityChanged(EventData eventData) {
//            if (eventData.Sender is DrawnActor3D actor) {
//                if (eventData.EventType == EventActionType.OnOpaqueToTransparent) {
//                    this.opaqueDrawList.Remove(actor);
//                    this.transparentDrawList.Add(actor);
//                } else if (eventData.EventType == EventActionType.OnTransparentToOpaque) {
//                    this.transparentDrawList.Remove(actor);
//                    this.opaqueDrawList.Add(actor);
//                }
//            }
//        }
//        #endregion

//        #region Methods
//        private void InitializeGraphics()
//        {
//            //Set the graphics card to repeat the end pixel value for any UV value outside 0-1
//            //See http://what-when-how.com/xna-game-studio-4-0-programmingdeveloping-for-windows-phone-7-and-xbox-360/samplerstates-xna-game-studio-4-0-programming/
//            SamplerState samplerState = new SamplerState {
//                AddressU = TextureAddressMode.Mirror,
//                AddressV = TextureAddressMode.Mirror
//            };

//            game.GraphicsDevice.SamplerStates[0] = samplerState;

//            //Opaque objects
//            this.rasterizerStateOpaque = new RasterizerState {
//                CullMode = CullMode.CullCounterClockwiseFace
//            };

//            //Transparent objects
//            this.rasterizerStateTransparent = new RasterizerState {
//                CullMode = CullMode.None
//            };
//        }

//        private void SetGraphicsStateObjects(bool isOpaque)
//        {
//            //Remember this code from our initial aliasing problems with the Sky box?
//            //enable anti-aliasing along the edges of the quad i.e. to remove jagged edges to the primitive
//            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

//            if (isOpaque)
//            {
//                //set the appropriate state for opaque objects
//                game.GraphicsDevice.RasterizerState = this.rasterizerStateOpaque;

//                //disable to see what happens when we disable depth buffering - look at the boxes
//                game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
//            }
//            else
//            {
//                //set the appropriate state for transparent objects
//                game.GraphicsDevice.RasterizerState = this.rasterizerStateTransparent;

//                //enable alpha blending for transparent objects i.e. trees
//                game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

//                //disable to see what happens when we disable depth buffering - look at the boxes
//                game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
//            }
//        }

//        public void Add(DrawnActor3D actor)
//        {
//            if (actor.GetAlpha() == 1)
//                this.opaqueDrawList.Add(actor);
//            else
//                this.transparentDrawList.Add(actor);
//        }

//        //Call when we want to remove a drawn object from the scene
//        public void Remove(DrawnActor3D actor)
//        {
//            this.removeList.Add(actor);
//        }

//        public Actor3D Find(Predicate<Actor3D> predicate)
//        {
//            Actor3D drawnActor = null;

//            //Look in opaque
//            drawnActor = this.opaqueDrawList.Find(predicate);

//            if (drawnActor != null)
//                return drawnActor;

//            //Look in transparent
//            drawnActor = this.transparentDrawList.Find(predicate);

//            if (drawnActor != null)
//                return drawnActor;

//            return null;
//        }

//        public List<Actor3D> FindAll(Predicate<Actor3D> predicate)
//        {
//            List<Actor3D> resultList = new List<Actor3D>();

//            //Look in opaque
//            resultList.AddRange(this.opaqueDrawList.FindAll(predicate));

//            //Look in transparent
//            resultList.AddRange(this.transparentDrawList.FindAll(predicate));

//            return resultList.Count == 0 ? null : resultList;
//        }

//        public int Remove(Predicate<Actor3D> predicate)
//        {
//            List<Actor3D> resultList = null;
//            resultList = this.opaqueDrawList.FindAll(predicate);

//            //The actor(s) were found in the opaque list
//            if ((resultList != null) && (resultList.Count != 0))
//            {
//                foreach (Actor3D actor in resultList)
//                    this.removeList.Add(actor);
//            }

//            //The actor(s) were found in the transparent list
//            else
//            {
//                resultList = this.transparentDrawList.FindAll(predicate);

//                if ((resultList != null) && (resultList.Count != 0))
//                    foreach (DrawnActor3D actor in resultList)
//                        this.removeList.Add(actor);
//            }

//            //Returns how many objects will be removed in the next update() call
//            return removeList != null ? removeList.Count : 0;
//        }

//        public void Clear()
//        {
//            this.opaqueDrawList.Clear();
//            this.transparentDrawList.Clear();
//            this.removeList.Clear();
//        }

//        //Batch remove on all objects that were requested to be removed
//        protected virtual void ApplyRemove()
//        {
//            foreach (DrawnActor3D actor in this.removeList)
//            {
//                if (actor.GetAlpha() == 1)
//                    this.opaqueDrawList.Remove(actor);
//                else
//                    this.transparentDrawList.Remove(actor);
//            }

//            this.removeList.Clear();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            //remove any outstanding objects since the last update
//            ApplyRemove();

//            //update all your opaque objects
//            foreach (Actor3D actor in this.opaqueDrawList)
//            {
//                if ((actor.GetStatusType() & StatusType.Update) == StatusType.Update) //if update flag is set
//                    actor.Update(gameTime);
//            }

//            //update all your transparent objects
//            foreach (Actor3D actor in this.transparentDrawList)
//            {
//                if ((actor.GetStatusType() & StatusType.Update) == StatusType.Update) //if update flag is set
//                {
//                    actor.Update(gameTime);
//                    //used to sort objects by distance from the camera so that proper depth representation will be shown
//                    MathUtility.SetDistanceFromCamera(actor as Actor3D, this.cameraManager.ActiveCamera);
//                }
//            }

//            //sort so that the transparent objects closest to the camera are the LAST transparent objects drawn
//            SortTransparentByDistance();
//        }

//        private void SortTransparentByDistance()
//        {
//            //Sorting in descending order
//            this.transparentDrawList.Sort((a, b) => (b.Transform.DistanceToCamera.CompareTo(a.Transform.DistanceToCamera)));
//        }

//        public void Draw(GameTime gameTime, Camera3D activeCamera)
//        {
//            //modify Draw() method to pass in the currently active camera - used to support multiple camera viewports - see ScreenManager::Draw()
//            //set the viewport dimensions to the size defined by the active camera
//            this.game.GraphicsDevice.Viewport = activeCamera.Viewport;

//            SetGraphicsStateObjects(true);
//            foreach (Actor3D actor in this.opaqueDrawList)
//            {
//                DrawByType(gameTime, actor as Actor3D, activeCamera);
//            }

//            SetGraphicsStateObjects(false);
//            foreach (Actor3D actor in this.transparentDrawList)
//            {
//                DrawByType(gameTime, actor as Actor3D, activeCamera);
//            }
//        }

//        //Calls the correct DrawObject() based on underlying object type
//        private void DrawByType(GameTime gameTime, Actor3D actor, Camera3D activeCamera)
//        {
//            //was the drawn enum value set?
//            if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
//            {
//                if (actor is AnimatedPlayerObject)
//                {
//                    DrawObject(gameTime, actor as AnimatedPlayerObject, activeCamera);
//                }
//                else if (actor is ModelObject)
//                {
//                    DrawObject(gameTime, actor as ModelObject, activeCamera);
//                }
//                else if (actor is BillboardPrimitiveObject)
//                {
//                    DrawObject(gameTime, actor as BillboardPrimitiveObject, activeCamera);
//                }
//                else if (actor is PrimitiveObject)
//                {
//                    DrawObject(gameTime, actor as PrimitiveObject, activeCamera);
//                }
//            }
//        }

//        //Draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
//        private void DrawObject(GameTime gameTime, PrimitiveObject primitiveObject, Camera3D activeCamera)
//        {
//            if (activeCamera.BoundingFrustum.Intersects(primitiveObject.BoundingSphere))
//            {
//                primitiveObject.EffectParameters.SetParameters(activeCamera);
//                primitiveObject.EffectParameters.SetWorld(primitiveObject.GetWorldMatrix());
//                primitiveObject.VertexData.Draw(gameTime, primitiveObject.EffectParameters.Effect);
//            }
//        }

//        //Draw a model object 
//        private void DrawObject(GameTime gameTime, ModelObject modelObject, Camera3D activeCamera)
//        {
//            if (modelObject.Model != null)
//            {
//                modelObject.EffectParameters.SetParameters(activeCamera);
//                foreach (ModelMesh mesh in modelObject.Model.Meshes)
//                {
//                    foreach (ModelMeshPart part in mesh.MeshParts)
//                    {
//                        part.Effect = modelObject.EffectParameters.Effect;
//                    }

//                    modelObject.EffectParameters.SetWorld(modelObject.BoneTransforms[mesh.ParentBone.Index] * modelObject.GetWorldMatrix());
//                    mesh.Draw();
//                }
//            }
//        }

//        //Draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
//        private void DrawObject(GameTime gameTime, PrimitiveObject primitiveObject)
//        {
//            BasicEffect effect = primitiveObject.EffectParameters.Effect as BasicEffect;

//            //W, V, P, Apply, Draw
//            effect.World = primitiveObject.GetWorldMatrix();
//            effect.View = this.cameraManager.ActiveCamera.View;
//            effect.Projection = this.cameraManager.ActiveCamera.ProjectionParameters.Projection;

//            if (primitiveObject.EffectParameters.Texture != null)
//                effect.Texture = primitiveObject.EffectParameters.Texture;

//            effect.DiffuseColor = primitiveObject.EffectParameters.DiffuseColor.ToVector3();
//            effect.Alpha = primitiveObject.Alpha;

//            effect.CurrentTechnique.Passes[0].Apply();
//            primitiveObject.VertexData.Draw(gameTime, effect);
//        }

//        private void DrawObject(GameTime gameTime, AnimatedPlayerObject animatedPlayerObject, Camera3D activeCamera)
//        {
//            //An array of the current positions of the model meshes
//            Matrix[] bones = animatedPlayerObject.AnimationPlayer.GetSkinTransforms();
//            Matrix world = animatedPlayerObject.GetWorldMatrix();

//            for (int i = 0; i < bones.Length; i++)
//            {
//                bones[i] *= world;
//            }

//            foreach (ModelMesh mesh in animatedPlayerObject.Model.Meshes)
//            {
//                foreach (SkinnedEffect skinnedEffect in mesh.Effects)
//                {
//                    skinnedEffect.SetBoneTransforms(bones);
//                    skinnedEffect.View = activeCamera.View;
//                    skinnedEffect.Projection = activeCamera.Projection;

//                    //if you want to overwrite the texture you baked into the animation in 3DS Max then set your own texture
//                    if (animatedPlayerObject.EffectParameters.Texture != null)
//                        skinnedEffect.Texture = animatedPlayerObject.EffectParameters.Texture;

//                    skinnedEffect.DiffuseColor = animatedPlayerObject.EffectParameters.DiffuseColor.ToVector3();
//                    skinnedEffect.Alpha = animatedPlayerObject.Alpha;
//                    skinnedEffect.EnableDefaultLighting();
//                    skinnedEffect.PreferPerPixelLighting = true;
//                }
//                mesh.Draw();
//            }
//        }

//        private void DrawObject(GameTime gameTime, BillboardPrimitiveObject billboardPrimitiveObject, Camera3D activeCamera)
//        {
//            if (activeCamera.BoundingFrustum.Intersects(billboardPrimitiveObject.BoundingSphere))
//            {
//                billboardPrimitiveObject.EffectParameters.SetParameters(activeCamera, billboardPrimitiveObject.BillboardOrientationParameters);
//                billboardPrimitiveObject.EffectParameters.SetWorld(billboardPrimitiveObject.GetWorldMatrix());
//                billboardPrimitiveObject.VertexData.Draw(gameTime, billboardPrimitiveObject.EffectParameters.Effect);
//            }
//        }
//        #endregion
//    }
//}

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
        private Viewport fullViewport;
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
            StatusType statusType,
            Viewport fullViewport
        ) : this(game, cameraManager, 10, 10, eventDispatcher, statusType)
        {
            this.fullViewport = fullViewport;
        }

        public ObjectManager(
            Game game,
            CameraManager cameraManager,
            int transparentInitialSize,
            int opaqueInitialSize,
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : base(game, statusType, eventDispatcher)
        {
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
        #endregion

        #region Methods
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

            Game.GraphicsDevice.Viewport = this.fullViewport;

            base.ApplyDraw(gameTime);
        }

        ////Calls the DrawObject() based on underlying object type
        //private void DrawActor(GameTime gameTime, DrawnActor3D actor, Camera3D activeCamera)
        //{
        //    //Was the drawn enum value set?
        //    if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
        //        if (actor is ModelObject)
        //            DrawObject(gameTime, actor as ModelObject, activeCamera);

        //    //Add additional elseif statements here to render other object types (e.g model, animated, billboard etc)
        //}

        //Calls the correct DrawObject() based on underlying object type
        private void DrawActor(GameTime gameTime, Actor3D actor, Camera3D activeCamera)
        {
            //was the drawn enum value set?
            if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
            {
                if (actor is AnimatedPlayerObject)
                {
                    DrawObject(gameTime, actor as AnimatedPlayerObject, activeCamera);
                }
                else if (actor is AnimatedEnemyObject)
                {
                    DrawObject(gameTime, actor as AnimatedEnemyObject, activeCamera);
                }
                else if (actor is ModelObject)
                {
                    DrawObject(gameTime, actor as ModelObject, activeCamera);
                }
                else if (actor is BillboardPrimitiveObject)
                {
                    DrawObject(gameTime, actor as BillboardPrimitiveObject, activeCamera);
                }
                else if (actor is PrimitiveObject)
                {
                    DrawObject(gameTime, actor as PrimitiveObject, activeCamera);
                }
            }
        }

        //Draw an animated player object
        private void DrawObject(GameTime gameTime, AnimatedPlayerObject animatedPlayerObject, Camera3D activeCamera)
        {
            //An array of the current positions of the model meshes
            Matrix[] bones = animatedPlayerObject.AnimationPlayer.GetSkinTransforms();
            Matrix world = animatedPlayerObject.GetWorldMatrix();

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] *= world;
            }

            foreach (ModelMesh mesh in animatedPlayerObject.Model.Meshes)
            {
                foreach (SkinnedEffect skinnedEffect in mesh.Effects)
                {
                    skinnedEffect.SetBoneTransforms(bones);
                    skinnedEffect.View = activeCamera.View;
                    skinnedEffect.Projection = activeCamera.Projection;

                    //if you want to overwrite the texture you baked into the animation in 3DS Max then set your own texture
                    if (animatedPlayerObject.EffectParameters.Texture != null)
                        skinnedEffect.Texture = animatedPlayerObject.EffectParameters.Texture;

                    skinnedEffect.DiffuseColor = animatedPlayerObject.EffectParameters.DiffuseColor.ToVector3();
                    skinnedEffect.Alpha = animatedPlayerObject.Alpha;
                    skinnedEffect.EnableDefaultLighting();
                    skinnedEffect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        //Draw an animated player object
        private void DrawObject(GameTime gameTime, AnimatedEnemyObject animatedEnemyObject, Camera3D activeCamera)
        {
            //An array of the current positions of the model meshes
            Matrix[] bones = animatedEnemyObject.AnimationPlayer.GetSkinTransforms();
            Matrix world = animatedEnemyObject.GetWorldMatrix();

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] *= world;
            }

            foreach (ModelMesh mesh in animatedEnemyObject.Model.Meshes)
            {
                foreach (SkinnedEffect skinnedEffect in mesh.Effects)
                {
                    skinnedEffect.SetBoneTransforms(bones);
                    skinnedEffect.View = activeCamera.View;
                    skinnedEffect.Projection = activeCamera.Projection;
                    
                    //If you want to overwrite the texture you baked into the animation in 3DS Max then set your own texture
                    if (animatedEnemyObject.EffectParameters.Texture != null)
                        skinnedEffect.Texture = animatedEnemyObject.EffectParameters.Texture;

                    skinnedEffect.DiffuseColor = animatedEnemyObject.EffectParameters.DiffuseColor.ToVector3();
                    skinnedEffect.Alpha = animatedEnemyObject.Alpha;
                    skinnedEffect.EnableDefaultLighting();
                    skinnedEffect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        //Draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
        private void DrawObject(GameTime gameTime, PrimitiveObject primitiveObject, Camera3D activeCamera)
        {
            if (activeCamera.BoundingFrustum.Intersects(primitiveObject.BoundingSphere))
            {
                primitiveObject.EffectParameters.SetParameters(activeCamera);
                primitiveObject.EffectParameters.SetWorld(primitiveObject.GetWorldMatrix());
                primitiveObject.VertexData.Draw(gameTime, primitiveObject.EffectParameters.Effect);
            }
        }

        //Draw a NON-TEXTURED primitive i.e. vertices (and possibly indices) defined by the user
        private void DrawObject(GameTime gameTime, PrimitiveObject primitiveObject)
        {
            BasicEffect effect = primitiveObject.EffectParameters.Effect as BasicEffect;

            //W, V, P, Apply, Draw
            effect.World = primitiveObject.GetWorldMatrix();
            effect.View = this.cameraManager.ActiveCamera.View;
            effect.Projection = this.cameraManager.ActiveCamera.ProjectionParameters.Projection;

            if (primitiveObject.EffectParameters.Texture != null) effect.Texture = primitiveObject.EffectParameters.Texture;

            effect.DiffuseColor = primitiveObject.EffectParameters.DiffuseColor.ToVector3();
            effect.Alpha = primitiveObject.Alpha;

            effect.CurrentTechnique.Passes[0].Apply();
            primitiveObject.VertexData.Draw(gameTime, effect);
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

        private void DrawObject(GameTime gameTime, BillboardPrimitiveObject billboardPrimitiveObject, Camera3D activeCamera)
        {
            if (activeCamera.BoundingFrustum.Intersects(billboardPrimitiveObject.BoundingSphere))
            {
                billboardPrimitiveObject.EffectParameters.SetParameters(activeCamera, billboardPrimitiveObject.BillboardOrientationParameters);
                billboardPrimitiveObject.EffectParameters.SetWorld(billboardPrimitiveObject.GetWorldMatrix());
                billboardPrimitiveObject.VertexData.Draw(gameTime, billboardPrimitiveObject.EffectParameters.Effect);
            }
        }
        #endregion
    }
}