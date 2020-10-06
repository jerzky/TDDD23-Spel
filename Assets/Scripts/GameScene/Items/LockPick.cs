using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : UsableItem
{
    private bool active = false;
    private float timer = 5f;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            timer -= Time.deltaTime;
            if(timer <= 1f)
            {

            }
        }
        
            
    }
    public override uint Use(ItemInfo item, Vector3 pos)
    {
        SoundController.Instance.GenerateSound(new Sound(pos, ItemList.ITEM_LOCKPICK.SoundRadius, Sound.SoundType.Construction));
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/LockPicking"), new Vector3(pos.x, pos.y, -5));
        active = true;
        timer = 5f;
        return 0;
    }

    public override void Cancel()
    {
        active = false;
        timer = 5f;
    }
}
