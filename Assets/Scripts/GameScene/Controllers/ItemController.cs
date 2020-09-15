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
        var lockpick = temp.AddComponent<LockPick>();
        var sledge = temp.AddComponent<SledgeHammer>();
        var pistol = temp.AddComponent<Pistol>();
        Items.Add(ItemList.ITEM_EXPLOSIVE_REMOTE.UID, exp);
        Items.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID, exp);
        Items.Add(ItemList.ITEM_LOCKPICK.UID, lockpick);
        Items.Add(ItemList.ITEM_SLEDGEHAMMER.UID, sledge);
        Items.Add(ItemList.ITEM_PISTOL.UID, pistol);


    }
    public void Use(ItemInfo item, Vector3 pos)
    {
        Items[item.UID].Use(item, pos);
    }

    public void SecondaryUse(ItemInfo item, Vector3 pos)
    {
        Items[item.UID].SecondaryUse(item, pos);

    }
}
