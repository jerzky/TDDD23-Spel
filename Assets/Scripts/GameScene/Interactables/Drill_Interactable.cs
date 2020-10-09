using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill_Interactable : Interactable
{
    AudioSource audioSource;
    Interactable inter;
    bool active = false;
    uint continousSoundID = 0;
    // Start is called before the first frame update
    void Start()
    {
        continousSoundID = SoundController.Instance.GenerateContinousSound(new Sound(transform.position, ItemList.ITEM_DRILL.SoundRadius, Sound.SoundType.Construction));
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
        if (!active)
            return;
        if(!((Openable)inter).IsLocked())
        {
            audioSource.enabled = false;
            active = false;
            SoundController.Instance.CancelContinousSound(continousSoundID);
        }
    }

    public override void Cancel()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("bullet"))
            Destroy(gameObject);
    }

    public override bool Interact(uint itemIndex)
    {
        SoundController.Instance.CancelContinousSound(continousSoundID);
        Inventory.Instance.AddItem(ItemList.ITEM_DRILL.UID, 1);
        Destroy(gameObject);
        return false;
    }

    public override string Name()
    {
        return "Drill";
    }
}
