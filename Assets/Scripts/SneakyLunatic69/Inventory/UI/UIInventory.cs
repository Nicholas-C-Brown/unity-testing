using System.Text;
using UnityEngine;

#pragma warning disable CS0649
namespace SneakyLunatic.Inventory.UI
{
    public class UIInventory : MonoBehaviour
    {

        [SerializeField]
        private GameObject uiSlotPrefab;

        private Inventory inventory;
        private UISlot[] slots;

        public Inventory GetInventory()
        {
            return inventory;
        }

        public void SetInventory(Inventory inventory)
        {
            this.inventory = inventory;
            this.inventory.InventoryUpdate += OnInventoryUpdate;
            InitializeInventory();
        }

        /// <summary>
        /// Destroys and replaces inventory UI components
        /// </summary>
        private void InitializeInventory()
        {
            if (inventory == null) return;

            if (slots != null)
            {
                //Destroy existing inventory slots
                for (int i = 0; i < slots.Length; i++)
                {
                    Destroy(slots[i].gameObject);
                }
            }

            //Initialize new slots array
            slots = new UISlot[inventory.GetSize()];

            //Instantiate new inventory slots
            for(int i = 0; i<slots.Length; i++)
            {
                GameObject instance = Instantiate(uiSlotPrefab, transform);
                UISlot slot = instance.GetComponent<UISlot>();
                slot.ItemDropped += OnItemDropped;
                slot.ID = i;
                slots[i] = slot;
            }

            OnInventoryUpdate();
        }

        /// <summary>
        /// Iterates through all UISlots and updates their UIItem based on the Inventory's items
        /// </summary>
        private void OnInventoryUpdate()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetUIItem(inventory.GetItem(i), inventory.GetItemAmount(i));  
            }
        }

        /// <summary>
        /// Handles logic to move items from one Inventory slot to another
        /// </summary>
        /// <param name="startSlot">Slot item was picked up from</param>
        /// <param name="endSlot">Slot item was dropped into</param>
        /// <param name="fromInventory">Inventory item was picked up from</param>
        private void OnItemDropped(int startSlot, int endSlot, Inventory fromInventory)
        {
            Item item = fromInventory.GetItem(startSlot);
            int amount = fromInventory.GetItemAmount(startSlot);

            //Add as many items as possible to the new slot
            //Keep track of how many items were added
            int toRemove = amount - inventory.AddItems(item, amount, endSlot);

            //Remove that many items from the old slot
            if(toRemove > 0)
                fromInventory.RemoveItems(toRemove, startSlot);
        }
        
    }
}
