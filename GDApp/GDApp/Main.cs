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
        private PhysicsManager physicsManager;
        private MyMenuManager menuManager;
        private UIManager uiManager;
        private PickingManager pickingManager;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialisation
        protected override void Initialize()
        {
            Window.Title = "Shrine to the Dark God";

            LoadDictionaries();
            LoadAssets();

            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.resolution = ScreenUtility.HD720;
            this.screenCentre = this.resolution / 2;

            InitializeVertices();
            InitializeGraphics();
            InitializeEffects();

            InitializeEventDispatcher();
            InitializeManagers();
            
            float worldScale = 2.54f;
            SetupBitArray(0, 5, 4, 4, 3, 2);

            LoadLevelFromFile();
            LoadMapFromFile();

            InitializeMap(100, 100, 100, worldScale);
            InitializeCameras(worldScale, 1920, 1080);

            InitializeMenu();
            InitializeUI();

            InitializeDebug();
            InitializeDebugCollisionSkinInfo();

            base.Initialize();
        }

        private void InitializeVertices()
        {
            this.vertices = VertexFactory.GetVertexPositionColorTextureQuad();
        }

        private void InitializeGraphics()
        {
            //Set the preferred resolution
            //See https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            this.graphics.PreferredBackBufferWidth = resolution.X;
            this.graphics.PreferredBackBufferHeight = resolution.Y;

            ////Solves the skybox border problem
            //SamplerState samplerState = new SamplerState
            //{
            //    AddressU = TextureAddressMode.Clamp,
            //    AddressV = TextureAddressMode.Clamp
            //};

            //this.graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            ////Enable alpha transparency - see ColorParameters
            //this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.graphics.ApplyChanges();
        }

        private void InitializeEffects()
        {
            BasicEffect basicEffect = null;
            DualTextureEffect dualTextureEffect = null;

            #region Lit objects
            //Create a BasicEffect and set the lighting conditions for all models that use this effect in their EffectParameters field
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = false,
                PreferPerPixelLighting = true,
                FogColor = new Vector3(0.1f, 0.05f, 0.1f),
                FogEnabled = true,
                FogStart = 127,
                FogEnd = 400,
                DiffuseColor = new Vector3(0, 0, 0),
                AmbientLightColor = new Vector3(0.05f, 0, 0.05f),
                EmissiveColor = new Vector3(0.05f, 0, 0.05f)
            };

            basicEffect.EnableDefaultLighting();
            this.effectDictionary.Add(AppData.LitModelsEffectID, new BasicEffectParameters(basicEffect));
            #endregion

            #region For Unlit objects
            //Used for model objects that dont interact with lighting i.e. sky
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = false
            };

            this.effectDictionary.Add(AppData.UnlitModelsEffectID, new BasicEffectParameters(basicEffect));
            #endregion

            #region For dual texture objects
            dualTextureEffect = new DualTextureEffect(graphics.GraphicsDevice);

            this.effectDictionary.Add(
                AppData.UnlitModelDualEffectID,
                new DualTextureEffectParameters(dualTextureEffect)
            );
            //#endregion

            #region Model Effects
            this.modelEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = false,
                LightingEnabled = false
            };

            this.modelEffect.EnableDefaultLighting();
            this.modelEffect.PreferPerPixelLighting = true;

            ////Setup fog
            this.modelEffect.FogColor = new Vector3(0.1f, 0.05f, 0.1f);
            this.modelEffect.FogEnabled = true;
            this.modelEffect.FogStart = 127;
            this.modelEffect.FogEnd = 400;

            //Setup color ambience
            this.modelEffect.DiffuseColor = new Vector3(0, 0, 0);
            this.modelEffect.AmbientLightColor = new Vector3(0.05f, 0, 0.05f);
            this.modelEffect.EmissiveColor = new Vector3(0.05f, 0, 0.05f);
            #endregion

            #region Pickup Effects
            this.pickupEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true
            };

            this.pickupEffect.EnableDefaultLighting();
            this.pickupEffect.PreferPerPixelLighting = true;
            #endregion
        }

        private void InitializeManagers()
        {
            #region Keyboard Manager
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);
            #endregion

            #region Physics Manager
            this.physicsManager = new PhysicsManager(
                this, 
                this.eventDispatcher, 
                StatusType.Off, 
                AppData.BigGravity
            );

            Components.Add(this.physicsManager);
            #endregion

            #region Mouse Manager
            bool bMouseVisible = true;
            this.mouseManager = new MouseManager(
                this, 
                bMouseVisible, 
                this.physicsManager
            );

            this.mouseManager.SetPosition(this.screenCentre);
            Components.Add(this.mouseManager);
            #endregion

            #region Camera Manager
            this.cameraManager = new CameraManager(
                this, 
                2, 
                this.eventDispatcher, 
                StatusType.Off
            );

            Components.Add(this.cameraManager);
            #endregion

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

            #region Picking Manager
            //Use this predicate anytime we want to decide if a mouse over object is interesting to the PickingManager
            Predicate<CollidableObject> collisionPredicate = new Predicate<CollidableObject>(CollisionUtility.IsCollidableObjectOfInterest);
            
            //Listens for picking with the mouse on valid (based on specified predicate) collidable objects and pushes notification events to listeners
            this.pickingManager = new PickingManager(
                this, 
                this.eventDispatcher, 
                StatusType.Off,
                this.managerParameters, 
                this.cameraManager,
                PickingBehaviourType.PickOnly,
                AppData.PickStartDistance, 
                AppData.PickEndDistance, 
                collisionPredicate
            );

            Components.Add(this.pickingManager);
            #endregion

            #region Object Manager
            this.object3DManager = new ObjectManager(
                this, 
                this.cameraManager, 
                this.eventDispatcher, 
                StatusType.Off
            );

            Components.Add(this.object3DManager);
            #endregion
            
            #region Sound Manager
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

            #region Menu Manager
            this.menuManager = new MyMenuManager(
                this, 
                this.managerParameters,
                this.cameraManager, 
                this.spriteBatch, 
                this.eventDispatcher,
                StatusType.Drawn | StatusType.Update
            );

            Components.Add(this.menuManager);
            #endregion

            #region UI Manager
            this.uiManager = new UIManager(
                this,
                this.managerParameters,
                this.cameraManager,
                this.spriteBatch,
                this.eventDispatcher,
                StatusType.Off
            );

            Components.Add(this.uiManager);
            #endregion

            #region Game State Manager
            this.gameStateManager = new GameStateManager(
                this,
                this.eventDispatcher,
                StatusType.Off
            );

            Components.Add(this.gameStateManager);
            #endregion
        }

        private void InitializeEventDispatcher()
        {
            //Initialize with an arbitrary size based on the expected number of events per update cycle, increase/reduce where appropriate
            this.eventDispatcher = new EventDispatcher(this, 20);

            //Dont forget to add to the Component list otherwise EventDispatcher::Update won't get called and no event processing will occur!
            Components.Add(this.eventDispatcher);
        }

        private void InitializeMenu()
        {
            Vector2 position = Vector2.Zero;
            Transform2D transform = null;
            Texture2D texture = null;
            UIButtonObject uiButtonObject = null;
            UIButtonObject clone = null;
            string sceneID = "";
            string buttonID = "";
            string buttonText = "";
            int verticalBtnSeparation = 75;

            #region Main Menu
            sceneID = "main menu";

            //Retrieve the background texture
            texture = this.textureDictionary["mainmenu"];
            
            //Scale the texture to fit the entire screen
            Vector2 scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID, 
                new UITextureObject(
                    "mainmenuTexture",
                    ActorType.UITexture,
                    StatusType.Drawn,       //Notice we dont need to update a static texture
                    transform, 
                    Color.White, 
                    SpriteEffects.None,
                    1,                      //Depth is 1 so its always sorted to the back of other menu elements
                    texture
                )
            );

            //Add start button
            buttonID = "startbtn";
            buttonText = "Start";

            position = new Vector2(
                graphics.PreferredBackBufferWidth / 4.0f, 
                300
            );

            texture = this.textureDictionary["genericbtn"];

            transform = new Transform2D(
                position,
                0, 
                new Vector2(1.8f, 1f),
                new Vector2(texture.Width / 2.0f, 
                texture.Height / 2.0f), 
                new Integer2(texture.Width, texture.Height)
            );

            uiButtonObject = new UIButtonObject(
                buttonID, 
                ActorType.UIButton, 
                StatusType.Update | StatusType.Drawn,
                transform, 
                Color.White, 
                SpriteEffects.None, 
                0.1f, 
                texture, 
                buttonText,
                this.fontDictionary["menu"],
                Color.Black, 
                new Vector2(0, 2)
            );

            this.menuManager.Add(sceneID, uiButtonObject);

            //Add audio button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "audiobtn";
            clone.Text = "Audio";
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, verticalBtnSeparation);

            //Change the texture blend color
            clone.Color = Color.LightGreen;
            this.menuManager.Add(sceneID, clone);

            //Add controls button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "controlsbtn";
            clone.Text = "Controls";

            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);

            //Change the texture blend color
            clone.Color = Color.LightBlue;
            this.menuManager.Add(sceneID, clone);

            //Add exit button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "exitbtn";
            clone.Text = "Exit";

            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);

            //Change the texture blend color
            clone.Color = Color.LightYellow;
            
            //Store the original color since if we modify with a controller and need to reset
            clone.OriginalColor = clone.Color;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Audio Menu
            sceneID = "audio menu";

            //Retrieve the audio menu background texture
            texture = this.textureDictionary["audiomenu"];
            
            //Scale the texture to fit the entire screen
            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID, 
                new UITextureObject("audiomenuTexture",
                    ActorType.UITexture,
                    StatusType.Drawn,       //Notice we dont need to update a static texture
                    transform, 
                    Color.White, 
                    SpriteEffects.None,
                    1,                      //Depth is 1 so its always sorted to the back of other menu elements
                    texture
                )
            );

            //Add volume up button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "volumeUpbtn";
            clone.Text = "Volume Up";
            
            //Change the texture blend color
            clone.Color = Color.LightPink;
            this.menuManager.Add(sceneID, clone);

            //Add volume down button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, verticalBtnSeparation);
            clone.ID = "volumeDownbtn";
            clone.Text = "Volume Down";

            //Change the texture blend color
            clone.Color = Color.LightGreen;
            this.menuManager.Add(sceneID, clone);

            //Add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            clone.ID = "volumeMutebtn";
            clone.Text = "Volume Mute";
            
            //Change the texture blend color
            clone.Color = Color.LightBlue;
            this.menuManager.Add(sceneID, clone);

            //Add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            clone.ID = "volumeUnMutebtn";
            clone.Text = "Volume Un-mute";
            
            //Change the texture blend color
            clone.Color = Color.LightSalmon;
            this.menuManager.Add(sceneID, clone);

            //Add back button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 4 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            
            //Change the texture blend color
            clone.Color = Color.LightYellow;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Controls Menu
            sceneID = "controls menu";

            //Retrieve the controls menu background texture
            texture = this.textureDictionary["controlsmenu"];

            //Scale the texture to fit the entire screen
            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID, 
                new UITextureObject(
                    "controlsmenuTexture", 
                    ActorType.UITexture,
                    StatusType.Drawn, //notice we dont need to update a static texture
                    transform, 
                    Color.White, 
                    SpriteEffects.None,
                    1, //depth is 1 so its always sorted to the back of other menu elements
                    texture
                )
            );

            //Add back button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 9 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            
            //Change the texture blend color
            clone.Color = Color.LightYellow;
            this.menuManager.Add(sceneID, clone);
            #endregion
        }

        private void InitializeUI()
        {
            Transform2D transform = new Transform2D(
                new Vector2(100, 200),
                0,
                Vector2.One,
                Vector2.Zero,
                new Integer2(100, 25)
            );

            UITextObject uiTextObject = new UITextObject("Greeting", ActorType.UIText, StatusType.Update | StatusType.Drawn, transform, Color.Red, SpriteEffects.None, 1, "Hello World", this.fontDictionary["menu"]);
            this.uiManager.Add("main_ui", uiTextObject);
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
            BasicEffectParameters effectParameters = this.effectDictionary[AppData.LitModelsEffectID].Clone() as BasicEffectParameters;
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
            BasicEffectParameters effectParameters = this.effectDictionary[AppData.LitModelsEffectID].Clone() as BasicEffectParameters;
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

            #region Generic Textures
            this.textureDictionary.Load("Assets/Textures/Props/Crates/crate1");
            this.textureDictionary.Load("Assets/Textures/Props/Crates/crate2");
            this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass1");
            this.textureDictionary.Load("Assets/Textures/Skybox/back");
            this.textureDictionary.Load("Assets/Textures/Skybox/left");
            this.textureDictionary.Load("Assets/Textures/Skybox/right");
            this.textureDictionary.Load("Assets/Textures/Skybox/sky");
            this.textureDictionary.Load("Assets/Textures/Skybox/front");
            this.textureDictionary.Load("Assets/Textures/Foliage/Trees/tree2");
            #endregion

            #region Dual Textures
            this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass_midlevel");
            this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass_highlevel");
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard_greywhite");
            #endregion

            #region Menu Buttons
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Buttons/genericbtn");
            #endregion

            #region Menu Backgrounds
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/mainmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/audiomenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/controlsmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/exitmenuwithtrans");
            #endregion

            #region UI Elements
            this.textureDictionary.Load("Assets/Textures/UI/HUD/reticuleDefault");
            this.textureDictionary.Load("Assets/Textures/UI/HUD/progress_gradient");
            #endregion

            #region Architecture
            this.textureDictionary.Load("Assets/Textures/Architecture/Buildings/house-low-texture");
            this.textureDictionary.Load("Assets/Textures/Architecture/Walls/wall");
            #endregion

            #region Debug
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard");
            this.textureDictionary.Load("Assets/Debug/Textures/ml");
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard");
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
            this.fontDictionary.Load("Assets/Fonts/hudFont", "hudFont");
            this.fontDictionary.Load("Assets/Fonts/menu", "menu");
            this.fontDictionary.Load("Assets/Debug/Fonts/debugFont", "debugFont");
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
            Vector3 translation = this.cameraManager.ActiveCamera.Transform.Translation;
        }

        private void DemoToggleMenu()
        {
            if (this.keyboardManager.IsFirstKeyPress(AppData.MenuShowHideKey))
            {
                if (this.menuManager.IsVisible)
                    EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                else
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
            }
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
                    this.fontDictionary["debugFont"],
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
            DemoToggleMenu();
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