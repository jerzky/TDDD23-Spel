﻿using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory 
{
    public static Inventory Instance;
    static int inventorySizeX = 8;
    static int inventorySizeY = 9;
    uint currentItem = 0;
    GameObject SelectedItem;

    public class InventoryItem
    {
        public ItemInfo ItemInfo { get; set; }
        public uint Count { get; set; }

        public InventoryItem(ItemInfo i, uint c)
        {
            ItemInfo = i;
            Count = c;
        }
    }

    private ItemBar ItemBar { get; set; }
    public InventoryItem[,] InventoryItemInfo = new InventoryItem[inventorySizeX, inventorySizeY];
    public SortedDictionary<uint, Vector2> InventoryItemMap = new SortedDictionary<uint, Vector2>();
    public Inventory(ItemBar bar)
    {
        Instance = this;
        ItemBar = bar;
    }



    public bool AddItem(uint id, uint count)
    {
        Vector2 itemPos;
        if(InventoryItemMap.TryGetValue(id, out itemPos))
        {
            InventoryItem temp = InventoryItemInfo[(int)itemPos.x, (int)itemPos.y];
            if(temp.Count + count <= temp.ItemInfo.InventoryStackSize)
            {
                temp.Count += count;
                ItemBar.UpdateCount(itemPos, temp.Count);
                return true;
            }
            else
            {
                return false;
            }
        }

        ItemInfo item;

        if (!ItemList.AllItems.TryGetValue(id, out item))
        {
            return false;
        }

        for (int y = 0; y < inventorySizeX; y++)
        {
            for (int x = 0; x < inventorySizeY; x++)
            {
                if (InventoryItemInfo[x, y] == null)
                {
                    InventoryItemMap.Add(id, new Vector2(x, y));
                    InventoryItemInfo[x, y] = new InventoryItem(item, count);
                    ItemBar.AddItem(new Vector2(x, y), InventoryItemInfo[x, y]);
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(uint id, uint count)
    {
        Vector2 itemPos;
        if (InventoryItemMap.TryGetValue(id, out itemPos))
        {
            
            if(InventoryItemInfo[(int)itemPos.x, (int)itemPos.y].Count <= count)
            {
                InventoryItemInfo[(int)itemPos.x, (int)itemPos.y] = null;
                InventoryItemMap.Remove(id);
                ItemBar.RemoveItem(itemPos);
            }
            else
            {
                InventoryItemInfo[(int)itemPos.x, (int)itemPos.y].Count -= count;
                ItemBar.UpdateCount(itemPos, InventoryItemInfo[(int)itemPos.x, (int)itemPos.y].Count);
            }
            return true;
        }
        return false;
    }

    public ItemInfo GetItem(Vector2 index)
    {
        return InventoryItemInfo[(int)index.x, (int)index.y].ItemInfo;
    }

    public uint GetItemCount(uint itemUID)
    {
        Vector2 pos;
        if(InventoryItemMap.TryGetValue(itemUID, out pos))
        {
            if (pos != null)
            {
                return InventoryItemInfo[(int)pos.x, (int)pos.y].Count;
            }
        }
        return 0;
    }

    public ItemInfo GetCurrentItem()
    {
        if (InventoryItemInfo[currentItem, 0] == null)
            return null;
        return InventoryItemInfo[currentItem, 0].ItemInfo;
    }

    public void OpenInventory()
    {
        ItemBar.OpenInventory();
    }

    public void SelectItem()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)
            return;
        if (SelectedItem == null)
        {
            Vector2 first;
            first.x = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(0, 1));
            first.y = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 1));
            InventoryItem firstInfo = InventoryItemInfo[(int)first.x, (int)first.y];
            if (firstInfo == null)
            {
                DeSelectItem();
                return;
            }
            SelectedItem = EventSystem.current.currentSelectedGameObject;
        }
        else
        {
            GameObject secondGO = EventSystem.current.currentSelectedGameObject;
            Vector2 first;
            first.x = int.Parse(SelectedItem.name.Substring(0, 1));
            first.y = int.Parse(SelectedItem.name.Substring(2, 1));
            Vector2 second;
            second.x = int.Parse(secondGO.name.Substring(0, 1));
            second.y = int.Parse(secondGO.name.Substring(2, 1));

            InventoryItem firstInfo = InventoryItemInfo[(int)first.x, (int)first.y];
            if (firstInfo == null) 
                return;
            InventoryItem secondInfo = InventoryItemInfo[(int)second.x, (int)second.y];

            InventoryItemMap.Remove(firstInfo.ItemInfo.UID);
            InventoryItemMap.Add(firstInfo.ItemInfo.UID, second);


            InventoryItemInfo[(int)first.x, (int)first.y] = secondInfo;
            InventoryItemInfo[(int)second.x, (int)second.y] = firstInfo;
            ItemBar.RemoveItem(first);
            
            if(secondInfo != null)
            {
                InventoryItemMap.Remove(secondInfo.ItemInfo.UID);
                InventoryItemMap.Add(secondInfo.ItemInfo.UID, first);
                ItemBar.RemoveItem(second);
                ItemBar.AddItem(first, secondInfo);
                ItemBar.UpdateCount(first, secondInfo.Count);
                
            }
            ItemBar.AddItem(second, firstInfo);
            ItemBar.UpdateCount(second, firstInfo.Count);
            DeSelectItem();
            WeaponController.Instance.ChangeWeaponSprite();
        }
    }

    public void DeSelectItem()
    {
        SelectedItem = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateCurrentItem(uint index)
    {
        currentItem = index;
        ItemBar.UpdateCurrentItem(index);
    }
}
