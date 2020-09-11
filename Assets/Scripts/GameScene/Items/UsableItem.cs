using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : MonoBehaviour
{

    public abstract uint Add(ItemInfo item, Vector3 pos);

}
