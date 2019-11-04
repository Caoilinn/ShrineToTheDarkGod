using GDLibrary;
using JigLibX.Geometry;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System;

namespace GDApp
{
    public class Main : Game
    {
        //Graphics
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;

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
        private Dictionary<string, EffectParameters> effectDictionary;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = "Shrine to the Dark God";

            int currentLevel = 0; //Should load current level from file
            int[,,] levelMap;

            InitializeGraphics(1280, 800);
            InitializeEffects();
            InitializeVertices();

            InitializeEventDispatcher();
            InitializeManagers();

            levelMap = LoadMap(currentLevel);
            InitializeMap(levelMap);

            float worldScale = 2.54f;
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

            //Effect parameters
            this.effectDictionary = new Dictionary<string, EffectParameters>();
        }

        private void LoadAssets()
        {
            #region Models
            //Rooms
            this.modelDictionary.Load("Assets/Models/Rooms/room001", "room001");
            this.modelDictionary.Load("Assets/Models/Rooms/room002", "room002");
            this.modelDictionary.Load("Assets/Models/Rooms/room003", "room003");
            this.modelDictionary.Load("Assets/Models/Rooms/room004", "room004");
            this.modelDictionary.Load("Assets/Models/Rooms/room005", "room005");
            this.modelDictionary.Load("Assets/Models/Rooms/room006", "room006");
            this.modelDictionary.Load("Assets/Models/Rooms/room007", "room007");
            this.modelDictionary.Load("Assets/Models/Rooms/room008", "room008");
            this.modelDictionary.Load("Assets/Models/Rooms/room009", "room009");
            this.modelDictionary.Load("Assets/Models/Rooms/room010", "room010");
            this.modelDictionary.Load("Assets/Models/Rooms/room011", "room011");
            this.modelDictionary.Load("Assets/Models/Rooms/room012", "room012");
            this.modelDictionary.Load("Assets/Models/Rooms/room013", "room013");
            this.modelDictionary.Load("Assets/Models/Rooms/room014", "room014");
            this.modelDictionary.Load("Assets/Models/Rooms/room015", "room015");
            this.modelDictionary.Load("Assets/Models/Rooms/room016", "room016");
            #endregion

            #region Textures
            //Rooms
            this.textureDictionary.Load("Assets/Textures/Props/Crates/crate_001");
            #endregion
        }

        private void InitializeVertices()
        {
            this.vertices = VertexFactory.GetVertexPositionColorTextureQuad();
        }

        private void InitializeManagers()
        {
            #region Camera Manager
            this.cameraManager = new CameraManager(this);
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
            Transform3D transform = null;
            Camera3D camera = null;

            Viewport viewPort = new Viewport(0, 0, resolutionWidth, (int) (resolutionHeight / 1.25f));

            #region First-Person Camera
            transform = new Transform3D(
                new Vector3(635, 127, 889),
                Vector3.Zero,
                Vector3.One,
                -Vector3.UnitZ,
                Vector3.UnitY
            );

            camera = new Camera3D(
                "FP Cam 1",
                ActorType.Camera,
                StatusType.Update,
                transform,
                new ProjectionParameters(
                    MathHelper.ToRadians(45),
                    4 / 3.0f,
                    0.1f,
                    10000
                )
            );

            Vector3 movementVector = new Vector3(100, 100, 100) * worldScale;
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

            this.cameraManager.SetActiveCameraIndex(0);
        }

        private void InitializeEffects()
        {
            //handle/reference to a default shader on the GFX card
            this.texturedVertexEffect = new BasicEffect(graphics.GraphicsDevice);

            //this.texturedVertexEffect.VertexColorEnabled = true; //enable to see R, G, B vertices
            this.texturedVertexEffect.TextureEnabled = true; //enable to see texture on the primitive
            
            this.modelEffect = new BasicEffect(graphics.GraphicsDevice);
            this.modelEffect.TextureEnabled = false;
            this.modelEffect.EnableDefaultLighting();
            this.modelEffect.PreferPerPixelLighting = true;
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

        private int[, ,] LoadMap(int currentLevel)
        {
            #region Read File
            int x = 1, y = 1, z = 1;

            //Store all file data
            string fileText = File.ReadAllText("App/Data/levelData.txt");

            //Split the file into an array of levels
            string[] levels = fileText.Split('*');

            //Split the current level into an array of layers (y axis)
            string[] layers = levels[currentLevel].Split('&');
            #endregion

            #region Determine Map Size
            //Set y - The amount of layers correspond to the amount of rooms in the y dimension
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

                    //Update z - The amount of lines correspond to the amount of rooms in the z dimension
                    z = lines.Length;

                //Loop through each line
                foreach (string line in lines)
                {
                    //Cleanup line
                    string cleanLine;
                    cleanLine = line.Trim();
                    cleanLine = cleanLine.Replace('|', ' ');
                    cleanLine = cleanLine.Replace(" ", string.Empty);
                    cleanLine = cleanLine.Replace(",", string.Empty);

                    //If the line length is larger than the current x (each line length)
                    if (cleanLine.Length > x)

                        //Update x - The length of the line corresponds to the amount of rooms in the x dimension
                        x = cleanLine.Length;
                }
            }

            //Create array based on map size
            int[, ,] levelMap = new int[x, y, z];
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
                    //Cleanup line
                    string cleanLine;
                    cleanLine = line.Trim();
                    cleanLine = cleanLine.Replace('|', ' ');
                    cleanLine = cleanLine.Replace(" ", string.Empty);

                    //Split the current line into rooms
                    string[] rooms = cleanLine.Split(',');

                    //Loop through each room
                    foreach (string room in rooms)
                    {
                        //Place room in map
                        levelMap[x, y, z] = int.Parse(room);

                        //Iterate x
                        x++;
                    }

                    //Iterate z
                    z++;
                    x = 0;
                }

