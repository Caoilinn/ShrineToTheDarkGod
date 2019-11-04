/*
Function: 		Encapsulates the event data sent by the sender to the dispatcher. This data will be interpreted and acted upon by the registered recipients.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public class EventData
    {
        #region Fields
        private string id;
        private object sender;
        private EventActionType eventType;
        private EventCategoryType eventCategoryType;

        //Optional array for passing multiple event parameters
        //Used for camera, sound, or video-based events
        private object[] additionalParameters;
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public object Sender
        {
            get
            {
                return this.sender;
            }
            set
            {
                this.sender = value;
            }
        }

        public EventActionType EventType
        {
            get
            {
                return this.eventType;
            }
            set
            {
                this.eventType = value;
            }
        }

        public EventCategoryType EventCategoryType
        {
            get
            {
                return this.eventCategoryType;
            }
            set
            {
                this.eventCategoryType = value;
            }
        }

        public object[] AdditionalParameters
        {
            get
            {
                return this.additionalParameters;
            }
            set
            {
                this.additionalParameters = value;
            }
        }
        #endregion

        #region Constructors
        //When we don't have any pertinent string data in ID AND sender
        public EventData(
            EventActionType eventType,
            EventCategoryType eventCategoryType
        ) : this(null, null, eventType, eventCategoryType, null) {

        }

        //Qhen we don't have any pertinent string data in ID AND sender but have additional event parameters
        public EventData(
            EventActionType eventType,
            EventCategoryType eventCategoryType,
            object[] additionalParameters
        ) : this(null, null, eventType, eventCategoryType, additionalParameters) {

        }

        //When we don't have any pertinent string data in ID
        public EventData(
            object sender,
            EventActionType eventType,
            EventCategoryType eventCategoryType
        ) : this(null, sender, eventType, eventCategoryType, null) {

        }

        //Pre-object[] compatability constructor
        public EventData(
            string id,
            object sender,
            EventActionType eventType,
            EventCategoryType eventCategoryType
        ) : this(id, sender, eventType, eventCategoryType, null) {

        }

        //Supports passing of multiple parameter objects within an event
        public EventData(
            string id,
            object sender,
            EventActionType eventType,
            EventCategoryType eventCategoryType,
            object[] additionalParameters
        ) {
            this.id = id;                               //Sender ID
            this.sender = sender;                       //Sender Object Ref
            this.eventType = eventType;                 //Play/Mute/Volume/Zone 
            this.eventCategoryType = eventCategoryType; //Origination - UI, Menu, Video

            //Used to pass information between sender and registered recipient(s)
            this.additionalParameters = additionalParameters;
        }
        #endregion

        #region Methods
        public object Clone()
        {
            //Using MemberwiseClone as all data-types are primitives or structs
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            EventData other = obj as EventData;

            bool bEquals = true;
            
            if (this.id != null)
                bEquals = bEquals 
                    && this.id.Equals(other.ID);

            if (this.sender != null)
                bEquals = bEquals 
                    && this.sender.Equals(other.Sender);

            return bEquals
                && ((this.additionalParameters != null && this.additionalParameters.Length != 0) ? this.additionalParameters.Equals(other.additionalParameters) : true)
                && this.eventType == other.EventType
                && this.eventCategoryType == other.EventCategoryType;
        }

        public override int GetHashCode()
        {
            int hash = 1;
            if (this.id != null)
                hash = hash * 7 + this.id.GetHashCode();
            if (this.sender != null)
                hash = hash * 11 + this.sender.GetHashCode();

            if (this.additionalParameters != null && this.additionalParameters.Length != 0)
                hash = hash * 31 + this.additionalParameters.GetHashCode();

            hash = hash * 47 + this.eventType.GetHashCode();
            hash = hash * 79 + this.eventCategoryType.GetHashCode();
            return hash;
        }
        #endregion
    }
}