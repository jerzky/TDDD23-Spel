using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public class Vault : Door
{
    protected override void Start()
    {
        base.Start();
        tag = "Untagged";
    }

    public override void MoveDoor()
    {
        base.MoveDoor();
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/VaultOpen"), transform.position);
    }

    public override void AssignUnlockItems()
    {
        unlockItems = new HashSet<uint> { ItemList.ITEM_DRILL.UID };
    }
}