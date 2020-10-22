using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public class Vault : Door
{
    bool hasPlayedSound = false;
    protected override void Start()
    {
        base.Start();
        tag = "Untagged";
    }

    public override void MoveDoor()
    {
        
        if (!hasPlayedSound)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/VaultOpen"), ((Vector2)transform.position));
            hasPlayedSound = true;
        }
        base.MoveDoor();

        if (!moving)
            hasPlayedSound = false;
    }

    public override void AssignUnlockItems()
    {
        unlockItems = new HashSet<uint> { ItemList.ITEM_DRILL.UID };
    }
}