using System.Collections.Generic;

namespace GDLibrary
{
    //used by the EventDispatcher to compare to events in the HashSet - remember that HashSets allow us to quickly prevent something from being added to a list/stack twice
    class EventDataEqualityComparer : IEqualityComparer<EventData>
    {
        //Compare two events
        public bool Equals(EventData e1, EventData e2)
        {
            //Default flag as true
            bool bEquals = true;

            //If both event ID's are set
            if (e1.ID != null && e2.ID != null)

                //Check if the event ID's match
                bEquals = e1.ID.Equals(e2.ID);

            //If bEquals is true (not previously flagged as false)
            bEquals = bEquals

                //and if both event types match
                && e1.EventType.Equals(e2.EventType)

                    //and if both event category types match
                    && e1.EventCategoryType.Equals(e2.EventCategoryType);

            //If both event senders have been specified
            if (e1.Sender != null && e2.Sender != null)

                //If bEquals is true (not previously flagged as false)
                bEquals = bEquals

                    //and if both senders match
                    && (e1.Sender as Actor).GetID().Equals(e2.Sender as Actor);
            //&& (e1.Sender as Actor).Equals(e2.Sender as Actor);

            //Return state
            return bEquals;
        }

        public int GetHashCode(EventData e)
        {
            return e.GetHashCode();
        }
    }
}