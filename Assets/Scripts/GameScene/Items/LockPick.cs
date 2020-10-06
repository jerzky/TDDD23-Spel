using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : UsableItem
{
    private bool active = false;
    private float timer = 0f;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;
            if(timer >= 1f)
            {
                timer = 0f;
                SoundController.Instance.GenerateSound(new Sound(transform.position, ItemList.ITEM_LOCKPICK.SoundRadius, Sound.SoundType.Construction));
            }
        }  
    }
    public override uint Use(ItemInfo item, Vector3 pos)
    {
        Debug.Log("USE ITEM LOCKPICK");
        SoundController.Instance.GenerateSound(new Sound(transform.position, ItemList.ITEM_LOCKPICK.SoundRadius, Sound.SoundType.Construction));
        audioSource.enabled = true;
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LockPicking"));
        active = true;
        timer = 0f;
        return 0;
    }

    public override void Cancel()
    {
        Debug.Log("CANCEL ITEM LOCKPICK");
        audioSource.enabled = false;
        active = false;
    }
}
