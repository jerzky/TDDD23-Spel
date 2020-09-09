using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour
{
    public static BreakableController Instance;
    SortedDictionary<GameObject, int> damagedObjects = new SortedDictionary<GameObject, int>();
    HashSet<GameObject> brokenObjects = new HashSet<GameObject>();
    public void Start()
    {
        Instance = this;
    }

    public bool HitObject(GameObject go, uint itemID)
    {
        Debug.Log("hit object");
        if(go.tag == "breakable" && go.GetComponent<BoxCollider2D>().enabled == true)
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
            damagedObjects[go] -= damage;
            Debug.Log("hit saved object: " + damagedObjects[go]);
            if (damagedObjects[go] <= 0)
                Destroy(go);
            else
            {
                EditColor(MapController.AllTiles[go.name].Durability, damagedObjects[go], go);
            }
        }
        else
        {
            Debug.Log("hit new object");

            int durability = MapController.AllTiles[go.name].Durability;
            damagedObjects.Add(go, durability - damage);
            if (damagedObjects[go] <= 0)
                Destroy(go);
            else
            {
                EditColor(durability, damagedObjects[go], go);
            }
        }

        return false;
    }

    private void EditColor(float max, float curr, GameObject go)
    {
        float c = curr / max;
        Debug.Log(c);
        go.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
        Debug.Log(go.GetComponent<SpriteRenderer>().color);
    }

    void Destroy(GameObject go)
    {
        Debug.Log("Destroy object");

        go.GetComponent<SpriteRenderer>().enabled = false;
        go.GetComponent<BoxCollider2D>().enabled = false;
        brokenObjects.Add(go);
        damagedObjects.Remove(go);
    }
}
