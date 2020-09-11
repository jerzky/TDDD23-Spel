using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockpick : Item
{
    public override uint ID { get { return ItemList.ITEM_LOCKPICK.UID; } }

    // Start is called before the first frame update
    void Start()
    {

      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Interact(Interactable interactable)
    {
        Debug.LogError("WHAAT?");
        return interactable.Interact(ID);
    }

    public override bool Use(uint id)
    {
        Debug.LogError("WHAAT?");
        return false;
    }
}
