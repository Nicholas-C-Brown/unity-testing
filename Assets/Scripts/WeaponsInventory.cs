using SneakyLunatic.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestingGround {
    public class WeaponsInventory : AbstractInventoryManager
    {
        protected override void Start()
        {
            base.Start();

            List<Item.ItemType> allowed = new List<Item.ItemType>
            {
                Item.ItemType.Weapon
            };

            inventory.SetAllowedTypes(allowed);
        }
    }
}
