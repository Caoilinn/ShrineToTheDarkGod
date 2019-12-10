using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class KeybindManager : PausableDrawableGameComponent
    {
        public KeybindManager(
            Game game,
            StatusType statusType,
            EventDispatcher eventDispatcher,
            MouseManager mouseManager,
            KeyboardManager keyboardManager,
            SpriteBatch spriteBatch)
            : base(game, statusType, eventDispatcher) { }

        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.KeybindEvent += EventDispatcher_Keybind;

            base.RegisterForEventHandling(eventDispatcher);
        }

        protected void EventDispatcher_Keybind(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Keybind)
            {
                if (eventData.EventType.Equals(EventActionType.OnKeybind))
                {
                    if (eventData.AdditionalParameters[0] != null)
                    {
                        switch (eventData.AdditionalParameters[0])
                        {
                            case "Attack":
                                GDLibraryData.CombatKeys[0] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Defend":
                                GDLibraryData.CombatKeys[1] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Dodge":
                                GDLibraryData.CombatKeys[2] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Forward":
                                GDLibraryData.CameraMoveKeys[0] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Back":
                                GDLibraryData.CameraMoveKeys[1] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Left":
                                GDLibraryData.CameraMoveKeys[2] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "Right":
                                GDLibraryData.CameraMoveKeys[3] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "TLeft":
                                GDLibraryData.CameraMoveKeys[4] = (Keys)eventData.AdditionalParameters[1];
                                break;

                            case "TRight":
                                GDLibraryData.CameraMoveKeys[5] = (Keys)eventData.AdditionalParameters[1];
                                break;

                        }

                    } 
                }
            }
        }
    }
}
