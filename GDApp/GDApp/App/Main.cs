using GDLibrary;
using JigLibX.Geometry;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Xna.Framework.Audio;

namespace GDApp
{
    public class Main : Game
    {
        //Graphics
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        private Integer2 resolution;

        //Vertices
        private VertexPositionColorTexture[] vertices;

        //Effects
        private BasicEffect texturedVertexEffect;
        private BasicEffect modelEffect;

        //Managers
        private ManagerParameters managerParameters;
        private ObjectManager object3DManager;
        private CameraManager cameraManager;
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private SoundManager soundManager;

        //Dispatchers
        private EventDispatcher eventDispatcher;

        //Vectors
        private Vector2 screenCentre;
        private Vector3 driveRotation = Vector3.Zero;

        //Models
        private CollidableObject staticModel;
        private ModelObject drivableModel;

        //Dictionaries
        private ContentDictionary<Model> modelDictionary;
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SoundEffect> soundEffectDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;

        private List<TriggerVolume> triggerList = new List<TriggerVolume>();
        private TriggerVolume demoTrig;
        private TriggerVolume soundTrigger;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = "Shrine to the Dark God";

            int currentLevel = 1;
            int[,,] levelMap;

            this.resolution = ScreenUtility.XVGA;

            InitializeGraphics(1280, 800);
            InitializeEffects();
            InitializeVertices();

            LoadDictionaries();
            LoadAssets();

            InitializeEventDispatcher();
            InitializeManagers();

            float worldScale = 2.54f;

            levelMap = LoadMap(currentLevel);
            InitializeMap(levelMap, 100, 100, 100, worldScale);
            InitializeCameras(worldScale, 1920, 1080);

            base.Initialize();
        }

