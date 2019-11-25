using GDLibrary;
using JigLibX.Geometry;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;

namespace GDApp
{
    public class Main : Game
    {
        //Graphics
        private Integer2 resolution;
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;

        //Vertices
        private VertexPositionColorTexture[] vertices;

        //Managers
        private StateManager gameStateManager;
        private ManagerParameters managerParameters;
        private GridManager gridManager;
        private ObjectManager objectManager;
        private CameraManager cameraManager;
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private SoundManager soundManager;
        private CombatManager combatManager;
        private PhysicsManager physicsManager;
        private MyMenuManager menuManager;
        private MyUIManager uiManager;
        private MyTextboxManager textboxManager;
        private PickingManager pickingManager;
        private InventoryManager inventoryManager;

        //Dispatchers
        private EventDispatcher eventDispatcher;

        //Vectors
        private Vector2 screenCentre;

        //Models
        private CollidableObject collidableModel;
        private ModelObject staticModel;

        //Dictionaries
        private ContentDictionary<Model> modelDictionary;
        private ContentDictionary<Model> collisionBoxDictionary;
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SoundEffect> soundEffectDictionary;
        private ContentDictionary<SpriteFont> fontDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;
        private Dictionary<string, PickupParameters> pickupParametersDictionary;
        private Dictionary<string, EnemyObject> enemyDictionary;

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
        private int gatesStartPosition = 6;

        //Array Shift Position
        private int roomsShiftPosition;
        private int pickupsShiftPosition;
        private int triggersShiftPosition;
        private int playersShiftPosition;
        private int enemiesShiftPosition;
        private int gatesShiftPosition;

        //Array Reserved Bits
        private int reservedRoomBits;
        private int reservedPickupBits;
        private int reservedTriggerBits;
        private int reservedPlayerBits;
        private int reservedEnemyBits;
        private int reservedGateBits;

        //Player Posiiton
        private Transform3D playerPosition;
        private Transform3D uiPosition;
        private PlayerObject player;
        private List<EnemyObject> enemies;
        private BasicEffect torchLitRoomEffect;
        private BasicEffect standardRoomEffect;
        private BasicEffect pickupEffect;
        private BasicEffect gateEffect;
        private BasicEffect enemyEffect;

        private ProjectionParameters projectionParameters;

        private Viewport viewport;
        private float depth;
        private string textboxText;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialisation
        protected override void Initialize()
        {
            Window.Title = "Shrine to the Dark God";

            InitializeVertices();

            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.resolution = ScreenUtility.WXGA;
            this.screenCentre = this.resolution / 2;

            InitializeGraphics();
            InitializeEffects();
            InitializeEnemies();

            InitializeEventDispatcher();
            InitializeManagers();

            LoadContent();
            
            float worldScale = 2.54f;
            SetupBitArray(0, 5, 4, 4, 3, 2, 3);

            LoadLevelFromFile();
            LoadMapFromFile();

            InitializeMap(100, 100, 100, worldScale);

            InitializeMenu();
            InitializeTextbox();
            InitializeUI();

            InitializeGrid();
            base.Initialize();
        }

        private void InitializeGrid()
        {
            EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
        }

        private void InitializeVertices()
        {
            this.vertices = VertexFactory.GetVertexPositionColorTextureQuad();
        }

        private void InitializeGraphics()
        {
            this.graphics.PreferredBackBufferWidth = resolution.X;
            this.graphics.PreferredBackBufferHeight = resolution.Y;
            this.graphics.ApplyChanges();
        }

        private void InitializeEffects()
        {
            BasicEffect basicEffect;

            Vector3 fogColor = new Vector3(0f, 0f, 0f);
            float fogStart = 0;
            float fogEnd = 550;

            #region Standard Room Effect
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                FogEnabled = true,
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                FogColor = fogColor,
                FogStart = fogStart,
                FogEnd = fogEnd
            };

            //Standard Light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(-0.5f, -0.75f, -0.5f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.4f, 0.3f);

            //Standard Light
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.Direction = new Vector3(0.5f, 0f, 0.5f);
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.4f, 0.3f);

