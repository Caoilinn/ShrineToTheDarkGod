/*
Function: 		Represent a message broker for events received and routed through the game engine. 
                Allows the receiver to receive event messages with no reference to the publisher - decouples the sender and receiver.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
Comments:       Should consider making this class a Singleton because of the static message Stack - See https://msdn.microsoft.com/en-us/library/ff650316.aspx
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class EventDispatcher : GameComponent
    {
        //See Queue doc - https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1?view=netframework-4.7.1
        private static Queue<EventData> queue; //stores events in arrival sequence
        private static HashSet<EventData> uniqueSet; //prevents the same event from existing in the stack for a single update cycle (e.g. when playing a sound based on keyboard press)
      
        //A delegate is basically a list - the list contains a pointer to a function - this function pointer comes from the object wishing to be notified when the event occurs.
        public delegate void CameraEventHandler(EventData eventData);
        public delegate void MenuEventHandler(EventData eventData);
        public delegate void Sound2DEventHandler(EventData eventData);
        public delegate void Sound3DEventHandler(EventData eventData);
        public delegate void GlobalSoundEventHandler(EventData eventData);
        public delegate void GameEventHandler(EventData eventData);

        //An event is either null (not yet happened) or non-null - when the event occurs the delegate reads through its list and calls all the listening functions
        public event CameraEventHandler CameraChanged;
        public event MenuEventHandler MenuChanged;
        public event Sound2DEventHandler Sound2DChanged;
        public event Sound3DEventHandler Sound3DChanged;
        public event GlobalSoundEventHandler GlobalSoundChanged;
        public event GameEventHandler GameChanged;

        public EventDispatcher(
            Game game, 
            int initialSize
        ) : base(game) {
            queue = new Queue<EventData>(initialSize);
            uniqueSet = new HashSet<EventData>(new EventDataEqualityComparer());
        }

        public static void Publish(EventData eventData)
        {
            //this prevents the same event being added multiple times within a single update e.g. 10x bell ring sounds
            if (!uniqueSet.Contains(eventData))
            {
                queue.Enqueue(eventData);
                uniqueSet.Add(eventData);
            }
        }

        EventData eventData;
        public override void Update(GameTime gameTime)
        { 
            for (int i = 0; i < queue.Count; i++)
            {
                eventData = queue.Dequeue();
                Process(eventData);
                uniqueSet.Remove(eventData);
            }

            base.Update(gameTime);
        }

        private void Process(EventData eventData)
        {
            //Switch - See https://msdn.microsoft.com/en-us/library/06tc147t.aspx
            //One case for each category type
            switch (eventData.EventCategoryType)
            {
                case EventCategoryType.MainMenu:
                    OnMenu(eventData);
                    break;

                case EventCategoryType.Camera:
                    OnCamera(eventData);
                    break;

                case EventCategoryType.Sound2D:
                    OnSound2D(eventData);
                    break;

                case EventCategoryType.Sound3D:
                    OnSound3D(eventData);
                    break;

                case EventCategoryType.GlobalSound:
                    OnGlobalSound(eventData);
                    break;

                case EventCategoryType.Game:
                    OnGameChanged(eventData);
                    break;

                default:
                    break;
            }
        }

        //Called when a camera event needs to be generated
        protected virtual void OnCamera(EventData eventData)
        {
            CameraChanged?.Invoke(eventData);
        }

        //Called when a menu change is requested
        protected virtual void OnMenu(EventData eventData)
        {
            MenuChanged?.Invoke(eventData);
        }

        //Called when a 2D sound event is sent e.g. play "menu music"
        protected virtual void OnSound2D(EventData eventData)
        {
            Sound2DChanged?.Invoke(eventData);
        }

        //Called when a 3D sound event is sent e.g. play "boom"
        protected virtual void OnSound3D(EventData eventData)
        {
            Sound3DChanged?.Invoke(eventData);
        }

        //Called when a global sound event is sent to set volume by category or mute all sounds
        protected virtual void OnGlobalSound(EventData eventData)
        {
            GlobalSoundChanged?.Invoke(eventData);
        }
        
        //Called when the game state has changed (next level, reset level etc.)
        protected virtual void OnGameChanged(EventData eventData)
        {
            GameChanged?.Invoke(eventData);
        }
    }
}
