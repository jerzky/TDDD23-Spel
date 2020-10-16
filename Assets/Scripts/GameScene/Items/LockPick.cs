using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : UsableItem
{
    AudioSource audioSource;
    uint continousSoundID = 0;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
          
    }
    public override uint Use(ItemInfo item, Vector3 pos)
    {
        continousSoundID = SoundController.Instance.GenerateContinousSound(new Sound(transform.position, ItemList.ITEM_LOCKPICK.SoundRadius, Sound.SoundType.Construction), ItemList.ITEM_LOCKPICK.AverageUseTime);
        audioSource.enabled = true;
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LockPicking"));
        return 0;
    }

    public override void Cancel()
    {
      
        audioSource.enabled = false;
        SoundController.Instance.CancelContinousSound(continousSoundID);
    }
}
