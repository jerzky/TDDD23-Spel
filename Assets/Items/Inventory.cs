using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    private static readonly Dictionary<uint, ItemInfo> AllItems = new Dictionary<uint, ItemInfo>
    {
        {0,
             new ItemInfo
                 {
            IconPath = "Textures/lockpick",
            IconIndex = 0
        }
        }
    };

    public class ItemInfo
    {
        public string IconPath { get; set; }
        public uint IconIndex { get; set; }
       
    }

    public class CurrentItem
    {
        public uint Count { get; set; }
        public Item Item { get; set; }
    }

    private ItemBar ItemBar { get; set; }
    public ItemInfo[] ItemsVisible = new ItemInfo[8];
    public Inventory(ItemBar bar)
    {
        ItemBar = bar;
    
    }

 

    public bool AddItem(uint id, uint count)
    {
        ItemInfo item;

        if (!AllItems.TryGetValue(id, out item))
            return false;

        for (uint i = 0; i < 8; i++)
        {
            if (ItemsVisible[i] == null)
            {
                ItemsVisible[i] = item;
                ItemBar.AddItem(i, item);
                return true;
            }
         
        }
        return false;
              
    }

    public ItemInfo GetItem(uint index)
    {
        return ItemsVisible[index];
    }

    public void OpenInventory()
    {
        ItemBar.OpenInventory();
    }
}
