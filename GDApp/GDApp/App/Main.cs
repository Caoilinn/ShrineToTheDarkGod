using System;
using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace GDApp
{
    /// <summary>
    /// Demonstrates the use of a PrimitiveObject to store a reference to user-defined vertices and draw and update same.
    /// </summary>
    public class Main : Game
    {
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        private VertexPositionColorTexture[] vertices;
        private BasicEffect texturedVertexEffect;
        private BasicEffect modelEffect;

        private CameraManager cameraManager;
        private ObjectManager object3DManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private Vector2 screenCentre;

        private ModelObject staticModel;
        private ModelObject drivableModel;
        Vector3 driveRotation = Vector3.Zero;

        private int currentLevel = 0;
        private int[,,] map;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            initGraphics(1280, 800);
            initEffects();
            initVertices();

            initManagers();

            loadMap();
            initMap();

            float worldScale = 2.54f;
            initCameras(worldScale);
            initGround(worldScale);
            initBack(worldScale);
            initLeft(worldScale);
            initRight(worldScale);
            initTop(worldScale);
            initFront(worldScale);

            base.Initialize();
        }

        private void initModels()
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

            this.staticModel = new ModelObject(
                "room1",
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

        private void initMap()
        {
            Transform3D transform;
            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
            EffectParameters effectParameters;
            Model model;

            //List<Texture2D> textureList = new List<Texture2D>() { Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1") };

            List<EffectParameters> effectParametersList = new List<EffectParameters>() {
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1),
                new EffectParameters(this.modelEffect, texture, Color.White, 1)
            };

            List<Model> modelList = new List<Model>() {
                Content.Load<Model>("Assets/Models/Room_001"),
                Content.Load<Model>("Assets/Models/Room_002"),
                Content.Load<Model>("Assets/Models/Room_003"),
                Content.Load<Model>("Assets/Models/Room_004"),
                Content.Load<Model>("Assets/Models/Room_005"),
                Content.Load<Model>("Assets/Models/Room_006"),
                Content.Load<Model>("Assets/Models/Room_007"),
                Content.Load<Model>("Assets/Models/Room_008"),
                Content.Load<Model>("Assets/Models/Room_009"),
                Content.Load<Model>("Assets/Models/Room_010"),
                Content.Load<Model>("Assets/Models/Room_011"),
                Content.Load<Model>("Assets/Models/Room_012"),
                Content.Load<Model>("Assets/Models/Room_013"),
                Content.Load<Model>("Assets/Models/Room_014"),
                Content.Load<Model>("Assets/Models/Room_015"),
                Content.Load<Model>("Assets/Models/Room_016"),
            };

            for (int x = 0; x < this.map.GetLength(0); x++)
                for (int y = 0; y < this.map.GetLength(1); y++)
                    for (int z = 0; z < this.map.GetLength(2); z++)
                    {
                        transform = new Transform3D(
                            new Vector3(x * (100 * 2.54f), y * (100 * 2.54f), z * (100 * 2.54f)),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            -Vector3.UnitZ,
                            Vector3.UnitY
                        );

                        Console.WriteLine("x: " + x + ", y: " + y + ", z: " + z + ", value: " + this.map[x,y,z]);

                        if (this.map[x, y, z] > 0)
                        {
                            //texture = textureList[(this.map[x, y, z]) - 1];
                            effectParameters = effectParametersList[this.map[x, y, z] - 1];
                            model = modelList[this.map[x, y, z] - 1];

                            this.staticModel = new ModelObject(
                                "room1",
                                ActorType.Billboard,
                                StatusType.Update | StatusType.Drawn,
                                transform,
                                effectParameters,
                                model
                            );

                            this.object3DManager.Add(staticModel);
                        }
                    }
        }     

        private void loadMap()
        {
            int x = 1, y = 1, z = 1;

            //Store all file data
            string fileText = File.ReadAllText("App/Data/levelData.txt");

            //Split the file into an array of levels
            string[] levels = fileText.Split('*');

            //Split the current level into an array of layers (y axis)
            string[] layers = levels[this.currentLevel].Split('&');
            y = layers.Length;

            #region Determine Map Size
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

                    //Update z
                    z = lines.Length;

                //Loop through each line
                foreach (string line in lines)
                {
                    string cleanLine;

                    //Cleanup line
                    cleanLine = line.Trim();
                    cleanLine = cleanLine.Replace('|', ' ');
                    cleanLine = cleanLine.Replace(" ", string.Empty);
                    cleanLine = cleanLine.Replace(",", string.Empty);

                    //If the line lenght is larger than the current x (each line length)
                    if (cleanLine.Length > x)

                        //Update x dimension
                        x = cleanLine.Length;
                }
            }

            //Create array based on map size
            this.map = new int[x, y, z];
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
                        map[x, y, z] = int.Parse(room);

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
            #endregion
        }

        private void initVertices()
        {
            this.vertices = VertexFactory.GetVertexPositionColorTextureQuad();
        }

        private void initManagers()
        {
            this.mouseManager = new MouseManager(this);
            Components.Add(this.mouseManager);
            this.mouseManager.SetPosition(this.screenCentre);

            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);

            this.cameraManager = new CameraManager(this);
            Components.Add(this.cameraManager);

            //new manager to store all drawn objects
            //add to the component list so that it will receive the Update() and Draw() heartbeat
            this.object3DManager = new ObjectManager(this, this.cameraManager);
            Components.Add(this.object3DManager);
        }

        private void initGraphics(int width, int height)
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

        private void initCameras(float worldScale)
        {
            Transform3D transform = null;
            Camera3D camera = null;

            #region First-Person Camera
            transform = new Transform3D(
                new Vector3(635, 127, 889),
                Vector3.Zero,
                Vector3.One * worldScale,
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

            ManagerParameters managerParameters = new ManagerParameters(
                object3DManager,
                cameraManager,
                mouseManager,
                keyboardManager,
                null
            );

            IController firstPersonCameraController = new FirstPersonCameraController(
                "FP Controller 1",
                ControllerType.FirstPerson,
                AppData.CameraMoveKeys,
                AppData.CameraMoveSpeed,
                AppData.CameraStrafeSpeed,
                AppData.CameraRotationSpeed,
                managerParameters
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

        private void initEffects()
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

        private void initGround(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(0, 0, 0),
                new Vector3(-90, 0, 0),
                new Vector3(worldScale, worldScale, 1),
                Vector3.UnitZ, 
                Vector3.UnitY
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices, 
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "grass", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initRight(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(worldScale / 2, 0, 0),
                new Vector3(0, -90, 0),
                new Vector3(worldScale, worldScale, 1),
                Vector3.UnitZ, 
                Vector3.UnitY
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices,
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "right skybox", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initLeft(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(-worldScale / 2, 0, 0),
                new Vector3(0, 90, 0),
                new Vector3(worldScale, worldScale, 1),
                Vector3.UnitZ, 
                Vector3.UnitY
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices,
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "left skybox", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initBack(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(0, 0, -worldScale / 2),
                Vector3.Zero,
                new Vector3(worldScale, worldScale, 1),
                Vector3.UnitZ, 
                Vector3.UnitY
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices,
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "back skybox", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initTop(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(0, worldScale / 2, 0),
                new Vector3(90, -90, 0),
                new Vector3(worldScale, worldScale, 1),
                -Vector3.UnitY, 
                Vector3.UnitZ
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices,
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "back skybox", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initFront(float worldScale)
        {
            Transform3D transform = new Transform3D(
                new Vector3(0, 0, worldScale / 2),
                new Vector3(0, 180, 0),
                new Vector3(worldScale, worldScale, 1),
                -Vector3.UnitY, 
                Vector3.UnitZ
            );

            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            EffectParameters effectParameters = new EffectParameters(
                this.texturedVertexEffect,
                texture, 
                Color.White, 
                1
            );

            VertexData<VertexPositionColorTexture> vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices,
                PrimitiveType.TriangleStrip, 
                2
            );

            PrimitiveObject pObj = new PrimitiveObject(
                "front skybox", 
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform,
                effectParameters,
                vertexData
            );

            this.object3DManager.Add(pObj);
        }

        private void initMap(float worldScale)
        {

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