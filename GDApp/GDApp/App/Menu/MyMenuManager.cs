using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDApp
{
    
    public class MyMenuManager : MenuManager
    {
        private readonly ObjectManager objectManager;
        private DrawnActor2D keybinding;

        private string action;
        #region Constructors
        public MyMenuManager(
            Game game,
            StatusType statusType,
            ObjectManager objectManager,
            EventDispatcher eventDispatcher, 
            CameraManager cameraManager,
            MouseManager mouseManager,
            KeyboardManager keyboardManager,
            SpriteBatch spriteBatch
        ) : base(game, statusType, eventDispatcher, cameraManager, mouseManager, keyboardManager, spriteBatch) {
            this.objectManager = objectManager;
        }
        #endregion

        #region Event Handling
        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Call base method to show/hide the menu
            base.EventDispatcher_MenuChanged(eventData);

            //Then generate sound events particular to your game e.g. play background music in a menu
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Add event to stop background menu music here...
                object[] additionalParameters = { "music_main", 1 };
                EventDispatcher.Publish(new EventData(EventActionType.OnStop, EventCategoryType.Sound2D, additionalParameters));

                //If additionaly parameters have been set
                if (eventData.AdditionalParameters != null)
                {
                    //If the event was a win event
                    if (eventData.AdditionalParameters[0].Equals("win_scene"))
                    {
                        //Display the win menu
                        SetActiveList("win menu");                     
                        this.objectManager.Clear();
                        
                    }

                    //If the event was a lose event
                    if (eventData.AdditionalParameters[0].Equals("lose_scene"))
                    {
                        //Display the win menu
                        SetActiveList("lose menu");
                    }
                }
            }
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Add event to play background menu music here...
                object[] additionalParameters = { "music_main" };
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, additionalParameters));

                //object [] additionalParameters = {2f, "battle_drums"};
                //EventDispatcher.Publish(new EventData(EventActionType.OnVolumeUp, EventCategoryType.Sound2D, additionalParameters));
            }
        }
        #endregion

        #region Methods
        protected override void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
            if (uiObject.Transform.Bounds.Contains(this.MouseManager.Bounds))
            {
                //Mouse is inside the bounds of the object - uiObject.ID
                if (this.MouseManager.IsLeftButtonClickedOnce())
                    HandleMouseClick(uiObject, gameTime);
            }
        }

        protected override void HandleKeyboardInput()
        {
            if (StateManager.IsKeyBinding && this.action != "")
            {
                KeyboardState state = Keyboard.GetState();
                Keys[] keys = state.GetPressedKeys();
                DoBind(this.action, keys[0]);
                this.action = "";
            }
        }

        //Add the code here to say how click events are handled by your code
        protected override void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            //Notice that the IDs are the same as the button IDs specified when we created the menu in Main::AddMenuElements()
            switch (uiObject.ID)
            {
                case "startbtn":

                    SetActiveList("begin menu");
                    break;

                case "exitbtn":

                    DoExit();
                    break;

                case "audiobtn":

                    //Use sceneIDs specified when we created the menu scenes in Main::AddMenuElements()
                    SetActiveList("audio menu");
                    break;

                case "volumeUpbtn":
                    {
                        //Curly brackets scope additionalParameters to be local to this case
                        object[] additionalParameters = { 0.1f, "Global" };
                        EventDispatcher.Publish(new EventData(EventActionType.OnVolumeUp, EventCategoryType.GlobalSound, additionalParameters));
                    }
                    break;

                case "volumeDownbtn":
                    {
                        object[] additionalParameters = { 0.1f, "Global" };
                        EventDispatcher.Publish(new EventData(EventActionType.OnVolumeDown, EventCategoryType.GlobalSound, additionalParameters));
                    }
                    break;

                case "volumeMutebtn":
                    {
                        object[] additionalParameters = { 0.0f, "Global" };
                        EventDispatcher.Publish(new EventData(EventActionType.OnMute, EventCategoryType.GlobalSound, additionalParameters));
                    }
                    break;

                case "volumeUnMutebtn":
                    {
                        object[] additionalParameters = { 0.5f, "Global" };
                        EventDispatcher.Publish(new EventData(EventActionType.OnUnMute, EventCategoryType.GlobalSound, additionalParameters));
                    }
                    break;

                case "backbtn":

                    //Use sceneIDs specified when we created the menu scenes in Main::AddMenuElements()
                    StateManager.IsKeyBinding = false;
                    SetActiveList("main menu");
                    break;

                case "controlsbtn":
                    SetActiveList("controls menu");
                    break;

                case "beginbtn":

                    //Use sceneIDs specified when we created the menu scenes in Main::AddMenuElements()
                    DoStart();
                    break;

                case "menubtn":

                    //Use sceneIDs specified when we created the menu scenes in Main::AddMenuElements()
                    SetActiveList("main menu");
                    break;

                case "bindbtn":
                    if (!StateManager.IsKeyBinding)
                        StateManager.IsKeyBinding = true;

                    else
                        StateManager.IsKeyBinding = false;

                    break;

                case "attack":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Attack";
                    }
                    
                    break;

                case "defend":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Defend";
                    }
                    break;

                case "dodge":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Dodge";
                    }
                    break;

                case "forward":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Forward";
                    }
                    break;

                case "back":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Back";
                    }
                    break;

                case "left":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Left";
                    }
                    break;

                case "right":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "Right";
                    }
                    break;

                case "tleft":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "TLeft";
                    }
                    break;

                case "tright":
                    if (StateManager.IsKeyBinding)
                    {
                        uiObject.Color = Color.LightSalmon;
                        this.keybinding = uiObject;
                        this.action = "TRight";
                    }
                    break;

                default:
                    break;
            }

            //Add event to play mouse click
            DoMenuClickSound();
        }

        private void DoMenuClickSound()
        {
            //e.g. Play a boing
            object[] additionalParameters = { "ui_click" };
            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, additionalParameters));
        }

        private void DoStart()
        {
            //Will be received by the menu manager and screen manager and set the menu to be shown and game to be paused
            EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
        }

        private void DoBind(string action, Keys key)
        {
            object[] additionalParameters = { action, key };
            EventDispatcher.Publish(new EventData(EventActionType.OnKeybind, EventCategoryType.Keybind, additionalParameters));
            (keybinding as UIButtonObject).Text = key.ToString();
            (keybinding as UIButtonObject).Color = Color.White;
        }

        private void DoExit()
        {
            this.Game.Exit();
        }
        #endregion
    }
}