            //Add to dictionary
            this.standardRoomEffect = basicEffect;
            #endregion

            #region Room With Torch Effect
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                FogEnabled = true,
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                FogColor = fogColor,
                FogStart = fogStart,
                FogEnd = fogEnd
            };

            //Standard Light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(-0.5f, -0.75f, -0.5f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.85f, 0.75f, 0.65f);

            //Standard Light
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.Direction = new Vector3(0.5f, 0.75f, 0.5f);
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.85f, 0.75f, 0.65f);

            //Torch Light
            basicEffect.DirectionalLight2.Enabled = true;
            basicEffect.DirectionalLight2.DiffuseColor = Color.DarkOrange.ToVector3();
            basicEffect.DirectionalLight2.Direction = new Vector3(0, 1f, 0);

            //Add to dictionary
            this.torchLitRoomEffect = basicEffect;
            #endregion

            #region Pickup Effect
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                FogEnabled = true,
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                FogColor = fogColor,
                FogStart = fogStart,
                FogEnd = fogEnd
            };

            //Standard Light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(-0f, -1f, -0f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.85f, 0.85f, 0.65f);

            //Add to dictionary
            this.pickupEffect = basicEffect;
            #endregion

            #region Pickup Effect
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                FogEnabled = true,
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                FogColor = fogColor,
                FogStart = fogStart,
                FogEnd = fogEnd
            };

            //Standard Light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(-0.5f, -0.75f, -0.5f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.85f, 0.75f, 0.65f);

            //Standard Light
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.Direction = new Vector3(0.5f, 0.75f, 0.5f);
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.85f, 0.75f, 0.65f);

            //Add to dictionary
            this.gateEffect = basicEffect;

            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                FogEnabled = true,
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                FogColor = fogColor,
                FogStart = fogStart,
                FogEnd = fogEnd
            };

            //Standard Light
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(-0.5f, -0.75f, -0.5f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);

            //Standard Light
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.DirectionalLight1.Direction = new Vector3(0.5f, 0.75f, 0.5f);
            basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.85f, 0.75f, 0.65f);

            this.enemyEffect = basicEffect;
            #endregion
        }

        private void InitializeManagers()
        {
            #region Camera Manager
            this.cameraManager = new CameraManager(
                this, 
                2, 
                this.eventDispatcher, 
                StatusType.Off
            );

            Components.Add(this.cameraManager);
            #endregion

            #region Object Manager
            this.objectManager = new ObjectManager(
                this, 
                this.cameraManager, 
                this.eventDispatcher, 
                StatusType.Off,
                new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)
            );

            Components.Add(this.objectManager);
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

            #region Keyboard Manager
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);
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
            
            #region State Manager
            this.gameStateManager = new StateManager(
                this,
                this.eventDispatcher,
                StatusType.Off
            );

            Components.Add(this.gameStateManager);
            #endregion

            #region Inventory Manager
            this.inventoryManager = new InventoryManager(
                this,
                this.eventDispatcher,
                StatusType.Off
                );

            Components.Add(this.inventoryManager);
            #endregion

            #region Combat Manager
            this.combatManager = new CombatManager(
                this, 
                this.eventDispatcher, 
                StatusType.Update, 
                this.inventoryManager,
                this.keyboardManager,
                this.objectManager,
                this.gridManager,
                AppData.CombatKeys
            );

            Components.Add(this.combatManager);
            #endregion

            #region Menu Manager
            this.menuManager = new MyMenuManager(
                this,
                StatusType.Drawn | StatusType.Update,
                this.eventDispatcher,
                this.cameraManager,
                this.mouseManager,
                this.spriteBatch
            );

            Components.Add(this.menuManager);
            #endregion

            #region UI Manager
            this.uiManager = new MyUIManager(
                this,
                this.managerParameters,
                this.spriteBatch,
                this.eventDispatcher,
                StatusType.Off
            );

            Components.Add(this.uiManager);
            #endregion

            #region textbox Manager
            this.textboxManager = new MyTextboxManager(
                this,
                this.managerParameters,
                this.spriteBatch,
                this.eventDispatcher,
                StatusType.Update | StatusType.Drawn,
                this.textboxText
            );

            Components.Add(this.textboxManager);
            #endregion

            #region Manager Parameters
            this.managerParameters = new ManagerParameters(
                this.objectManager,
                this.cameraManager,
                this.mouseManager,
                this.keyboardManager,
                this.gamePadManager,
                this.soundManager,
                this.physicsManager,
                this.inventoryManager,
                this.combatManager,
                this.uiManager,
                this.textboxManager
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

            #region Grid Manager
            this.gridManager = new GridManager(
                this,
                this.eventDispatcher,
                StatusType.Update,
                new HashSet<Actor3D>(),
                new HashSet<Actor3D>(),
                new HashSet<Actor3D>(),
                new HashSet<Actor3D>(),
                new HashSet<Actor3D>(),
                this.objectManager,
                this.soundManager,
                this.inventoryManager,
                this.combatManager
            );

            Components.Add(this.gridManager);
            #endregion

           #region Draw order
           this.objectManager.DrawOrder = 1;
           this.uiManager.DrawOrder = 2;
           this.textboxManager.DrawOrder = 3;
           this.menuManager.DrawOrder = 4;
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
            int verticalBtnSeparation = 55;

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
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add controls button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "controlsbtn";
            clone.Text = "Controls";

            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);

            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add exit button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "exitbtn";
            clone.Text = "Exit";

            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);

            //Change the texture blend color
            clone.Color = Color.White;
            
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
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add volume down button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, verticalBtnSeparation);
            clone.ID = "volumeDownbtn";
            clone.Text = "Volume Down";

            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            clone.ID = "volumeMutebtn";
            clone.Text = "Volume Mute";
            
            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            clone.ID = "volumeUnMutebtn";
            clone.Text = "Volume Un-mute";
            
            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Add back button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            
            //Move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 4 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            
            //Change the texture blend color
            clone.Color = Color.White;
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
            clone.Transform.Translation += new Vector2(700, 7 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            
            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Begin Menu
            sceneID = "begin menu";

            //Retrieve the controls menu background texture
            texture = this.textureDictionary["begingame"];

            //Scale the texture to fit the entire screen
            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "beginmenuTexture",
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
            clone.Transform.Translation += new Vector2(700, 7 * verticalBtnSeparation);
            clone.ID = "beginbtn";
            clone.Text = "Begin";

            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Win Screen
            sceneID = "win menu";

            //Retrieve the controls menu background texture
            texture = this.textureDictionary["wingame"];

            //Scale the texture to fit the entire screen
            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "wingameTexture",
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
            clone.Transform.Translation += new Vector2(000, 4 * verticalBtnSeparation);
            clone.ID = "menubtn";
            clone.Text = "Return to Menu";

            //Change the texture blend color
            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Lose Screen
            sceneID = "lose menu";

            //Retrieve the controls menu background texture
            texture = this.textureDictionary["losegame"];

            //Scale the texture to fit the entire screen
            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "losegameTexture",
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
            clone.Transform.Translation += new Vector2(0, 4 * verticalBtnSeparation);
            clone.ID = "menubtn";
            clone.Text = "Return to Menu";

            //Change the texture blend color
            clone.Color = Color.White;
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

            Texture2D texture = this.textureDictionary["HUD"];
            string sceneID = "UI";
            DrawnActor2D text;

            //Scale the texture to fit the entire screen
            Vector2 scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            transform = new Transform2D(scale);
            text = new UITextureObject(
                "uiTexture",
                ActorType.UITexture,
                StatusType.Drawn,       //Notice we dont need to update a static texture
                transform,
                Color.White,
                SpriteEffects.None,
                0,                      //Depth is 1 so its always sorted to the back of other menu elements
                texture
            );

            this.uiManager.Add(sceneID, text);

            //AddUICamera(
            //    new Transform3D(
            //        new Vector3(400, 400, 400),
            //        Vector3.Zero,
            //        Vector3.One,
            //        Vector3.Forward,
            //        Vector3.Up
            //    )
            //);
        }

        private void InitializeTextbox()
        {
            #region Text box
            string sceneID = "TextboxID";
            this.textboxText = this.textboxManager.TextboxText;

            Transform2D transformtext = new Transform2D(
                new Vector2(940, 40),
                0,
                new Vector2(.6f, .6f),
                Vector2.Zero,
                new Integer2(100, 25)
            );

            UITextObject uiTextObj = new UITextObject(
                sceneID,
                ActorType.UIText,
                StatusType.Drawn,
                transformtext,
                Color.Cyan,
                SpriteEffects.None,
                0,
                this.textboxText,
                this.fontDictionary["menu"]
            );

            this.textboxManager.Add("textbox", uiTextObj);
            #endregion
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
            depth = 0;

            
        }
        #endregion

        private void AddFirstPersonCamera(Transform3D playerTransform)
        {
            //Setup projection parameters
            ProjectionParameters projectionParameters = new ProjectionParameters(
                AppData.FirstPersonCameraFieldOfView,
                AppData.FirstPersonCameraAspectRatio,
                AppData.FirstPersonCameraNearClipPlane,
                AppData.FirstPersonCameraFarClipPlane
            );

            //Setup viewport and draw depth
            Viewport viewport = new Viewport(10, 10, graphics.PreferredBackBufferWidth - 400, graphics.PreferredBackBufferHeight- 250);
            float drawDepth = 0;

            //Create a camera
            Camera3D camera = new Camera3D(
                "First Person Camera",
                ActorType.CollidableCamera,
                StatusType.Update,
                playerTransform,
                projectionParameters,
                viewport,
                drawDepth
            );

            //Attacha first person camera controller
            camera.AttachController(
                new CollidableFirstPersonCameraController(
                    camera + " Controller",
                    ControllerType.CollidableCamera,
                    AppData.CameraMoveKeys,
                    AppData.CameraMoveSpeed,
                    AppData.CameraRotationSpeed,
                    this.managerParameters,
                    AppData.CharacterMovementVector,
                    AppData.CharacterRotationVector
                )
            );

            //Add to lists
            this.cameraManager.Add(camera);
        }

        private void AddUICamera(Transform3D uiTransform)
        {
            this.uiPosition.Translation += new Vector3(400, 400, 400);
            
            //Setup projection parameters
            ProjectionParameters projectionParameters = new ProjectionParameters(
                AppData.FirstPersonCameraFieldOfView,
                AppData.FirstPersonCameraAspectRatio,
                AppData.FirstPersonCameraNearClipPlane,
                AppData.FirstPersonCameraFarClipPlane
            );

            viewport = new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            float drawDepth = 0f;

            Camera3D camera = new Camera3D(
                "UICam",
                ActorType.CollidableCamera,
                StatusType.Update,
                this.uiPosition,
                projectionParameters,
                viewport,
                drawDepth
            );

            camera.AttachController(
                new FirstPersonCameraController(
                    camera + " Controller",
                    ControllerType.CollidableCamera,
                    AppData.CameraMoveKeys,
                    AppData.CameraMoveSpeed,
                    AppData.CameraRotationSpeed,
                    this.managerParameters,
                    AppData.CharacterMovementVector,
                    AppData.CharacterRotationVector
                )
            );
            
            this.cameraManager.Add(camera);
            //this.combatManager.AddPlayer((camera.ControllerList[0] as CollidableFirstPersonCameraController).PlayerObject);
        }
        #endregion
        
        #region Map Setup
        private void SetupBitArray(int roomsShiftPosition, int reservedRoomBits, int reservedPickupBits, int reservedTriggerBits, int reservedPlayerBits, int reservedEnemyBits, int reservedGateBits)
        {
            //Reserve bits for each map component
            this.reservedRoomBits = reservedRoomBits;
            this.reservedPickupBits = reservedPickupBits;
            this.reservedTriggerBits = reservedTriggerBits;
            this.reservedPlayerBits = reservedPlayerBits;
            this.reservedEnemyBits = reservedEnemyBits;
            this.reservedGateBits = reservedGateBits;

            //Calculate shift for each map component, relative to previous component
            this.roomsShiftPosition = roomsShiftPosition;
            this.pickupsShiftPosition = this.roomsShiftPosition + this.reservedRoomBits;
            this.triggersShiftPosition = this.pickupsShiftPosition + this.reservedPickupBits;
            this.playersShiftPosition = this.triggersShiftPosition + this.reservedTriggerBits;
            this.enemiesShiftPosition = this.playersShiftPosition + this.reservedPlayerBits;
            this.gatesShiftPosition = this.enemiesShiftPosition + this.reservedEnemyBits;
        }

        private void LoadLevelFromFile()
        {
            if (File.Exists("App/Data/currentLevel.txt"))
                StateManager.CurrentLevel = int.Parse(File.ReadAllText("App/Data/currentLevel.txt"));
            else
                StateManager.CurrentLevel = 1;
        }

        private void WriteLevelToFile()
        {
            File.WriteAllText("App/Data/mapData.txt", StateManager.CurrentLevel.ToString());
        }

        private void LoadMapFromFile()
        {
            #region Reset
            //Reset map
            this.objectManager.Clear();
            #endregion

            #region Read File
            //Store all file data
            string fileText = File.ReadAllText("App/Data/mapData.txt");

            //Split the file into an array of levels
            string[] levels = fileText.Split('*');

            //Split the current level into an array of layers (y axis)
            string[] layers = levels[StateManager.CurrentLevel].Split('&');
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

                    #region Place Gates
                    PlaceComponents(line, this.gatesStartPosition, this.gatesShiftPosition, x, y, z);
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
                            -Vector3.UnitZ,
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
                            SpawnPlayer(playerType, transform);
                        #endregion

                        #region Spawn Enemies
                        //Extract enemy from level map
                        int enemyType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedEnemyBits, this.enemiesShiftPosition);

                        //If an enemy has been set
                        if (enemyType > 0)

                            //Spawn enemy
                            SpawnEnemy(enemyType, transform);
                        #endregion

                        #region Construct Gates
                        //Extract gate from level map
                        int gateType = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedGateBits, this.gatesShiftPosition);

                        //If a gate has been set
                        if (gateType > 0)

                            //Construct gate
                            ConstructGate(gateType, transform);
                        #endregion
                    }
                }
            }
            #endregion
        }

        public void ConstructRoom(int roomType, Transform3D transform)
        {
            //Setup dimensions
            Transform3D roomTransform = transform.Clone() as Transform3D;

            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["roomEffect" + roomType];
            Model model = this.modelDictionary["roomModel" + roomType];

           
            //Load collision box
            Model collisionBox = this.collisionBoxDictionary["roomCollision" + roomType];

            //Create model
            this.collidableModel = new CollidableArchitecture(
                "Room " + roomType,
                ActorType.CollidableArchitecture,
                roomTransform,
                effectParameters,
                model,
                collisionBox,
                new MaterialProperties()
            );

            //Add collision
            this.collidableModel.Enable(true, 1);

            //Add to object manager list
            this.objectManager.Add(collidableModel);
        }

        public void ConstructPickup(int pickupType, Transform3D transform)
        {
            //Setup dimensions
            Transform3D pickupTransform = transform.Clone() as Transform3D;
            pickupTransform.Translation += new Vector3(127, 127, 127);
            
            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["pickupEffect" + pickupType];
            Model model = this.modelDictionary["pickupModel" + pickupType];

            //Load collision box
            Model collisionBox = this.collisionBoxDictionary["pickupCollision"];

            //Select pickup parameters
            PickupParameters pickupParameters = SelectPickupParameters(pickupType);

            //Create model
            this.collidableModel = new ImmovablePickupObject(
                "Pickup " + pickupType,
                ActorType.CollidablePickup,
                pickupTransform,
                effectParameters,
                model,
                collisionBox,
                new MaterialProperties(),
                pickupParameters
            );

            this.collidableModel.AttachController(new SpinController("pickupSpin" + pickupType, ControllerType.Spin, 1));

            //Add collision
            this.collidableModel.Enable(true, 1);

            //Add to lists
            this.objectManager.Add(collidableModel);
            this.gridManager.Add(collidableModel);
        }

        PickupParameters SelectPickupParameters(int pickupType)
        {
            switch (pickupType)
            {
                case 1:
                    return this.pickupParametersDictionary["sword"];
                case 2:
                    return this.pickupParametersDictionary["key"];
                case 3:
                    return this.pickupParametersDictionary["potion"];
                default:
                    return null;
            }
        }

        public void ConstructTrigger(int triggerType, Transform3D transform)
        {
            Transform3D triggerTransform = transform.Clone() as Transform3D;

            //Determine trigger type
            switch (triggerType)
            {
                case 1:
                    //Create win trigger
                    triggerTransform.Translation += new Vector3(127, 127, 127);

                    this.collidableModel = new ZoneObject(
                        "Win Zone",
                        ActorType.Trigger,
                        triggerTransform,
                        this.effectDictionary["roomEffect1"],
                        null
                    );

                    this.collidableModel.AddPrimitive(new Capsule(Vector3.Zero, Matrix.CreateRotationX(MathHelper.PiOver2), 77, 77), new MaterialProperties());

                    //Enable collision
                    this.collidableModel.Enable(true, 1);

                    //Add to lists
                    this.objectManager.Add(this.collidableModel);
                    this.gridManager.Add(this.collidableModel);
                    break;

                default:
                    break;
            }
        }
        
        public void ConstructGate(int gateType, Transform3D transform)
        {
            //Setup dimensions
            Transform3D gateTransform = transform.Clone() as Transform3D;
            gateTransform.Translation += new Vector3(127, 127, 127);

            //Load model and effect parameters
            EffectParameters effectParameters = this.effectDictionary["propEffect" + gateType];
            Model model = this.modelDictionary["gateModel" + gateType];

            //Load collision box
            Model collisionBox = this.collisionBoxDictionary["gateCollision" + gateType];

            //Create model
            this.collidableModel = new CollidableArchitecture(
                "Gate " + gateType,
                ActorType.Gate,
                gateTransform,
                effectParameters,
                model,
                collisionBox,
                new MaterialProperties()
            );

            //Add collision
            this.collidableModel.Enable(true, 1);

            //Add to object manager list
            this.objectManager.Add(collidableModel);
            this.gridManager.Add(this.collidableModel);
        }

        public void SpawnPlayer(int playerType, Transform3D transform)
        {
            //Position player
            this.playerPosition = transform.Clone() as Transform3D;
            this.playerPosition.Translation += new Vector3(127, 127, 127);
            this.uiPosition = transform.Clone() as Transform3D;

            //Create first person camera
            AddFirstPersonCamera(this.playerPosition);
            AddUICamera(this.uiPosition);

            //Create a player
            this.collidableModel = new PlayerObject(
                "Player" + playerType,
                ActorType.Player,
                playerPosition,
                null,
                null,
                AppData.CharacterAccelerationRate,
                AppData.CharacterDecelerationRate,
                AppData.CharacterMovementVector,
                AppData.CharacterRotationVector,
                AppData.CharacterMoveSpeed,
                AppData.CharacterRotateSpeed,
                AppData.PlayerHealth,
                AppData.PlayerAttack,
                AppData.PlayerDefence,
                this.managerParameters,
                AppData.CameraMoveKeys
            );

            //Enable collision
            this.collidableModel.Enable(true, 1);

            //Add to lists
            this.gridManager.Add(this.collidableModel);
            this.combatManager.AddPlayer(this.collidableModel as PlayerObject);
            this.objectManager.Add(this.collidableModel);
        }
        
        public void SpawnEnemy(int enemyType, Transform3D transform)
        {   
            //Select enemy
            switch (enemyType) {
                case 1:
                    this.collidableModel = this.enemyDictionary["Skeleton"];
                    break;
                case 2:
                    this.collidableModel = this.enemyDictionary["Cultist"];
                    break;
            }

            //Position enemy
            this.collidableModel.Transform = transform.Clone() as Transform3D;
            this.collidableModel.Transform.Translation += new Vector3(127, 127, 127);

            //Enable collision
            this.collidableModel.Enable(true, 1);
            
            //Add to lists
            this.gridManager.Add(this.collidableModel);
            this.enemies.Add(this.collidableModel as EnemyObject);
            this.combatManager.PopulateEnemies(this.collidableModel as EnemyObject);
            this.objectManager.Add(this.collidableModel);
        }

        private void InitializeEnemies()
        {
            this.enemies = new List<EnemyObject>();
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
            this.fontDictionary = new ContentDictionary<SpriteFont>("Font Dictionary", this.Content);

            //Effect parameters
            this.effectDictionary = new Dictionary<string, EffectParameters>();

            //Pickup parameters
            this.pickupParametersDictionary = new Dictionary<string, PickupParameters>();

            //Enemies
            this.enemyDictionary = new Dictionary<string, EnemyObject>();
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

            #region Fonts
            LoadFonts();
            #endregion

            #region Pickup Parameters
            LoadPickupParameters();
            #endregion

            #region Enemies
            LoadEnemies();
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
            this.modelDictionary.Load("Assets/Models/Pickups/sword", "pickupModel1");
            this.modelDictionary.Load("Assets/Models/Pickups/key", "pickupModel2");
            this.modelDictionary.Load("Assets/Models/Pickups/potion", "pickupModel3");
            #endregion

            #region Character Models
            this.modelDictionary.Load("Assets/Models/Characters/enemy_001", "skeletonModel");
            this.modelDictionary.Load("Assets/Models/Characters/enemy_002", "cultistModel");
            #endregion

            #region Prop Models
            this.modelDictionary.Load("Assets/Models/Props/gate_001", "gateModel1");
            this.modelDictionary.Load("Assets/Models/Props/gate_002", "gateModel2");
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

            #region Pickup Collision
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Pickups/pickup_collision", "pickupCollision");
            #endregion

            #region Enemy Collision
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Characters/enemy_collision", "enemyCollision");
            #endregion

            #region Zone Collision
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Zones/zone_collision", "zoneCollision");
            #endregion

            #region Prop Collision
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Props/gate_collision_001", "gateCollision1");
            this.collisionBoxDictionary.Load("Assets/Collision Boxes/Props/gate_collision_002", "gateCollision2");
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
            this.textureDictionary.Load("Assets/Textures/Props/Gates/gate_texture_001", "gateTexture1");
            this.textureDictionary.Load("Assets/Textures/Props/Gates/gate_texture_002", "gateTexture2");
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
            #endregion

            #region Menu Buttons
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Buttons/genericbtn");
            #endregion

            #region Menu Backgrounds
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/mainmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/audiomenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/controlsmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/begingame");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/wingame");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/losegame");
            #endregion

            #region UI Elements
            this.textureDictionary.Load("Assets/Textures/UI/HUD/reticuleDefault");
            this.textureDictionary.Load("Assets/Textures/UI/HUD/progress_gradient");
            this.textureDictionary.Load("Assets/Textures/UI/HUD/HUD");
            #endregion

            #region Architecture
            this.textureDictionary.Load("Assets/Textures/Architecture/Buildings/house-low-texture");
            this.textureDictionary.Load("Assets/Textures/Architecture/Walls/wall");
            #endregion
        }

        public void LoadEffects()
        {
            #region Room Effects
            this.effectDictionary.Add("roomEffect1", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture1"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect2", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture2"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect3", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture3"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect4", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture4"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect5", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture5"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect6", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture6"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect7", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture7"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect8", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture8"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect9", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture9"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect18", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture10"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect11", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture11"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect12", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture12"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect13", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture13"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect14", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture14"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect15", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture15"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect16", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture16"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect17", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture17"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("roomEffect10", new BasicEffectParameters(this.standardRoomEffect, this.textureDictionary["roomTexture18"], new Color(new Vector3(0.52f, 0.45f, 0.37f)), Color.Black, Color.Black, Color.Black, 0, 1));
            #endregion

            #region Pickup Effects
            this.effectDictionary.Add("pickupEffect1", new BasicEffectParameters(this.pickupEffect, null, new Color(new Vector3(1.0f, 1.0f, 1.0f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("pickupEffect2", new BasicEffectParameters(this.pickupEffect, null, new Color(new Vector3(1.0f, 0.7f, 0.0f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("pickupEffect3", new BasicEffectParameters(this.pickupEffect, null, new Color(new Vector3(1.0f, 0.2f, 0.2f)), Color.Black, Color.Black, Color.Black, 0, 1));
            #endregion

            #region Gate Effects
            this.effectDictionary.Add("propEffect1", new BasicEffectParameters(this.gateEffect, null, new Color(new Vector3(0.9f, 0.6f, 0.3f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("propEffect2", new BasicEffectParameters(this.gateEffect, null, new Color(new Vector3(0.9f, 0.6f, 0.3f)), Color.Black, Color.Black, Color.Black, 0, 1));
            #endregion

            #region Enemy Effects
            this.effectDictionary.Add("skeletonEffect", new BasicEffectParameters(this.enemyEffect, null, new Color(new Vector3(0.3f, 0.2f, 0.1f)), Color.Black, Color.Black, Color.Black, 0, 1));
            this.effectDictionary.Add("cultistEffect", new BasicEffectParameters(this.enemyEffect, null, new Color(new Vector3(0.0f, 0.0f, 0.0f)), Color.Black, Color.Black, Color.Black, 0, 1));
            #endregion
        }

        public void LoadFonts()
        {
            #region Game Fonts
            this.fontDictionary.Load("Assets/Fonts/hudFont", "hudFont");
            this.fontDictionary.Load("Assets/Fonts/menu", "menu");
            #endregion
        }

        public void LoadPickupParameters()
        {
            this.pickupParametersDictionary.Add("sword", new PickupParameters("Steel Sword", 1, PickupType.Sword));
            this.pickupParametersDictionary.Add("key", new PickupParameters("Gate Key", 2, PickupType.Key));
            this.pickupParametersDictionary.Add("potion", new PickupParameters("Health Potion", 3, PickupType.Health));
        }

        public void LoadEnemies()
        {
            this.enemyDictionary.Add(
                "Skeleton",
                new EnemyObject(
                    "Skeleton",
                    ActorType.Enemy,
                    Transform3D.Zero,
                    this.effectDictionary["skeletonEffect"],
                    this.modelDictionary["skeletonModel"],
                    AppData.CharacterAccelerationRate,
                    AppData.CharacterDecelerationRate,
                    AppData.CharacterMovementVector,
                    AppData.CharacterRotationVector,
                    AppData.CharacterMoveSpeed,
                    AppData.CharacterRotateSpeed,
                    AppData.SkeletonHealth,
                    AppData.SkeletonAttack,
                    AppData.SkeletonDefence,
                    this.managerParameters
                )
            );

            this.enemyDictionary.Add(
                "Cultist",
                new EnemyObject(
                    "Cultist",
                    ActorType.Enemy,
                    Transform3D.Zero,
                    this.effectDictionary["cultistEffect"],
                    this.modelDictionary["cultistModel"],
                    AppData.CharacterAccelerationRate,
                    AppData.CharacterDecelerationRate,
                    AppData.CharacterMovementVector,
                    AppData.CharacterRotationVector,
                    AppData.CharacterMoveSpeed,
                    AppData.CharacterRotateSpeed,
                    AppData.CultistHealth,
                    AppData.CultistAttack,
                    AppData.CultistDefence,
                    this.managerParameters
                )
            );
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
        #endregion

        #region Debug
        #if DEBUG
        private void InitializeDebugCollisionSkinInfo()
        {
            this.physicsDebugDrawer = new PhysicsDebugDrawer(
                this, 
                this.cameraManager, 
                this.objectManager,
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