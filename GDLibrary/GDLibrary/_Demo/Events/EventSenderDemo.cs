
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class EventSenderDemo : GameComponent
    {
        //a list of registered functions which will handle the event when fired
        public delegate void PlayerEventHandler(string s, int x, object sender);
        public delegate void WinLoseEventHandler();
        public delegate void CameraEventHandler(EventData eventData);

        //a flag that is set when an event occurs - it is either null (not yet happened) or non-null - when the event occurs the delegate reads through its list and calls all the listening functions
        public event PlayerEventHandler PlayerChanged;
        public event CameraEventHandler CameraChanged;
        public event WinLoseEventHandler GameStateChanged;

        public EventSenderDemo(
            Game game
        ) : base (game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.E))
                OnPlayer("Hello from sender!", 1, this);

            if (keyState.IsKeyDown(Keys.C))
            {
                object[] additionalParameters = { "Hello", 123, true, 0.6f, "Goodbye" };
                EventData eventData = new EventData(
                    EventActionType.OnCameraToggle,
                    EventCategoryType.Camera,
                    additionalParameters
                );
                OnCamera(eventData);
            }    
        }

        //called when the event occurs and notifications need to be generated
        protected virtual void OnPlayer(string s, int x, object sender)
        {
            //?. - is it null or not (if statement)
            PlayerChanged?.Invoke(s, x, sender);
        }

        protected virtual void OnCamera(EventData eventData)
        {
            //Enumerating through the delegate (i.ie through the list) and invoking function based on the function pointers sotred within the delegate list
            CameraChanged?.Invoke(eventData);
        }

    }
}