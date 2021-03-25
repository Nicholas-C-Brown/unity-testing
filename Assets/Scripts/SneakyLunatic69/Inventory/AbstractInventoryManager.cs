using UnityEngine;
using SneakyLunatic.Inventory.UI;

namespace SneakyLunatic.Inventory
{
    /// <summary>
    /// All Inventory Managers should derive from this class.
    /// </summary>
    public abstract class AbstractInventoryManager : MonoBehaviour
    {

        [SerializeField]
        protected int size = 5;
        [SerializeField]
        protected UIInventory UI;

        protected Inventory inventory;
   
        protected virtual void Start()
        {
            inventory = new Inventory(size);
            UI.SetInventory(inventory);
        }


    }
}
