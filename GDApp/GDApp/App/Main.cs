using GDLibrary;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

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

        //Models
        private CollidableObject staticModel;
        private ModelObject drivableModel;

        //Dictionaries
        private ContentDictionary<Model> modelDictionary;
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SoundEffect> soundEffectDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;

        private List<String> soundEffectList = new List<String>();
        private List<TriggerVolume> triggerList = new List<TriggerVolume>();
        private GameStateManager gameStateManager;

        private int[,,] levelMap;
        
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialisation
        protected override void Initialize()
        {
            Window.Title = "Shrine to the Dark God";

            this.resolution = ScreenUtility.XVGA;

            InitializeGraphics(1280, 800);
            InitializeEffects();
            InitializeVertices();

            LoadDictionaries();
            LoadAssets();

            InitializeEventDispatcher();
            InitializeManagers();
            
            float worldScale = 2.54f;

            LoadLevelFromFile();

            LoadMapFromFile();
            InitializeMap(100, 100, 100, worldScale, 0, 5, 6, 4);
            InitializeCameras(worldScale, 1920, 1080);

            base.Initialize();
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

            #region Game State Manager
            this.gameStateManager = new GameStateManager(this, this.eventDispatcher, StatusType.Update);
            Components.Add(this.gameStateManager);
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

        private void InitializeEffects()
        {
            //Handle/reference to a default shader on the GFX card
            this.texturedVertexEffect = new BasicEffect(graphics.GraphicsDevice);

            //This.texturedVertexEffect.VertexColorEnabled = true; //enable to see R, G, B vertices
            this.texturedVertexEffect.TextureEnabled = true; //enable to see texture on the primitive

            //Setup textures
            this.modelEffect = new BasicEffect(graphics.GraphicsDevice);
            this.modelEffect.TextureEnabled = false;
            this.modelEffect.EnableDefaultLighting();
            this.modelEffect.PreferPerPixelLighting = false;

            //Setup fog
            this.modelEffect.FogColor = new Vector3(0.1f, 0.05f, 0.1f);
            this.modelEffect.FogEnabled = true;
            this.modelEffect.FogStart = 127;
            this.modelEffect.FogEnd = 400;

            //Setup color ambience
            this.modelEffect.DiffuseColor = new Vector3(0, 0, 0);
            this.modelEffect.AmbientLightColor = new Vector3(0.05f, 0, 0.05f);
            this.modelEffect.EmissiveColor = new Vector3(0.05f, 0, 0.05f);
        }
        #endregion

        #region Cameras
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
        #endregion

        #region Map Setup
        private void LoadLevelFromFile()
        {
            if (File.Exists("App/Data/currentLevel.txt"))
                GameStateManager.currentLevel = int.Parse(File.ReadAllText("App/Data/currentLevel.txt"));
            else
                GameStateManager.currentLevel = 1;
        }

        private void WriteLevelToFile()
        {
            File.WriteAllText("App/Data/mapData.txt", GameStateManager.currentLevel.ToString());
        }

        private void LoadMapFromFile()
        {
            #region Read File
            //Store all file data
            string fileText = File.ReadAllText("App/Data/mapData.txt");

            //Split the file into an array of levels
            string[] levels = fileText.Split('*');

            //Split the current level into an array of layers (y axis)
            string[] layers = levels[GameStateManager.currentLevel].Split('&');
            #endregion

            #region Determine Map Size
            CalculateMapSize(layers);
            #endregion

            #region Create Map
            CreateMap(layers);
            #endregion
        }

        private void CalculateMapSize(string[] layers)
        {
            int x = 0, y = 0, z = 0;

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

            this.levelMap = new int[x, y, z];
        }

        private void CreateMap(string[] layers)
        {
            int x = 0, y = 0, z = 0;

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
                    #region Create Rooms
                    CreateRooms(line, x, y, z);
                    #endregion

                    #region Create Sounds
                    CreateSounds(line, x, y, z);
                    #endregion

                    #region Create Triggers
                    CreateTriggers(line, x, y, z);
                    #endregion

                    //Iterate z
                    z++;
                }

                //Reset z
                z = 0;

                //Iterate y
                y++;
            }
        }

        private void CreateRooms(string line, int x, int y, int z)
        {
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
                this.levelMap[x, y, z] += int.Parse(room);

                //Iterate x
                x++;
            }
        }

        private void CreateSounds(string line, int x, int y, int z)
        {
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
                this.levelMap[x, y, z] += (int.Parse(sound) << 5);

                //Iterate x
                x++;
            }
        }

        private void CreateTriggers(string line, int x, int y, int z)
        {
            //Create sounds line
            string triggersLine;
            triggersLine = line.Split('-')[3].Trim();
            triggersLine = triggersLine.Replace('|', ' ');
            triggersLine = triggersLine.Replace(" ", string.Empty);

            //Split the sounds line into an array of sounds
            string[] triggers = triggersLine.Split(',');

            //Loop through each triggr
            foreach (string trigger in triggers)
            {
                //Place trigger in map
                this.levelMap[x, y, z] += (int.Parse(trigger) << 11);

                //Iterate x
                x++;
            }
        }

        private void InitializeMap(float cellWidth, float cellHeight, float cellDepth, float worldScale, int roomsStartPos, int reservedRoomBits, int reservedSoundBits, int reservedTriggerBits)
        {
            Transform3D transform;

            //Calculate the width, height and depth of each cell
            float width = (cellWidth * worldScale);
            float height = (cellHeight * worldScale);
            float depth = (cellDepth * worldScale);
            
            //Calculate the starting bit position of each component in the map
            int soundsStartPos = roomsStartPos + reservedRoomBits;
            int triggersStartPos = soundsStartPos + reservedSoundBits;

            #region Construct Cells
            //Loop through each element in the 3D level map array
            for (int x = 0; x < this.levelMap.GetLength(0); x++)
            {
                for (int y = 0; y < this.levelMap.GetLength(1); y++)
                {
                    for (int z = 0; z < this.levelMap.GetLength(2); z++)
                    {
                        #region Calculate Transform
                        //Calculate the transform position of each component in the map
                        transform = new Transform3D(
                            new Vector3(x * width, y * height, z * depth),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            Vector3.UnitZ,
                            Vector3.UnitY
                        );
                        #endregion

                        #region Construct Rooms
                        //Extract room from level map
                        int roomType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], reservedRoomBits, roomsStartPos);

                        //If a room has been set
                        if (roomType > 0) 
                            
                            //Construct room
                            constructRooms(roomType, transform);
                        #endregion

                        #region Construct Sounds
                        //Extract sound from level map
                        int soundType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], reservedSoundBits, soundsStartPos);

                        //If a sound has been set
                        if (soundType > 0)
                            
                            //Construct sound
                            constructSounds(soundType, transform);
                        #endregion

                        #region Construct Triggers
                        //Extract trigger from level map
                        int triggerType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], reservedTriggerBits, triggersStartPos);

                        //If a trigger has been set
                        if (triggerType > 0)

                            //Construct trigger
                            constructTriggers(triggerType, transform);
                        #endregion
                    }
                }
            }
            #endregion
        }

        public void constructRooms(int roomType, Transform3D transform)
        {
            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["roomEffect" + roomType];
            Model model = this.modelDictionary["roomModel" + roomType];

            //Create model
            this.staticModel = new CollidableObject(
                "room" + roomType,
                ActorType.CollidableArchitecture,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                model
            );

            //Add collision
            this.staticModel.AddPrimitive(new JigLibX.Geometry.Plane(transform.Up, transform.Translation), new MaterialProperties(0.8f, 0.8f, 0.7f));
            this.staticModel.Enable(true, 1);

            //Add to object manager list
            this.object3DManager.Add(staticModel);
        }

        public void constructSounds(int soundType, Transform3D transform)
        {
            //Add trigger to list
            this.triggerList.Add(
                new TriggerVolume(
                    transform.Translation.X,
                    transform.Translation.Y,
                    transform.Translation.Z,
                    (100 * 2.54f),
                    (100 * 2.54f),
                    (100 * 2.54f),
                    TriggerType.PlaySound,
                    new object[] { soundEffectList[soundType - 1] }
                )
            );        
        }

        public void constructTriggers(int triggerType, Transform3D transform)
        {
            //Determine trigger type
            switch (triggerType)
            {
                case 1:
                    this.triggerList.Add(
                        new TriggerVolume(
                            transform.Translation.X,
                            transform.Translation.Y,
                            transform.Translation.Z,
                            (100 * 2.54f),
                            (100 * 2.54f),
                            (100 * 2.54f),
                            TriggerType.InitiateBattle,
                            null
                        )
                    );
                    break;

                case 2:
                    this.triggerList.Add(
                        new TriggerVolume(
                            transform.Translation.X,
                            transform.Translation.Y,
                            transform.Translation.Z,
                            (100 * 2.54f),
                            (100 * 2.54f),
                            (100 * 2.54f),
                            TriggerType.EndLevel,
                            null
                        )
                    );
                    break;
            }
        }
        #endregion

        #region Content
        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
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
            this.soundEffectList.Add("boing");
            this.soundEffectList.Add("boing");
            this.soundEffectList.Add("boing");
            this.soundEffectList.Add("boing");
            #endregion
        }
        #endregion

        #region DEMOS
        private void DemoTriggerVolume()
        {
            foreach (TriggerVolume trigger in this.triggerList)
            {
                //If the actor is within the trigger, and the trigger has not yet fired
                if (trigger.isActorWithin(this.cameraManager[0]) && !trigger.HasFired)
                {
                    //Fire the trigger
                    trigger.fireEvent();

                    //Flag the trigger as fired
                    trigger.HasFired = true;
                }

                //If the actor is not within the trigger, reset the 'has fired' flag
                if (!trigger.isActorWithin(this.cameraManager[0]))
                {
                    trigger.HasFired = false;
                }
            }
        }
        #endregion

        #region Update, Draw
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            DemoTriggerVolume();
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
        #endregion
    }
}