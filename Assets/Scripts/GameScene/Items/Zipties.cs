using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Zipties : UsableItem
{
    public override void Cancel()
    {
        
    }

    public override uint Use(ItemInfo item, Vector3 pos)
    {

        RaycastHit2D hit = Physics2D.Raycast(pos, PlayerController.Instance.lookDir, 2, LayerMask.GetMask("AI"), -Mathf.Infinity, Mathf.Infinity);
        if (hit.collider == null)
            return 0;

        hit.collider.GetComponent<AI>().GetZipTied();
        Inventory.Instance.RemoveItem(item.UID, 1);
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/ziptie3s"), pos);
        SoundController.Instance.GenerateContinousSound(new Sound(pos, ItemList.ITEM_ZIPTIES.SoundRadius, Sound.SoundType.Construction), ItemList.ITEM_ZIPTIES.AverageUseTime);
        return 0;
    }
}
