#define DEBUG

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
        private BasicEffect modelEffect;
        private BasicEffect texturedModelEffect;
        private BasicEffect texturedVertexEffect;

        //Managers
        private GameStateManager gameStateManager;
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

        //Dictionaries
        private ContentDictionary<Model> modelDictionary;
        private ContentDictionary<Model> collisionBoxDictionary;
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SoundEffect> soundEffectDictionary;
        private ContentDictionary<SpriteFont> fontDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;

        //Lists
        private List<string> soundEffectList = new List<String>();
        private List<TriggerVolume> triggerList = new List<TriggerVolume>();

        //Debug
        private PhysicsDebugDrawer physicsDebugDrawer;

        //Map
        private int[,,] levelMap;

        //Array Position
        private int roomsStartPosition = 1;
        private int pickupsStartPosition = 2;
        private int triggersStartPosition = 3;
        private int playersStartPosition = 4;
        private int enemiesStartPosition = 5;

        //Array Shift Position
        private int roomsShiftPosition;
        private int pickupsShiftPosition;
        private int triggersShiftPosition;
        private int enemiesShiftPosition;
        private int playersShiftPosition;

        //Array Reserved Bits
        private int reservedRoomBits;
        private int reservedPickupBits;
        private int reservedTriggerBits;
        private int reservedPlayerBits;
        private int reservedEnemyBits;

        //Player Posiiton
        private Transform3D playerPosition;
        private BasicEffect pickupEffect;

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

            LoadContent();

            InitializeEventDispatcher();
            InitializeManagers();
            
            float worldScale = 2.54f;

            SetupBitArray(0, 5, 4, 4, 3, 2);

            LoadLevelFromFile();
            LoadMapFromFile();
            InitializeMap(100, 100, 100, worldScale);
            InitializeCameras(worldScale, 1920, 1080);

            InitializeDebugCollisionSkinInfo();

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
            this.modelEffect.LightingEnabled = false;
            this.modelEffect.EnableDefaultLighting();
            this.modelEffect.PreferPerPixelLighting = true;

            ////Setup fog
            //this.modelEffect.FogColor = new Vector3(0.1f, 0.05f, 0.1f);
            //this.modelEffect.FogEnabled = true;
            //this.modelEffect.FogStart = 127;
            //this.modelEffect.FogEnd = 400;

            //Setup color ambience
            this.modelEffect.DiffuseColor = new Vector3(0, 0, 0);
            this.modelEffect.AmbientLightColor = new Vector3(0.05f, 0, 0.05f);
            this.modelEffect.EmissiveColor = new Vector3(0.05f, 0, 0.05f);

            this.pickupEffect = new BasicEffect(graphics.GraphicsDevice);
            this.pickupEffect.TextureEnabled = true;
            this.pickupEffect.EnableDefaultLighting();
            this.pickupEffect.PreferPerPixelLighting = true;

            //this.texturedModelEffect = new BasicEffect(graphics.GraphicsDevice);
            //this.texturedModelEffect.TextureEnabled = true;
            //this.texturedModelEffect.EnableDefaultLighting();
            //this.texturedModelEffect.PreferPerPixelLighting = true;
            //this.texturedModelEffect.DiffuseColor = new Vector3(1,1,1);
            //this.texturedModelEffect.Texture = null;
            //this.texturedModelEffect.Alpha = 1;
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
        }

        private void AddFirstPersonCamera(ProjectionParameters projectionParameters, Viewport viewport, float depth)
        {
            //Transform3D transform = new Transform3D(
            //    new Vector3(635, 381, 1143),
            //    Vector3.Zero,
            //    Vector3.One,
            //    -Vector3.UnitZ,
            //    Vector3.UnitY
            //);

            //IController firstPersonCameraController = new FirstPersonCameraController(
            //    "FP Controller 1",
            //    ControllerType.FirstPerson,
            //    AppData.CameraMoveKeys,
            //    AppData.CameraMoveSpeed,
            //    AppData.CameraStrafeSpeed,
            //    AppData.CameraRotationSpeed,
            //    this.managerParameters,
            //    movementVector,
            //    rotationVector
            //);

            this.playerPosition = new Transform3D(
                new Vector3(635, 381, 1143),
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
                this.playerPosition,
                projectionParameters,
                viewport,
                depth
            );

            Vector3 movementVector = new Vector3(100, 100, 100) * 2.54f;
            Vector3 rotationVector = new Vector3(0, 90, 0);

            float width = 254;
            float height = 254;
            Vector3 translationalOffset = new Vector3(0, 0, 0);

            IController collidableFirstPersonCameraController = new CollidableFirstPersonCameraController(
                "CFPC Controller 1",
                ControllerType.FirstPerson,
                AppData.CameraMoveKeys,
                AppData.CameraMoveSpeed,
                AppData.CameraStrafeSpeed,
                AppData.CameraRotationSpeed,
                this.managerParameters,
                movementVector,
                rotationVector,
                camera,
                width,
                height,
                translationalOffset
            );

            camera.AttachController(collidableFirstPersonCameraController);
            this.cameraManager.Add(camera);
        }
        #endregion

        #region Map Setup
        private void SetupBitArray(int roomsShiftPosition, int reservedRoomBits, int reservedPickupBits, int reservedTriggerBits, int reservedPlayerBits, int reservedEnemybits)
        {
            //Reserve bits for each map component
            this.reservedRoomBits = reservedRoomBits;
            this.reservedPickupBits = reservedPickupBits;
            this.reservedTriggerBits = reservedTriggerBits;
            this.reservedPlayerBits = reservedPlayerBits;
            this.reservedEnemyBits = reservedEnemybits;

            //Calculate shift for each map component, relative to previous component
            this.roomsShiftPosition = roomsShiftPosition;
            this.pickupsShiftPosition = this.roomsShiftPosition + this.reservedRoomBits;
            this.triggersShiftPosition = this.pickupsShiftPosition + this.reservedPickupBits;
            this.playersShiftPosition = this.triggersShiftPosition + this.reservedTriggerBits;
            this.enemiesShiftPosition = this.playersShiftPosition + this.reservedPlayerBits;
        }

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

                //Split the current layer into an array of lines
                string[] lines = cleanLayer.Split('/');

                //Loop through each line
                foreach (string line in lines)
                {
                    #region Place Rooms
                    PlaceComponents(line, this.roomsStartPosition, this.roomsShiftPosition, x, y, z);
                    #endregion

                    #region Place Pickups
                    PlaceComponents(line, this.pickupsStartPosition, this.pickupsShiftPosition, x, y, z);
                    #endregion

                    #region Place Triggers
                    PlaceComponents(line, this.triggersStartPosition, this.triggersShiftPosition, x, y, z);
                    #endregion

                    #region Place Players
                    PlaceComponents(line, this.playersStartPosition, this.playersShiftPosition, x, y, z);
                    #endregion

                    #region Place Enemies
                    PlaceComponents(line, this.enemiesStartPosition, this.enemiesShiftPosition, x, y, z);
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

        private void PlaceComponents(string line, int index, int shiftPosition, int x, int y, int z)
        {
            //Filter information
            line = line.Split('-')[index].Trim();
            line = line.Replace('|', ' ');
            line = line.Replace(" ", string.Empty);

            //Split the line into an array of cells
            string[] cells = line.Split(',');

            //Loop through each cell
            foreach (string cell in cells)
            {
                //Place room in map
                this.levelMap[x, y, z] += (int.Parse(cell) << shiftPosition);

                //Iterate x
                x++;
            }
        }

        private void InitializeMap(float cellWidth, float cellHeight, float cellDepth, float worldScale)
        {
            #region Calculate Cell Dimensions
            //Calculate the width, height and depth of each cell
            float width = (cellWidth * worldScale);
            float height = (cellHeight * worldScale);
            float depth = (cellDepth * worldScale);
            #endregion

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
                        Transform3D transform = new Transform3D(
                            new Vector3(x * width, y * height, z * depth),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            Vector3.UnitZ,
                            Vector3.UnitY
                        );
                        #endregion

                        #region Construct Rooms
                        //Extract room from level map
                        int roomType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedRoomBits, this.roomsShiftPosition);

                        //If a room has been set
                        if (roomType > 0) 
                            
                            //Construct room
                            ConstructRoom(roomType, transform);
                        #endregion

                        #region Construct Pickups
                        //Extract sound from level map
                        int pickupType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedPickupBits, this.pickupsShiftPosition);

                        //If a pickup has been set
                        if (pickupType > 0)
                            
                            //Construct pickup
                            ConstructPickup(pickupType, transform);
                        #endregion

                        #region Construct Triggers
                        //Extract trigger from level map
                        int triggerType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedTriggerBits, this.triggersShiftPosition);

                        //If a trigger has been set
                        if (triggerType > 0)

                            //Construct trigger
                            ConstructTrigger(triggerType, transform);
                        #endregion

                        #region Spawn Players
                        //Extract player from level map
                        int playerType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedPlayerBits, this.playersShiftPosition);

                        //If a player has been set
                        if (playerType > 0)

                            //Spawn player
                            SpawnPlayer(transform);
                        #endregion

                        #region Spawn Enemies
                        //Extract enemy from level map
                        int enemyType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedEnemyBits, this.enemiesShiftPosition);

                        ////If an enemy has been set
                        if (enemyType > 0)

                            //    //Spawn enemy
                            SpawnEnemy(enemyType, transform);
                        #endregion
                    }
                }
            }
            #endregion
        }

        public void ConstructRoom(int roomType, Transform3D transform)
        {
            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["roomEffect" + roomType];
            Model model = this.modelDictionary["roomModel" + roomType];

            //Load collision box
            Model collisionBox = this.collisionBoxDictionary["roomCollision" + roomType];

            //Create model
            this.staticModel = new TriangleMeshObject(
                "room" + roomType,
                ActorType.CollidableArchitecture,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                model,
                collisionBox,
                new MaterialProperties()
            );

            //Add collision
            this.staticModel.Enable(true, 1);

            //Add to object manager list
            this.object3DManager.Add(staticModel);
        }

        public void ConstructPickup(int pickupType, Transform3D transform)
        {
            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["pickupEffect" + pickupType];
            Model model = this.modelDictionary["pickupModel" + pickupType];

            //Create model
            this.staticModel = new CollidableObject(
                "pickup" + pickupType,
                ActorType.CollidableArchitecture,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                model
            );

            //Add collision
            this.staticModel.AddPrimitive(new JigLibX.Geometry.Box(transform.Translation, Matrix.Identity, new Vector3(254, 254, 254)), new MaterialProperties(0.8f, 0.8f, 0.7f));
            this.staticModel.Enable(true, 1);

            //Add to object manager list
            this.object3DManager.Add(staticModel);
        }

        public void ConstructTrigger(int triggerType, Transform3D transform)
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
        
        public void SpawnPlayer(Transform3D transform)
        {
            this.playerPosition = transform;
        }

        public void SpawnEnemy(int enemyType, Transform3D transform)
        {
        }
        #endregion

        #region Content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadDictionaries();
            LoadAssets();
        }

        protected override void UnloadContent()
        {
        }

        private void LoadDictionaries()
        {
            //Models
            this.modelDictionary = new ContentDictionary<Model>("Model Dictionary", this.Content);

            //Collision Boxes
            this.collisionBoxDictionary = new ContentDictionary<Model>("Collision Box Dictionary", this.Content);

            //Textures
            this.textureDictionary = new ContentDictionary<Texture2D>("Texture Dictionary", this.Content);

            //Sounds
            this.soundEffectDictionary = new ContentDictionary<SoundEffect>("Sound Effect Dictionary", this.Content);

            //Fonts
            this.fontDictionary = new ContentDictionary<SpriteFont>("font dictionary", this.Content);

            //Effect parameters
            this.effectDictionary = new Dictionary<string, EffectParameters>();
        }

        private void LoadAssets()
        {
            #region Models
            LoadModels();
            #endregion

            #region Collision Boxes
            LoadCollisionBoxes();
            #endregion

            #region Textures
            LoadTextures();
            #endregion

            #region Effects
            LoadEffects();
            #endregion

            #region Sounds
            LoadSounds();
            #endregion

            #region Fonts
            LoadFonts();
            #endregion
        }

        public void LoadModels()
        {
            #region Room Models
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
            this.modelDictionary.Load("Assets/Models/Rooms/room_017", "roomModel17");
            this.modelDictionary.Load("Assets/Models/Rooms/room_018", "roomModel18");
            #endregion

            #region Pickup Models
            this.modelDictionary.Load("Assets/Models/Pickups/box", "pickupModel1");
            this.modelDictionary.Load("Assets/Models/Pickups/box", "pickupModel2");
            this.modelDictionary.Load("Assets/Models/Pickups/box", "pickupModel3");
            this.modelDictionary.Load("Assets/Models/Pickups/box", "pickupModel4");
            #endregion

            #region Character Models
            #endregion
        }

        public void LoadCollisionBoxes()
        {
            #region Room Collision
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_001", "roomCollision1");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_002", "roomCollision2");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_003", "roomCollision3");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_004", "roomCollision4");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_005", "roomCollision5");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_006", "roomCollision6");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_007", "roomCollision7");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_008", "roomCollision8");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_009", "roomCollision9");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_010", "roomCollision10");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_011", "roomCollision11");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_012", "roomCollision12");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_013", "roomCollision13");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_014", "roomCollision14");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_015", "roomCollision15");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_016", "roomCollision16");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_017", "roomCollision17");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Rooms/room_collision_018", "roomCollision18");
            #endregion
        }

        public void LoadTextures()
        {
            #region Room Textures
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
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_017", "roomTexture17");
            this.textureDictionary.Load("Assets/Textures/Environment/Rooms/room_texture_018", "roomTexture18");
            #endregion
        }

        public void LoadEffects()
        {
            #region Room Effects
            this.effectDictionary.Add("roomEffect1", new EffectParameters(this.modelEffect, null, Color.Blue, 1));
            this.effectDictionary.Add("roomEffect2", new EffectParameters(this.modelEffect, null, Color.DarkGray, 1));
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
            this.effectDictionary.Add("roomEffect17", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture16"], Color.DarkGray, 1));
            this.effectDictionary.Add("roomEffect18", new EffectParameters(this.modelEffect, this.textureDictionary["roomTexture16"], Color.DarkGray, 1));
            #endregion

            #region Pickup Effects
            this.effectDictionary.Add("pickupEffect1", new EffectParameters(this.modelEffect, null, Color.White, 1));
            this.effectDictionary.Add("pickupEffect2", new EffectParameters(this.modelEffect, null, Color.White, 1));
            this.effectDictionary.Add("pickupEffect3", new EffectParameters(this.modelEffect, null, Color.White, 1));
            this.effectDictionary.Add("pickupEffect4", new EffectParameters(this.modelEffect, null, Color.White, 1));
            #endregion
        }

        public void LoadSounds()
        {
            #region Sound Effects
            this.soundEffectDictionary.Load("Assets/Audio/boing", "boing");
            this.soundEffectDictionary.Load("Assets/Audio/boing", "boing");
            this.soundEffectDictionary.Load("Assets/Audio/boing", "boing");
            this.soundEffectDictionary.Load("Assets/Audio/boing", "boing");
            #endregion

            #region Music
            #endregion
        }

        public void LoadFonts()
        {
            #region Game Fonts
            //this.fontDictionary.Load("hudFont", "Assets/Fonts/hudFont");
            //this.fontDictionary.Load("menu", "Assets/Fonts/menu");
            //this.fontDictionary.Load("debugFont", "Assets/Debug/Fonts/debugFont");
            #endregion
        }
        #endregion

        #region Demos
        private void DemoTriggerVolume()
        {
            foreach (TriggerVolume trigger in this.triggerList)
            {
                //If the actor is within the trigger, and the trigger has not yet fired
                if (trigger.isActorWithin(this.cameraManager.ActiveCamera) && !trigger.HasFired)
                {
                    //Fire the trigger
                    trigger.FireEvent();

                    //Flag the trigger as fired
                    trigger.HasFired = true;
                }

                //If the actor is not within the trigger, reset the 'has fired' flag
                if (!trigger.isActorWithin(this.cameraManager[0]))
                {
                    trigger.HasFired = false;
                    trigger.PauseEvent();
                }
            }
        }

        private void DemoProximityTrigger()
        {

        }
        #endregion

        #region Debug
        #if DEBUG
        private void InitializeDebug()
        {
            Components.Add(
                new DebugDrawer(
                    this,
                    this.cameraManager,
                    this.eventDispatcher,
                    StatusType.Off,
                    this.spriteBatch,
                    null,
                    new Vector2(20, 20),
                    Color.White
                )
            );
        }

        private void InitializeDebugCollisionSkinInfo()
        {
            this.physicsDebugDrawer = new PhysicsDebugDrawer(
                this, 
                this.cameraManager, 
                this.object3DManager,
                this.eventDispatcher, 
                StatusType.Off
            );

            Components.Add(this.physicsDebugDrawer);
        }
        #endif
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