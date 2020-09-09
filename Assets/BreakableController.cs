using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController
{
    SortedDictionary<GameObject, int> damagedObjects = new SortedDictionary<GameObject, int>();
    HashSet<GameObject> brokenObjects = new HashSet<GameObject>();
    public BreakableController()
    {

    }

    public bool BreakObject(GameObject go, uint itemID)
    {
        if(go.tag == "breakable")
        {
            int damage = ItemList.AllItems[itemID].BreakableDamage;
            if(damage == 0f)
            {
                return false;
            }
            DamageObject(go, damage);
        }

        return false;
    }

    bool DamageObject(GameObject go, int damage)
    {
        if (damagedObjects.ContainsKey(go))
        {
            int currentDurability = damagedObjects[go];
            currentDurability -= damage;
            if (currentDurability < 0)
                Destroy(go);
        }
        else
        {
            int durability = MapController.AllTiles[go.name].Durability;
            damagedObjects.Add(go, durability - damage);
            int currentDurability = damagedObjects[go];
            currentDurability -= damage;
            if (currentDurability < 0)
                Destroy(go);
            else
            {
                int c = durability / currentDurability;
                go.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
            }
        }

        return false;
    }

    void Destroy(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().enabled = false;
        brokenObjects.Add(go);
        damagedObjects.Remove(go);
    }
}
