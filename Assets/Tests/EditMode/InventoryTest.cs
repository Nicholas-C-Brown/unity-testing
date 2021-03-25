using NUnit.Framework;
using SneakyLunatic.Inventory;
using UnityEngine;

namespace Tests
{
    public class InventoryTest
    {
        Inventory inventory;
        Item nonStackableItem1, nonStackableItem2, stackableItem1, stackableItem2;

        [SetUp]
        public void Setup()
        {
            inventory = new Inventory(5);

            Sprite blankSprite = Sprite.Create(new Texture2D(0, 0), new Rect(), Vector2.zero);
            nonStackableItem1 = new Item("Test Non-Stackable 1", Item.ItemType.Misc, blankSprite);
            nonStackableItem2 = new Item("Test Non-Stackable 2", Item.ItemType.Food, blankSprite);
            stackableItem1 = new Item("Test Stackable 1", Item.ItemType.Misc, blankSprite, 5);
            stackableItem2 = new Item("Test Stackable 2", Item.ItemType.Food, blankSprite, 3);
        }

        [Test]
        public void AddItem_ToSpecifiedSlot()
        {
            bool success = inventory.AddItem(nonStackableItem1, 0);

            Assert.IsTrue(success);
            Assert.AreEqual(nonStackableItem1, inventory.GetItem(0));
            Assert.AreNotEqual(nonStackableItem2, inventory.GetItem(0));
            Assert.AreEqual(1, inventory.GetItemAmount(0));
        }

        [Test]
        public void AddItem_ToFreeSlot()
        {
            //Populate some random items
            inventory.AddItem(nonStackableItem1, 0);
            inventory.AddItem(nonStackableItem1, 1);
            inventory.AddItem(nonStackableItem1, 3);

            bool success = inventory.AddItem(nonStackableItem2);

            Assert.IsTrue(success);
            Assert.AreEqual(nonStackableItem2, inventory.GetItem(2));
            Assert.AreEqual(1, inventory.GetItemAmount(2));
        }

        [Test]
        public void AddItems_ToSpecifiedSlot()
        {
            int toAdd = 9;

            int expected = toAdd - stackableItem1.MaxStackCount;
            int actual = inventory.AddItems(stackableItem1, toAdd, 0);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(stackableItem1, inventory.GetItem(0));
            Assert.AreEqual(5, inventory.GetItemAmount(0));
        }

        [Test]
        public void AddItems_ToFreeSlots()
        {
            bool success = inventory.AddItems(stackableItem2, 7);
            Assert.IsTrue(success);
            Assert.AreEqual(inventory.GetItem(0), stackableItem2);
            Assert.AreEqual(3, inventory.GetItemAmount(0));
            Assert.AreEqual(1, inventory.GetItemAmount(2));
        }

        [Test]
        public void RemoveItem()
        {
            inventory.AddItems(stackableItem1, 5, 0);
            Assert.AreEqual(5, inventory.GetItemAmount(0));

            bool success = inventory.RemoveItems(3, 0);
            Assert.IsTrue(success);
            Assert.AreEqual(2, inventory.GetItemAmount(0));
        }

        [Test]
        public void RemoveItemsByName()
        {
            inventory.AddItems(stackableItem2, 8);
            Assert.AreEqual(3, inventory.GetItemAmount(0));
            Assert.AreEqual(2, inventory.GetItemAmount(2));

            bool success = inventory.RemoveItemsByName(stackableItem2.Name, 7);
            Assert.IsTrue(success);
            Assert.AreEqual(0, inventory.GetItemAmount(0));
            Assert.AreEqual(1, inventory.GetItemAmount(2));

            bool fail = inventory.RemoveItemsByName(stackableItem2.Name, 2);
            Assert.IsFalse(fail);
        }

       
    }
}
