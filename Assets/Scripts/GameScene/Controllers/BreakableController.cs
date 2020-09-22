using Assets.Items;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    public static BreakableController Instance;
    SortedDictionary<string, int> damagedObjects = new SortedDictionary<string, int>();
    SortedSet<string> brokenObjects = new SortedSet<string>();
    public void Start()
    {
        Instance = this;
    }

    public bool HitObject(GameObject go, uint itemID)
    {
        if(go.tag == "breakable" && !brokenObjects.Contains(go.transform.position.ToString()))
        {   
            int damage = ItemList.AllItems[itemID].BreakableDamage;
            if(damage == 0f)
            {
                return false;
            }
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Hit2"), new Vector3(go.transform.position.x, go.transform.position.y, 2.5f));
            return DamageObject(go, damage);
        }

        return false;
    }

    bool DamageObject(GameObject go, int damage)
    {
        if (damagedObjects.ContainsKey(go.transform.position.ToString()))
        {
            damagedObjects[go.transform.position.ToString()] -= damage;
            if (damagedObjects[go.transform.position.ToString()] <= 0)
                Destroy(go);
            else
            {
                int durability;
                if (!MapController.AllTiles.ContainsKey(go.name))
                    durability = 75;
                else
                    durability = MapController.AllTiles[go.name].Durability;
                EditColor(durability, damagedObjects[go.transform.position.ToString()], go);
            }
        }
        else
        {
            int durability;
            if (!MapController.AllTiles.ContainsKey(go.name))
                durability = 75;
            else
                durability = MapController.AllTiles[go.name].Durability;
            damagedObjects.Add(go.transform.position.ToString(), durability - damage);
            if (damagedObjects[go.transform.position.ToString()] <= 0)
                Destroy(go);
            else
            {
                EditColor(durability, damagedObjects[go.transform.position.ToString()], go);
            }
        }

        return go == null;
    }

    private void EditColor(float max, float curr, GameObject go)
    {
        float c = curr / max;
        go.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
    }

    void Destroy(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().enabled = false;
        go.GetComponent<BoxCollider2D>().enabled = false;
        brokenObjects.Add(go.transform.position.ToString());
        damagedObjects.Remove(go.transform.position.ToString());
    }
}
