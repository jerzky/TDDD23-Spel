using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static ItemController Instance;
    private SortedDictionary<uint, UsableItem> Items = new SortedDictionary<uint, UsableItem>();

    private void Start()
    {
        Instance = this;
        GameObject temp = new GameObject("UsableItems");
        var exp = temp.AddComponent<Explosive>();
        Items.Add(ItemList.ITEM_EXPLOSIVE_REMOTE.UID, exp);
        Items.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID, exp);
    }
    public void Use(ItemInfo item, Vector3 pos)
    {

        Items[item.UID].Add(item, pos);
    }
}
