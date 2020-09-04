using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{



    public abstract uint ID { get; set; }
    public abstract bool MustInteract { get; }

    public abstract bool Use(uint id);

    public abstract bool Interact(Interactable interactable);
}
