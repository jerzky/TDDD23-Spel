using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : UsableItem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override uint Use(ItemInfo item, Vector3 pos)
    {
        SoundController.Instance.GenerateSound(new Sound(pos, ItemList.ITEM_LOCKPICK.SoundRadius, Sound.SoundType.Construction));
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/LockPicking"), new Vector3(pos.x, pos.y, -5));
        return 0;
    }
}