                //Iterate y
                y++;
                z = 0;
            }

            return levelMap;
            #endregion
        }

        private void InitializeMap(int[, ,] levelMap)
        {
            Transform3D transform;
            Texture2D texture;
            EffectParameters effectParameters;
            Model model;

            //To be taken out and placed into dictionaries
            #region Assets
            List<Texture2D> textureList = new List<Texture2D>() {
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
                Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1"),
            };

            List<EffectParameters> effectParametersList = new List<EffectParameters>() {
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1),
                new EffectParameters(this.modelEffect, null, Color.DarkGray, 1)
            };

            List<Model> modelList = new List<Model>() {
                Content.Load<Model>("Assets/Models/Rooms/room_001"),
                Content.Load<Model>("Assets/Models/Rooms/room_002"),
                Content.Load<Model>("Assets/Models/Rooms/room_003"),
                Content.Load<Model>("Assets/Models/Rooms/room_004"),
                Content.Load<Model>("Assets/Models/Rooms/room_005"),
                Content.Load<Model>("Assets/Models/Rooms/room_006"),
                Content.Load<Model>("Assets/Models/Rooms/room_007"),
                Content.Load<Model>("Assets/Models/Rooms/room_008"),
                Content.Load<Model>("Assets/Models/Rooms/room_009"),
                Content.Load<Model>("Assets/Models/Rooms/room_010"),
                Content.Load<Model>("Assets/Models/Rooms/room_011"),
                Content.Load<Model>("Assets/Models/Rooms/room_012"),
                Content.Load<Model>("Assets/Models/Rooms/room_013"),
                Content.Load<Model>("Assets/Models/Rooms/room_014"),
                Content.Load<Model>("Assets/Models/Rooms/room_015"),
                Content.Load<Model>("Assets/Models/Rooms/room_016"),
            };
            #endregion

            //Loop through each element in the 3D array
            for (int x = 0; x < levelMap.GetLength(0); x++) {
                for (int y = 0; y < levelMap.GetLength(1); y++) {
                    for (int z = 0; z < levelMap.GetLength(2); z++) {

                        //Calculate position
                        transform = new Transform3D(
                            new Vector3(x * (100 * 2.54f), y * (100 * 2.54f), z * (100 * 2.54f)),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            -Vector3.UnitZ,
                            Vector3.UnitY
                        );

                        //If a room has been set at this position
                        if (levelMap[x, y, z] > 0) {

                            //Load model, texture, and effect parameters
                            texture = textureList[(levelMap[x, y, z]) - 1];
                            effectParameters = effectParametersList[levelMap[x, y, z] - 1];
                            model = modelList[levelMap[x, y, z] - 1];

                            //Create model
                            this.staticModel = new CollidableObject(
                                "room" + z,
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
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            DemoCameraManager();
            DemoSoundManager();
            DriveModel(gameTime);

            base.Update(gameTime);
        }

        #region DEMOS
        private void DemoCameraManager()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Space))
                this.cameraManager.CycleCamera();

            Predicate<Camera3D> cameraPredicate = (
                (camera) => (camera.ID.Equals("Track Cam 1"))
            );

            Predicate<IController> controllerPredicate = (
                (control) => (control.GetControllerType().Equals(ControllerType.Track))
            );
        }

        private void DemoRegisterEvent()
        {
            EventSenderDemo sender = new EventSenderDemo(this);
            this.Components.Add(sender);

            //Operator overloaded
            //Takes function
            //Gets address of the function
            //Adds addess to the delegate list
            sender.PlayerChanged += NotifyMe;
            sender.CameraChanged += NotifyCameraChanged;
        }

        private void NotifyMe(string s, int x, object sender)
        {
            System.Diagnostics.Debug.WriteLine("Event: " + s);
        }

        private void NotifyCameraChanged(EventData eventData)
        {
            if (eventData.AdditionalParameters != null)
            {
                int length = eventData.AdditionalParameters.Length;
                string msg = eventData.AdditionalParameters[length - 1] as string;
                if (msg != null) Console.WriteLine("Sender said" + msg);
            }
        }

        private void DemoSoundManager()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.W) || this.keyboardManager.IsFirstKeyPress(Keys.S) || this.keyboardManager.IsFirstKeyPress(Keys.A) || this.keyboardManager.IsFirstKeyPress(Keys.D))
            {
                object[] additionalParameters = { "environment_stone_steps" };
                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnPlay,
                        EventCategoryType.Sound2D,
                        additionalParameters
                    )
                );
            }

            if (this.keyboardManager.IsFirstKeyPress(Keys.Q) || this.keyboardManager.IsFirstKeyPress(Keys.E))
            {
                object[] additionalParameters = { "turn_around" };
                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnPlay,
                        EventCategoryType.Sound2D,
                        additionalParameters
                    )
                );
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