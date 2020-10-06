using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static ItemController Instance;
    private SortedDictionary<uint, UsableItem> Items = new SortedDictionary<uint, UsableItem>();
    private ItemInfo currentItem;

    private void Start()
    {
        Instance = this;
        GameObject temp = new GameObject("UsableItems");
        var exp = temp.AddComponent<Explosive>();
        var lockpick = temp.AddComponent<LockPick>();
        var sledge = temp.AddComponent<SledgeHammer>();
        var pistol = temp.AddComponent<Pistol>();
        var silencedPistol = temp.AddComponent<SilencedPistol>();
        var AK47 = temp.AddComponent<AK47>();
        var mac10 = temp.AddComponent<MAC10>();

        Items.Add(ItemList.ITEM_EXPLOSIVE_REMOTE.UID, exp);
        Items.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID, exp);
        Items.Add(ItemList.ITEM_LOCKPICK.UID, lockpick);
        Items.Add(ItemList.ITEM_SLEDGEHAMMER.UID, sledge);
        Items.Add(ItemList.ITEM_PISTOL.UID, pistol);
        Items.Add(ItemList.ITEM_SILENCED_PISTOL.UID, silencedPistol);
        Items.Add(ItemList.ITEM_AK47.UID, AK47);
        Items.Add(ItemList.ITEM_MAC10.UID, mac10);
    }
    public void Use(ItemInfo item, Vector3 pos)
    {
        Items[item.UID].Use(item, pos);
        currentItem = item;
    }

    public void Reload(ItemInfo itemInfo)
    {
        var gun = Items[itemInfo.UID] as Gun;
        if (gun == null)
            return;

        gun.Reload();
    }


    public void SecondaryUse(ItemInfo item, Vector3 pos)
    {
        Items[item.UID].SecondaryUse(item, pos);

    }

    public void CancelCurrentItem()
    {
        Items[currentItem.UID].Cancel();
    }
}