        private void InitializeModels()
        {
            #region DriveableModel
            Transform3D transform = new Transform3D(
                new Vector3(2, 2.54f, 0),
                new Vector3(0, 0, 0),
                2 * new Vector3(1, 1, 1),
                -Vector3.UnitZ,
                Vector3.UnitY
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");
            EffectParameters effectParameters = new EffectParameters(this.modelEffect, texture, Color.White, 1);
            Model model = Content.Load<Model>("Assets/Models/box2");

            this.drivableModel = new ModelObject(
                "box1",
                ActorType.Interactable,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                model
            );

            this.object3DManager.Add(drivableModel);
            #endregion

            #region StaticModels
            transform = new Transform3D(
                new Vector3(2, 2.54f, 0),
                new Vector3(0, 0, 0),
                2 * new Vector3(1, 1, 1),
                -Vector3.UnitZ,
                Vector3.UnitY
            );

            texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");
            effectParameters = new EffectParameters(this.modelEffect, texture, Color.White, 1);
            model = Content.Load<Model>("Assets/Models/room_001");

            this.staticModel = new CollidableObject(
                "room",
                ActorType.Billboard,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                model
            );

            this.object3DManager.Add(staticModel);
            #endregion

            #region Clones
            /*
            for(int i = -5; i <5; i++)
            {
                ModelObject clone = archetype.Clone() as ModelObject;
                clone.Transform.Translation = new Vector3(4 * i, 2, 0);
                clone.Transform.Scale = Vector3.One * (i+6) * 0.1f;
                clone.EffectParameters.DiffuseColor = Color.PaleGreen;
               // clone.EffectParameters.Alpha = 0.1f;
                this.object3DManager.Add(clone);
            }
            */
            #endregion
        }

        private void LoadDictionaries()
        {
            //Models
            this.modelDictionary = new ContentDictionary<Model>("Model Dictionary", this.Content);

            //Textures
            this.textureDictionary = new ContentDictionary<Texture2D>("Texture Dictionary", this.Content);

            //Sounds
            this.soundEffectDictionary = new ContentDictionary<SoundEffect>("Sound Effect Dictionary", this.Content);

            //Effect parameters
            this.effectDictionary = new Dictionary<string, EffectParameters>();
        }

        private void LoadAssets()
        {
            #region Models
            //Rooms
            this.modelDictionary.Load("Assets/Models/Rooms/room_001", "roomModel1");
            this.modelDictionary.Load("Assets/Models/Rooms/room_002", "roomModel2");
            this.modelDictionary.Load("Assets/Models/Rooms/room_003", "roomModel3");
            this.modelDictionary.Load("Assets/Models/Rooms/room_004", "roomModel4");
            this.modelDictionary.Load("Assets/Models/Rooms/room_005", "roomModel5");
            this.modelDictionary.Load("Assets/Models/Rooms/room_006", "roomModel6");
            this.modelDictionary.Load("Assets/Models/Rooms/room_007", "roomModel7");
            this.modelDictionary.Load("Assets/Models/Rooms/room_008", "roomModel8");
            this.modelDictionary.Load("Assets/Models/Rooms/room_009", "roomModel9");
            this.modelDictionary.Load("Assets/Models/Rooms/room_010", "roomModel10");
            this.modelDictionary.Load("Assets/Models/Rooms/room_011", "roomModel11");
            this.modelDictionary.Load("Assets/Models/Rooms/room_012", "roomModel12");
            this.modelDictionary.Load("Assets/Models/Rooms/room_013", "roomModel13");
            this.modelDictionary.Load("Assets/Models/Rooms/room_014", "roomModel14");
            this.modelDictionary.Load("Assets/Models/Rooms/room_015", "roomModel15");
            this.modelDictionary.Load("Assets/Models/Rooms/room_016", "roomModel16");
            #endregion

            #region Textures
            //Rooms
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_001", "roomTexture1");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_002", "roomTexture2");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_003", "roomTexture3");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_004", "roomTexture4");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_005", "roomTexture5");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_006", "roomTexture6");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_007", "roomTexture7");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_008", "roomTexture8");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_009", "roomTexture9");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_010", "roomTexture10");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_011", "roomTexture11");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_012", "roomTexture12");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_013", "roomTexture13");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_014", "roomTexture14");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_015", "roomTexture15");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_016", "roomTexture16");
            #endregion

            #region Effect Parameters
            //Rooms
            this.effectDictionary.Add("roomEffect1", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture1"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect2", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture2"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect3", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture3"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect4", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture4"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect5", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture5"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect6", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture6"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect7", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture7"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect8", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture8"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect9", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture9"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect10", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture10"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect11", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture11"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect12", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture12"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect13", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture13"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect14", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture14"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect15", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture15"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect16", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture16"], Color.DarkGray, 1));
            #endregion

            #region Sounds
            this.soundEffectDictionary.Load("Assets/Audio/boing", "boing");
            #endregion
        }

        private void InitializeVertices()
        {
            this.vertices = VertexFactory.GetVertexPositionColorTextureQuad();
        }

        private void InitializeManagers()
        {
            #region Camera Manager
            this.cameraManager = new CameraManager(this, 2, this.eventDispatcher, StatusType.Update);
            Components.Add(this.cameraManager);
            #endregion

            #region Object Manager
            this.object3DManager = new ObjectManager(this, this.cameraManager);
            Components.Add(this.object3DManager);
            #endregion

            #region Mouse Manager
            this.mouseManager = new MouseManager(this);
            this.mouseManager.SetPosition(this.screenCentre);
            Components.Add(this.mouseManager);
            #endregion

            #region Keyboard Manager
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);
            #endregion

            #region Gamepad Manager
            #endregion

            #region SoundManager
            this.soundManager = new SoundManager(
                this,
                this.eventDispatcher,
                StatusType.Update,
                "Content/Assets/Audio/",
                "GameAudio.xgs",
                "Movement.xwb",
                "Movement.xsb"
            );

            Components.Add(this.soundManager);
            #endregion

            #region Manager Parameters
            this.managerParameters = new ManagerParameters(
                object3DManager,
                cameraManager,
                mouseManager,
                keyboardManager,
                gamePadManager,
                soundManager
            );
            #endregion
        }

        private void InitializeEventDispatcher()
        {
            //Initialize with an arbitrary size based on the expected number of events per update cycle, increase/reduce where appropriate
            this.eventDispatcher = new EventDispatcher(this, 20);

            //Dont forget to add to the Component list otherwise EventDispatcher::Update won't get called and no event processing will occur!
            Components.Add(this.eventDispatcher);
        }

        private void InitializeGraphics(int width, int height)
        {
            //set the preferred resolution
            //See https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            this.graphics.PreferredBackBufferWidth = width;
            this.graphics.PreferredBackBufferHeight = height;

            this.IsMouseVisible = true;

            //ensure that we call ApplyChanges otherwise the settings will not take effect
            this.graphics.ApplyChanges();

            this.screenCentre = new Vector2(
               this.graphics.PreferredBackBufferWidth / 2,
               this.graphics.PreferredBackBufferHeight / 2
            );
        }

        private void InitializeCameras(float worldScale, int resolutionWidth, int resolutionHeight)
        {
            float aspectRatio = (4.0f / 3.0f);
            float nearClipPlane = 0.1f;
            float farClipPlane = 10000;

            ProjectionParameters projectionParameters;
            Viewport viewport;
            float depth;

            #region First-Person Camera
            projectionParameters = new ProjectionParameters(
                MathHelper.ToRadians(45),
                aspectRatio,
                nearClipPlane,
                farClipPlane
            );

            viewport = new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            depth = 0f;

            AddFirstPersonCamera(projectionParameters, viewport, depth);
            #endregion

            #region Mini-map
            //projectionParameters = new ProjectionParameters(
            //    MathHelper.ToRadians(45),
            //    aspectRatio,
            //    nearClipPlane,
            //    farClipPlane
            //);

            //viewport = new Viewport(100, 100, 175, 175);
            //depth = 0f;

            //AddSecurityCamera(projectionParameters, viewport, depth);
            #endregion

            #region Rail Camera
            //transform = new Transform3D(
            //    new Vector3(0, 5, 100),
            //    Vector3.Zero,
            //    Vector3.One, //scale makes no sense for a camera so we may add another Transform3D constructor flavour
            //    -Vector3.UnitZ,
            //    Vector3.UnitY
            //);

            //camera = new Camera3D(
            //    "Rail Cam 1",
            //    ActorType.Camera,
            //    StatusType.Update, //cameras dont need to be drawn!
            //    transform,
            //    new ProjectionParameters(
            //        MathHelper.ToRadians(45),
            //        4 / 3.0f,
            //        0.1f,
            //        500
            //    )
            //);

            //RailParameters rail = new RailParameters(
            //    "Rail 1",
            //    new Vector3(-60, 10, 50),
            //    new Vector3(60, 10, 10)
            //);

            //IController railController = new RailController(
            //    "Rail Controller 1",
            //    ControllerType.Rail,
            //    this.drivableModel,
            //    rail
            //);

            //camera.AttachController(railController);
            //this.cameraManager.Add(camera);
            #endregion

            #region Track Camera
            //transform = new Transform3D(
            //    Vector3.Zero,
            //    Vector3.Zero,
            //    Vector3.One,
            //    -Vector3.UnitZ,
            //    Vector3.UnitY
            //);

            //camera = new Camera3D("Track Cam 1",
            //    ActorType.Camera,
            //    StatusType.Update,
            //    transform,
            //    new ProjectionParameters(
            //        MathHelper.PiOver4,
            //        (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight,
            //        0.1f, 750
            //    )
            //);

            //Transform3DCurve transform3DCurve = new Transform3DCurve(CurveLoopType.Constant);
            //transform3DCurve.Add(new Vector3(0, 10, 200), -Vector3.UnitZ, Vector3.UnitY, 0);
            //transform3DCurve.Add(new Vector3(0, 15, 180), -Vector3.UnitZ, Vector3.UnitY, 2);  //A
            //transform3DCurve.Add(new Vector3(0, 10, 170), -Vector3.UnitZ, Vector3.UnitX, 3);  //B
            //transform3DCurve.Add(new Vector3(0, 5, 160), -Vector3.UnitZ, -Vector3.UnitY, 4);  //C
            //transform3DCurve.Add(new Vector3(0, 10, 150), -Vector3.UnitZ, -Vector3.UnitX, 5);  //D
            //transform3DCurve.Add(new Vector3(0, 15, 140), -Vector3.UnitZ, Vector3.UnitY, 6);  //E
            //transform3DCurve.Add(new Vector3(0, 15, 130), -Vector3.UnitZ, Vector3.UnitY, 11);  //A
            //transform3DCurve.Add(new Vector3(0, 10, 120), -Vector3.UnitZ, Vector3.UnitX, 18);  //B
            //transform3DCurve.Add(new Vector3(0, 5, 110), -Vector3.UnitZ, -Vector3.UnitY, 19);  //C
            //transform3DCurve.Add(new Vector3(0, 10, 100), -Vector3.UnitZ, -Vector3.UnitX, 20);  //D
            //transform3DCurve.Add(new Vector3(0, 15, 90), -Vector3.UnitZ, Vector3.UnitY, 30);  //E

            //TrackController trackController = new TrackController(
            //    "Track Controller 1",
            //    ControllerType.Track,
            //    transform3DCurve
            //);

            //camera.AttachController(trackController);
            //this.cameraManager.Add(camera);
            #endregion
        }

        private void AddFirstPersonCamera(ProjectionParameters projectionParameters, Viewport viewport, float depth)
        {
            Transform3D transform = new Transform3D(
                new Vector3(635, 127, 889),
                Vector3.Zero,
                Vector3.One,
                -Vector3.UnitZ,
                Vector3.UnitY
            );

            viewport = new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Camera3D camera = new Camera3D(
                "FP Cam 1",
                ActorType.Camera,
                StatusType.Update,
                transform,
                projectionParameters,
                viewport,
                depth
            );

            Vector3 movementVector = new Vector3(100, 100, 100) * 2.54f;
            Vector3 rotationVector = new Vector3(0, 90, 0);

            IController firstPersonCameraController = new FirstPersonCameraController(
                "FP Controller 1",
                ControllerType.FirstPerson,
                AppData.CameraMoveKeys,
                AppData.CameraMoveSpeed,
                AppData.CameraStrafeSpeed,
                AppData.CameraRotationSpeed,
                this.managerParameters,
                movementVector,
                rotationVector
            );

            camera.AttachController(firstPersonCameraController);
            this.cameraManager.Add(camera);
        }

        private void AddSecurityCamera(ProjectionParameters projectionParameters, Viewport viewport, float depth)
        {
            Transform3D transform = new Transform3D(
                new Vector3(50, 10, 10),
                Vector3.Zero,
                Vector3.Zero,
                -Vector3.UnitX,
                Vector3.UnitY
            );

            Camera3D camera = new Camera3D(
                "Map Cam 1",
                ActorType.Camera,
                StatusType.Update,
                transform,
                projectionParameters,
                viewport,
                depth
            );

            camera.AttachController(
                new SecurityCameraController(
                    "scc1",
                    ControllerType.Security,
                    15,
                    2,
                    Vector3.UnitX
                )
            );

            this.cameraManager.Add(camera);
        }

        private void InitializeEffects()
        {
            //Handle/reference to a default shader on the GFX card
            this.texturedVertexEffect = new BasicEffect(graphics.GraphicsDevice);

            //This.texturedVertexEffect.VertexColorEnabled = true; //enable to see R, G, B vertices
            this.texturedVertexEffect.TextureEnabled = true; //enable to see texture on the primitive

            this.modelEffect = new BasicEffect(graphics.GraphicsDevice);
            this.modelEffect.TextureEnabled = false;
            this.modelEffect.EnableDefaultLighting();
            this.modelEffect.PreferPerPixelLighting = true;
            this.modelEffect.FogColor = new Vector3(0.1f, 0.05f, 0.1f);
            this.modelEffect.FogEnabled = true;
            this.modelEffect.FogStart = 127;
            this.modelEffect.FogEnd = 400;

            this.modelEffect.DiffuseColor = new Vector3(0, 0, 0);
            this.modelEffect.AmbientLightColor = new Vector3(0.05f, 0, 0.05f);
            this.modelEffect.EmissiveColor = new Vector3(0.05f, 0, 0.05f);
            //this.modelEffect.DirectionalLight0.Direction = new Vector3(0, 0, 1);
        }

        #region Sky Box
        //private void InitializeGround(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(0, 0, 0),
        //        new Vector3(-90, 0, 0),
        //        new Vector3(worldScale, worldScale, 1),
        //        Vector3.UnitZ, 
        //        Vector3.UnitY
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices, 
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "grass", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}

        //private void InitializeRight(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(worldScale / 2, 0, 0),
        //        new Vector3(0, -90, 0),
        //        new Vector3(worldScale, worldScale, 1),
        //        Vector3.UnitZ, 
        //        Vector3.UnitY
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices,
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "right skybox", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}

        //private void InitializeLeft(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(-worldScale / 2, 0, 0),
        //        new Vector3(0, 90, 0),
        //        new Vector3(worldScale, worldScale, 1),
        //        Vector3.UnitZ, 
        //        Vector3.UnitY
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices,
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "left skybox", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}

        //private void InitializeBack(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(0, 0, -worldScale / 2),
        //        Vector3.Zero,
        //        new Vector3(worldScale, worldScale, 1),
        //        Vector3.UnitZ, 
        //        Vector3.UnitY
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices,
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "back skybox", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}

        //private void InitializeTop(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(0, worldScale / 2, 0),
        //        new Vector3(90, -90, 0),
        //        new Vector3(worldScale, worldScale, 1),
        //        -Vector3.UnitY, 
        //        Vector3.UnitZ
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices,
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "back skybox", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}

        //private void InitializeFront(float worldScale)
        //{
        //    Transform3D transform = new Transform3D(
        //        new Vector3(0, 0, worldScale / 2),
        //        new Vector3(0, 180, 0),
        //        new Vector3(worldScale, worldScale, 1),
        //        -Vector3.UnitY, 
        //        Vector3.UnitZ
        //    );

        //    Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
        //    EffectParameters effectParameters = new EffectParameters(
        //        this.texturedVertexEffect,
        //        texture, 
        //        Color.White, 
        //        1
        //    );

        //    VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
        //        this.vertices,
        //        PrimitiveType.TriangleStrip, 
        //        2
        //    );

        //    PrimitiveObject pObj = new PrimitiveObject(
        //        "front skybox", 
        //        ActorType.Decorator,
        //        StatusType.Update | StatusType.Drawn,
        //        transform,
        //        effectParameters,
        //        vertexData
        //    );

        //    this.object3DManager.Add(pObj);
        //}
        #endregion

        private int[,,] LoadMap(int currentLevel)
        {
            #region Read File
            int x = 1, y = 1, z = 1;

            //Store all file data
            string fileText = File.ReadAllText("App/Data/mapData.txt");

            //Split the file into an array of levels
            string[] levels = fileText.Split('*');

            //Split the current level into an array of layers (y axis)
            string[] layers = levels[currentLevel].Split('&');
            #endregion

            #region Determine Map Size
            //Set y - The amount of layers correspond to the amount of cells in the y dimension
            y = layers.Length;

            //Loop through each layer
            foreach (string layer in layers)
            {
                //Cleanup layer
                string cleanLayer;
                cleanLayer = layer.Trim();

                //Split the current layer into lines
                string[] lines = cleanLayer.Split('/');

                //If the amount of lines is larger than the current z (lines = z)
                if (lines.Length > z)

                    //Update z - The amount of lines correspond to the amount of cells in the z dimension
                    z = lines.Length;

                //Loop through each line
                foreach (string line in lines)
                {
                    //Cleanup line
                    string cellLine;
                    cellLine = line.Split('-')[1].Trim();            //Measures room element of the text file - see GDApp/App/Data/levelData.txt
                                                                     //Makes an assumption that all elements (rooms, sounds, items) of the map are of the same dimension
                    cellLine = cellLine.Replace('|', ' ');
                    cellLine = cellLine.Replace(" ", string.Empty);
                    string[] cells = cellLine.Split(',');

                    //If the current amount of cells in is larger than the current x (amount of cells in a line)
                    if (cells.Length > x)

                        //Update x - The amount of elements in the cells array corresponds to the amount of cells in the x dimension
                        x = cells.Length;
                }
            }

            //Create array based on map size
            int[,,] levelMap = new int[x, y, z];
            #endregion

            #region Create Map
            x = y = z = 0;

            //Loop through each layer
            foreach (string layer in layers)
            {
                //Cleanup layer
                string cleanLayer;
                cleanLayer = layer.Trim();

                //Split the current layer into lines
                string[] lines = cleanLayer.Split('/');

                //Loop through each line
                foreach (string line in lines)
                {
                    #region Rooms
                    //Create rooms line
                    string roomsLine;
                    roomsLine = line.Split('-')[1].Trim();
                    roomsLine = roomsLine.Replace('|', ' ');
                    roomsLine = roomsLine.Replace(" ", string.Empty);

                    //Split the rooms line into an array of rooms
                    string[] rooms = roomsLine.Split(',');

                    //Loop through each room
                    foreach (string room in rooms)
                    {
                        //Place room in map
                        levelMap[x, y, z] += int.Parse(room);

                        //Iterate x
                        x++;
                    }

                    //Reset x
                    x = 0;
                    #endregion

                    #region Sounds
                    //Create sounds line
                    string soundsLine;
                    soundsLine = line.Split('-')[2].Trim();
                    soundsLine = soundsLine.Replace('|', ' ');
                    soundsLine = soundsLine.Replace(" ", string.Empty);

                    //Split the sounds line into an array of sounds
                    string[] sounds = soundsLine.Split(',');

                    //Loop through each sound
                    foreach (string sound in sounds)
                    {
                        //Place room in map
                        levelMap[x, y, z] += (int.Parse(sound) << 5);

                        //Iterate x
                        x++;
                    }
                    
                    //Reset x
                    x = 0;
                    #endregion

                    #region Positions
                    //Create sounds line
                    string positionsLine;
                    positionsLine = line.Split('-')[3].Trim();
                    positionsLine = positionsLine.Replace('|', ' ');
                    positionsLine = positionsLine.Replace(" ", string.Empty);

                    //Split the sounds line into an array of sounds
                    string[] positions = positionsLine.Split(',');

                    //Loop through each sound
                    foreach (string position in positions)
                    {
                        //Place room in map
                        levelMap[x, y, z] += (int.Parse(position) << 10);

                        //Iterate x
                        x++;
                    }

                    //Reset x
                    x = 0;
                    #endregion

                    //Iterate z
                    z++;
                }

                //Reset z
                z = 0;

                //Iterate y
                y++;
            }

            return levelMap;
            #endregion
        }

        private void InitializeMap(int[,,] levelMap, float cellWidth, float cellHeight, float cellDepth, float worldScale)
        {
            TriggerVolume trigger;
            Transform3D transform;
            EffectParameters effectParameters;
            Model model;

            float width = (cellWidth * worldScale);
            float height = (cellHeight * worldScale);
            float depth = (cellDepth * worldScale);

            #region Construct Cells
            //Loop through each element in the 3D array
            for (int x = 0; x < levelMap.GetLength(0); x++)
            {
                for (int y = 0; y < levelMap.GetLength(1); y++)
                {
                    for (int z = 0; z < levelMap.GetLength(2); z++)
                    {
                        //Calculate trigger position
                        transform = new Transform3D(
                            new Vector3(x * width, y * height, z * depth),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            Vector3.UnitZ,
                            Vector3.UnitY
                        );

                        #region Construct Rooms
                        //Extract room from level map
                        int roomType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(levelMap[x, y, z], 5, 0);

                        //If a room has been set at this position
                        if (roomType > 0)
                        {

                            //Load model and effect parameters
                            effectParameters = this.effectDictionary["roomEffect" + roomType];
                            model = this.modelDictionary["roomModel" + roomType];

                            //Create model
                            this.staticModel = new CollidableObject(
                                "room" + roomType,
                                ActorType.CollidableArchitecture,
                                StatusType.Update | StatusType.Drawn,
                                transform,
                                effectParameters,
                                model
                            );

                            this.staticModel.AddPrimitive(new JigLibX.Geometry.Plane(transform.Up, transform.Translation), new MaterialProperties(0.8f, 0.8f, 0.7f));
                            this.staticModel.Enable(true, 1);

                            //Add to object manager list
                            this.object3DManager.Add(staticModel);
                        }
                        #endregion

                        #region Construct Sound Triggers
                        List<String> soundEffectList = new List<String>();
                        soundEffectList.Add("boing");
                        soundEffectList.Add("boing");

                        //Extract sound from level map
                        int soundType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(levelMap[x, y, z], 5, 5);

                        //If a sound has been set at this position
                        if (soundType > 0)
                        {
                            this.soundTrigger = new TriggerVolume(
                                x * (100 * 2.54f),
                                y * (100 * 2.54f),
                                z * (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                TriggerType.PlaySound,
                                new object[] { soundEffectList[soundType - 1] }
                            );

                            this.triggerList.Add(soundTrigger);
                        }
                        #endregion

                        #region Construct General Triggers
                        //Extract triggers from level map
                        int triggerType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(levelMap[x, y, z], 3, 10);

                        //If a trigger has been set at this position
                        if (triggerType == 1)
                        {
                            trigger = new TriggerVolume(
                                x * (100 * 2.54f),
                                y * (100 * 2.54f),
                                z * (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                TriggerType.InitiateBattle,
                                null
                            );

                            this.triggerList.Add(trigger);
                        }
                        else if (triggerType == 2)
                        {
                            trigger = new TriggerVolume(
                                x * (100 * 2.54f),
                                y * (100 * 2.54f),
                                z * (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                (100 * 2.54f),
                                TriggerType.EndLevel,
                                new object[] { soundEffectList[triggerType - 1] }
                            );

                            this.triggerList.Add(trigger);
                        }
                        #endregion
                    }
                }
            }
            #endregion
        }

        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            DemoTriggerVolume();
        }

        #region DEMOS
        private void DemoTriggerVolume()
        {
            foreach (TriggerVolume trig in this.triggerList)
            {
                //If the actor is within the trigger, and the trigger has not yet fired
                if (trig.isActorWithin(this.cameraManager[0]) && !trig.HasFired)
                {
                    //Fire the trigger
                    trig.Test();

                    //Flag the trigger as fired
                    trig.HasFired = true;
                }

                //If the actor is not within the trigger, reset the 'has fired' flag
                if (!trig.isActorWithin(this.cameraManager[0]))
                {
                    trig.HasFired = false;
                }
            }
        }
        #endregion

        private void DriveModel(GameTime gameTime)
        {
            float rotationSpeed = 0.05f;
            KeyboardState ksState = Keyboard.GetState();

            if (ksState.IsKeyDown(Keys.Up))
            {
                this.drivableModel.Transform.TranslateBy(this.drivableModel.Transform.Look * gameTime.ElapsedGameTime.Milliseconds * 0.05f);
            }
            else if (ksState.IsKeyDown(Keys.Down))
            {
                this.drivableModel.Transform.TranslateBy(-this.drivableModel.Transform.Look * gameTime.ElapsedGameTime.Milliseconds * 0.05f);
            }

            if (ksState.IsKeyDown(Keys.Left))
            {
                this.driveRotation += Vector3.UnitY * gameTime.ElapsedGameTime.Milliseconds * rotationSpeed;
                this.drivableModel.Transform.RotateBy(this.driveRotation);
            }
            else if (ksState.IsKeyDown(Keys.Right))
            {
                this.driveRotation -= Vector3.UnitY * gameTime.ElapsedGameTime.Milliseconds * rotationSpeed;
                this.drivableModel.Transform.RotateBy(this.driveRotation);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            /*
             * Think of a sampler as a paint brush. The sampler state defines
             * how a texture will be drawn on a surface and, more specifically,
             * what happens if the surface contains UV co-ordinates outside the range 0->1.
             */
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            this.graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            RasterizerState rsState = new RasterizerState();
            rsState.CullMode = CullMode.None;
            rsState.FillMode = FillMode.Solid;
            this.graphics.GraphicsDevice.RasterizerState = rsState;

            base.Draw(gameTime);
        }
    }
}