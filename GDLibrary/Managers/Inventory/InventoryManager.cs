using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    class InventoryManager : PausableDrawableGameComponent
    {
        
        #region Fields
        private List<ImmovablePickupObject> items;
        private ManagerParameters managerParameters;
        #endregion

        #region Consturctor
        public InventoryManager(Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType, ManagerParameters managerParameters) : 
            base(game, eventDispatcher, statusType)
        {
            this.managerParameters = managerParameters;
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
            if(eventData.EventType == EventActionType.OnItemAdded)
            {
                ImmovablePickupObject itemToAdd = eventData.AdditionalParameters[0] as ImmovablePickupObject;

                EventDispatcher.Publish(new EventData(
                    EventActionType.OnInventoryPickUp,
                    EventCategoryType.UIMenu,
                    new object[] { itemToAdd })
                    );

                this.AddItem(itemToAdd);

                //Publish UI cahnged event to update the inventory UI
                EventDispatcher.Publish(new EventData(
                                        EventActionType.OnAddToInventory,
                                        EventCategoryType.UIMenu,
                                        new object[] { itemToAdd.PickupParameters.PickupType}))
            } else if(eventData.EventType == EventActionType.OnItemRemoved)
            {
                ImmovablePickupObject itemToRemove = eventData.AdditionalParameters[0] as ImmovablePickupObject;



                //Publish UI event
                

            }
        }

        #endregion

        #region Methods
        public void AddItem(ImmovablePickupObject item)
        {
            if(item != null) {
                items.Add(item);

                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnItemAdded,
                        EventCategoryType.Inventory)
                        );
            }

        }

        public ImmovablePickupObject GetItem(string itemID)
        {
            if(items != null)
            {
                return this.items.Find(x=> x.ID == itemID);
            }
            return null;
        }

        public bool HasKey(string itemID)
        {
            return (this.items.Find(x => x.ID == itemID) != null);
        }

        public void UseItem(string itemID)
        {
            ImmovablePickupObject item = GetItem(itemID);

            if (itemID != null)
            {
                EventDispatcher.Publish(
                    new EventData(
                        EventActionType.OnItemRemoved,
                        EventCategoryType.Inventory)
                        );
            }
        }

        public void PrintInventory()
        {
           foreach (ImmovablePickupObject item in items)
            {
                Console.WriteLine("ID: " + item.ID);
            }
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
