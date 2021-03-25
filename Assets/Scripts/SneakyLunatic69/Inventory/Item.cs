using SneakyLunatic.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SneakyLunatic.Inventory
{
    public class Item
    {

        public readonly static IEnumerable<ItemType> ITEM_TYPES = SneakyUtils.GetEnumValues<ItemType>();
        public enum ItemType
        {
            Weapon,
            Food,
            Misc
        }

        public ItemType Type { get; private set; }
        public int MaxStackCount { get; private set; }

        public Sprite Icon { get; private set; }
        public string Name { get; private set; }

        public Item(string name, ItemType type, Sprite icon) : this(name, type, icon, 1) { }

        public Item(string name, ItemType type, Sprite icon, int maxStackCount) 
        {
            Name = name;
            Type = type;
            Icon = icon;
            MaxStackCount = maxStackCount;
        }
        
    }
}
