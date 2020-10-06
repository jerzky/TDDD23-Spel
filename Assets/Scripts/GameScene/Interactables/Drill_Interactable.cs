using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill_Interactable : Interactable
{
    AudioSource audioSource;
    private float timer = 1f;
    Interactable inter;
    bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
        active = true;
    }

    public void StartDrilling(Interactable inter)
    {
        this.inter = inter;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("active " + active);
        if (!active)
            return;
        timer += Time.deltaTime;
        Debug.Log("TIMER INCREASE?");
        if (timer >= 1f)
        {
            Debug.Log("SOUNDWAVES?");
            timer = 0f;
            SoundController.Instance.GenerateSound(new Sound(transform.position, ItemList.ITEM_DRILL.SoundRadius, Sound.SoundType.Construction));
        }
        if(!((Openable)inter).IsLocked())
        {
            audioSource.enabled = false;
            active = false;
        }
    }

    public override void Cancel()
    {

    }

    public override bool Interact(uint itemIndex)
    {
        Debug.Log("DRILL INTER");
        Inventory.Instance.AddItem(ItemList.ITEM_DRILL.UID, 1);
        Destroy(gameObject);
        return false;
    }
}
