using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class InventoryManager : PausableDrawableGameComponent
    {        
        #region Fields
        private List<ImmovablePickupObject> items;
        #endregion

        #region Constructor
        public InventoryManager(Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : base(game, statusType, eventDispatcher) {
            this.items = new List<ImmovablePickupObject>();
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.InventoryEvent += EventDispatcher_InventoryChanged;
            base.RegisterForEventHandling(eventDispatcher);
        }

        //Handles UI Updates for Inventory -- need UI code to implement
        protected void EventDispatcher_InventoryChanged(EventData eventData)
        {
            //If an add item event has been published
            if (eventData.EventType == EventActionType.OnItemAdded)
            {
                ImmovablePickupObject itemToAdd = eventData.AdditionalParameters[0] as ImmovablePickupObject;
                this.AddItem(itemToAdd);

                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnInventoryPickUp,
                        EventCategoryType.UIMenu,
                        new object[] { itemToAdd }
                    )
                );

                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnUpdateHud,
                        EventCategoryType.UI,
                        new object[] { itemToAdd.PickupParameters.PickupType }
                    )
                );

                      
            }

            //If a remove item eveNT has been published
            else if(eventData.EventType == EventActionType.OnItemRemoved)
            {
                //Create item
                ImmovablePickupObject item = eventData.AdditionalParameters[0] as ImmovablePickupObject;

                //Publish UI event
                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnUpdateHud,
                        EventCategoryType.UI,
                        new object[] { item }
                    )
                );

                //Remove from inventory
                UseItem(item);
            }
        }
        #endregion

        #region Methods
        public void AddItem(ImmovablePickupObject item)
        {
            if(item != null)
                items.Add(item);
        }

        public ImmovablePickupObject GetItemByDescription(string description)
        {
            if (items != null)
                foreach (ImmovablePickupObject item in this.items)
                    if (item.PickupParameters.Description == description)
                        return item;

            return null;
        }

        public bool HasItem(PickupType itemType)
        {
            if (items != null)
                foreach (ImmovablePickupObject item in this.items)
                    if (item.PickupParameters.PickupType == itemType)
                        return true;

            return false;
        }

        public void UseItem(PickupType itemType)
        {
            if (items != null)
            {
                foreach (ImmovablePickupObject item in this.items)
                {
                    if (item.PickupParameters.PickupType == itemType)
                    {
                        items.Remove(item);
                        return;
                    }
                }
            }
        }

        public void UseItem(ImmovablePickupObject item)
        {
            if (item != null)
            {
                items.Remove(item);
            }
        }

        public void PrintInventory()
        {
           foreach (ImmovablePickupObject item in items)
                Console.WriteLine("ID: " + item.ID);
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyboard(gameTime);
            base.Update(gameTime);
        }

        //Testing adding and removing
        protected override void HandleKeyboard(GameTime gameTime)
        {
            base.HandleKeyboard(gameTime);
        }
        #endregion
    }
}