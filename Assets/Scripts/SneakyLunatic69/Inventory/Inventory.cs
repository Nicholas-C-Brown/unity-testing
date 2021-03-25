using System.Collections.Generic;
using UnityEngine;
using System;

namespace SneakyLunatic.Inventory
{
    public class Inventory
    {

        public Action InventoryUpdate { get; set; }
        public List<Item.ItemType> AllowedTypes { get; private set; }

        private readonly Slot[] inventory;

        //Debug Messages
        private const string NO_SPACE = "Cannot add item(s). No valid slots available.";
        private const string INVALID_SLOT = "Cannot add item. Invalid slot number.";
        private const string INVALID_AMOUNT = "Cannot add item(s). Amount cannot be 0 or less.";
        private const string INVALID_TYPE = "Cannot add item(s). Slot does not allow this item type.";
        private const string SLOT_FULL = "Cannot add item(s). Slot is full";

        private const string SLOT_EMPTY = "Cannot remove item(s). Slot is empty.";
        private const string NOT_ENOUGH_ITEMS = "Cannot remove item(s). Remove count exceed amount stored in inventory.";

        public Inventory(int slots)
        {
            AllowedTypes = new List<Item.ItemType>();
            AllowedTypes.AddRange(Item.ITEM_TYPES);

            inventory = new Slot[slots];
            initializeStorage();
        }

        private void initializeStorage()
        {
            for(int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = new Slot();
            }
        }


        /// <summary>
        /// Replaces inventory's allowed item types
        /// </summary>
        /// <param name="allowed">List of allowed item types</param>
        public void SetAllowedTypes( List<Item.ItemType> allowed)
        {
            AllowedTypes.Clear();
            AllowedTypes.AddRange(allowed);
        }

        #region Add/Remove

        /// <summary>
        /// Adds a single item to the next available slot.
        /// </summary>
        /// <param name="item">Item to add to the inventory.</param>
        /// <returns>
        /// True if operation is successful
        /// </returns>
        public bool AddItem(Item item)
        {
            if (IsFreeSlotAvailable(item))
            {
                return AddItem(item, getNextFreeSlotIndex(item));
            }
            else
            {
                Debug.Log(NO_SPACE);
                return false;
            }
        }

        /// <summary>
        /// Adds a single item to the specified slot
        /// </summary>
        /// <param name="item">Item to add to the inventory</param>
        /// <param name="slotNum">Inventory slot to add the item to</param>
        /// <returns>
        /// True if operation is successful
        /// </returns>
        public bool AddItem(Item item, int slotNum)
        {
            if (!ValidateSlot(item, slotNum, 1)) return false;

            Slot s = inventory[slotNum];

            if (s.GetItem() == null) s.SetItem(item, 1);
            else s.AddAmount(1);

            InventoryUpdate?.Invoke();
            return true;
        }

        /// <summary>
        /// Adds a specified number of an item to any available free slots.
        /// </summary>
        /// <param name="item">Item to add to the inventory</param>
        /// <param name="amount">Amount of items to add to the inventory</param>
        /// <returns>
        /// True if operation is successful.
        /// </returns>
        public bool AddItems(Item item, int amount)
        {
            if (amount <= 0)
            {
                Debug.Log(INVALID_AMOUNT);
                return false;
            }

            if (amount == 1) return AddItem(item);
            
            Dictionary<Slot, int> toAdd = new Dictionary<Slot, int>();

            while(amount > 0)
            {
                int slotIndex = getNextFreeSlotIndex(item, new List<Slot>(toAdd.Keys));
                if (slotIndex == -1)
                {
                    Debug.Log(NO_SPACE);
                    return false;
                }

                Slot s = inventory[slotIndex];

                int addAmount = GetAmountToAdd(s, item, amount);
                toAdd.Add(s, addAmount);
                amount -= addAmount;
            }

            foreach (Slot s in toAdd.Keys)
            {
                if (s.GetItem() == null) s.SetItem(item, toAdd[s]);
                else s.AddAmount(toAdd[s]);
            }
            InventoryUpdate?.Invoke();
            return true;
        }


        /// <summary>
        /// Adds a stack of items to a given slot.
        /// </summary>
        /// <param name="item">Item to add to the inventory</param>
        /// <param name="amount">Amount of items to add to the inventory</param>
        /// <param name="slotNum">Inventory slot to add the item to</param>
        /// <returns>
        /// Number of items left in stack after adding to slot.
        /// </returns>
        public int AddItems(Item item, int amount, int slotNum)
        {
            if (!ValidateSlot(item, slotNum, 1)) return amount;

            Slot s = inventory[slotNum];
            int addAmount = GetAmountToAdd(s, item, amount);

            if (s.GetItem() == null) s.SetItem(item, addAmount); 
            else s.AddAmount(addAmount);
                
            InventoryUpdate?.Invoke();
            return amount - addAmount;
        }

        /// <summary>
        /// Adds a stack of items to a given slot.
        /// </summary>
        /// <param name="amount">Amount of items to remove the inventory</param>
        /// <param name="slotNum">Inventory slot to remove the item from</param>
        /// <returns>
        /// Number of items left in stack after adding to slot.
        /// </returns>
        public bool RemoveItems(int amount, int slotNum)
        {
            if (!IsValidSlotNumber(slotNum))
            {
                Debug.Log(INVALID_SLOT);
                return false;
            }

            Slot s = inventory[slotNum];

            if (s.IsEmpty()) 
            {
                Debug.Log(SLOT_EMPTY);
                return false;
            }

            if(amount > s.GetAmount())
            {
                Debug.Log(NOT_ENOUGH_ITEMS);
                return false;
            }

            s.RemoveAmount(amount);
            InventoryUpdate?.Invoke();
            return true;
        }

