using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockpick : Item
{
    public override uint ID { get; set; }
    public override bool MustInteract { get { return true; } }



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
        if (interactable.Interact(ID))
        {
            return true;
        }
        return false;
    }

    public override bool Use(uint id)
    {
        return false;
    }
}
