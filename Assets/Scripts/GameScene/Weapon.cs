using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{
    protected ItemInfo info { get; set; }
    protected Weapon(ItemInfo info)
    {
        this.info = info;
    }
    public abstract bool Use(GameObject go);
}