        /// <summary>
        /// Adds a stack of items to a given slot.
        /// </summary>
        /// <param name="name">Name of the item to remove</param>
        /// <param name="amount">Amount of items to remove from the inventory</param>
        /// <returns>
        /// Number of items left in stack after adding to slot.
        /// </returns>
        public bool RemoveItemsByName(string name, int amount)
        {
            Dictionary<Slot, int> toRemove = new Dictionary<Slot, int>();

            foreach(Slot s in inventory)
            {
                if (s.GetItem() != null && s.GetItem().Name.Equals(name))
                {
                    int removeAmount = Mathf.Min(s.GetAmount(), amount);
                    toRemove.Add(s, removeAmount);
                    amount -= removeAmount;
                    if (amount <= 0) break;
                } 
            }

            if (amount > 0) return false;
            foreach (Slot s in toRemove.Keys) s.RemoveAmount(toRemove[s]);
            InventoryUpdate?.Invoke();
            return true;
        }

        public Item GetItem(int slotNum)
        {
            //Returns null if the provided slotNum is out of bounds.
            if (!IsValidSlotNumber(slotNum)) return null;

            return inventory[slotNum].GetItem();
        } 

        public int GetItemAmount(int slotNum)
        {
            //Returns -1 if the provided slotNum is out of bounds.
            if (!IsValidSlotNumber(slotNum)) return -1;

            if (inventory[slotNum].GetItem() == null) return 0;
            else return inventory[slotNum].GetAmount();
        }

        public int GetSize()
        {
            return inventory.Length;
        }

        #endregion

        #region Helper
        public bool IsItemTypeAllowed(Item item)
        {
            return AllowedTypes.Contains(item.Type);
        }

        private bool ValidateSlot(Item item, int slotNum, int amount)
        {
            Slot s = inventory[slotNum];

            if (!IsValidSlotNumber(slotNum))
            {
                Debug.Log(INVALID_SLOT);
                return false;
            }

            if (!IsItemTypeAllowed(item))
            {
                Debug.Log(INVALID_TYPE);
                return false;
            }

            if (!IsSlotFree(s, item, amount))
            {
                Debug.Log(SLOT_FULL);
                return false;
            }

            return true;
        }

        private bool IsValidSlotNumber(int slotNum)
        {
            return !(slotNum >= inventory.Length || slotNum < 0);
        }

        private bool IsSlotFree(Slot slot, Item item, int amount)
        {
            if (slot.GetItem() == null) return true;
            if(slot.GetItem().Name == item.Name && (slot.GetAmount() + amount) <= item.MaxStackCount) return true;
            
            return false;
        }
        
        private int getNextFreeSlotIndex(Item item)
        {
            if (!IsItemTypeAllowed(item)) return -1;

            for (int i = 0; i < inventory.Length; i++)
            {
                Slot s = inventory[i];                
                if (IsSlotFree(s, item, 1)) return i;
            }

            return -1;
        }

        private int getNextFreeSlotIndex(Item item, List<Slot> exclude)
        {
            if (!IsItemTypeAllowed(item)) return -1;

            for (int i = 0; i < inventory.Length; i++)
            {
                Slot s = inventory[i];
                if (IsSlotFree(s, item, 1) && !exclude.Contains(s)) return i;
            }

            return -1;
        }

        private bool IsFreeSlotAvailable(Item item)
        {
            return getNextFreeSlotIndex(item) != -1;
        }

        private int GetAmountToAdd(Slot s, Item item, int amount)
        {
            if (s.GetItem() == null) return Mathf.Min(amount, item.MaxStackCount);
            else return Mathf.Min(amount, item.MaxStackCount - s.GetAmount()); 
        }

        #endregion

        private class Slot {
            //Error messages
            private const string EXCEEDS_MAX_STACK = "Amount exceeds item max stack size.";
            private const string EXCEEDS_ITEM_AMOUNT = "Amount exceeds item amount in slot.";
            private const string SLOT_INVALID_AMOUNT = "Amount cannot be less than or equal to zero.";

            private Item item;
            private int amount;

            public Slot()
            {
                item = null;
                amount = 0;
            }

            public Item GetItem()
            {
                return item;
            }

            public int GetAmount()
            {
                if (IsEmpty()) return 0;
                else return amount;
            }

            public void SetItem(Item item, int amount)
            {
                if (amount <= item.MaxStackCount)
                {
                    this.item = item;
                    this.amount = amount;
                }
                else
                {
                    Debug.LogError(EXCEEDS_MAX_STACK);
                } 
            }

            public void AddAmount(int amount)
            {
                if (amount > 0) {
                    if (this.amount + amount <= item.MaxStackCount)
                    {
                        this.amount += amount;
                    } else
                    {
                        Debug.LogError(EXCEEDS_MAX_STACK);
                    }
                }else
                {
                    Debug.Log(SLOT_INVALID_AMOUNT);
                }
            }

            public void RemoveAmount(int amount)
            {
                if (amount > 0)
                {
                    if (this.amount - amount >= 0)
                    {
                        this.amount -= amount;
                        if(this.amount == 0)
                        {
                            item = null;
                        }
                    }
                    else
                    {
                        Debug.LogError(EXCEEDS_ITEM_AMOUNT);
                    }
                } else
                {
                    Debug.LogError(SLOT_INVALID_AMOUNT);
                }
            }

            public bool IsEmpty()
            {
                return item == null;
            }
        }


    }
}
