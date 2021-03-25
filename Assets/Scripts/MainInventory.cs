using SneakyLunatic.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestingGround
{
    public class MainInventory : AbstractInventoryManager
    {

        public Sprite sprite, sprite2, sprite3;

        protected override void Start()
        {
            base.Start();
            TestInventory();
        }

        private void TestInventory()
        {
            inventory.AddItems(new Item("RATTLE", Item.ItemType.Weapon, sprite2, 3), 2, 0);
            inventory.AddItems(new Item("RATTLE", Item.ItemType.Weapon, sprite2, 3), 1, 3);
            inventory.AddItems(new Item("Bababooie", Item.ItemType.Misc, sprite, 3), 7);
            inventory.AddItems(new Item("circul", Item.ItemType.Weapon, sprite3, 3), 1, 5);
            inventory.AddItems(new Item("circul", Item.ItemType.Weapon, sprite3, 3), 1, 6);
        }

    }
}

