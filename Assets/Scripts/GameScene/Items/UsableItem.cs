using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : MonoBehaviour
{
    public abstract uint Use(ItemInfo item, Vector3 pos);
    public virtual uint SecondaryUse(ItemInfo item, Vector3 pos)
    {
        return 0;
    }

}
