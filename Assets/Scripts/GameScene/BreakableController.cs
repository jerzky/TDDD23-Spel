using Assets.Items;
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
        Debug.Log("hit object");
        if(go.tag == "breakable" && !brokenObjects.Contains(go.transform.position.ToString()))
        {
            int damage = ItemList.AllItems[itemID].BreakableDamage;
            if(damage == 0f)
            {
                return false;
            }
           return DamageObject(go, damage);
        }

        return false;
    }

    bool DamageObject(GameObject go, int damage)
    {
        if (damagedObjects.ContainsKey(go.transform.position.ToString()))
        {
            damagedObjects[go.transform.position.ToString()] -= damage;
            Debug.Log("hit saved object: " + damagedObjects[go.transform.position.ToString()]);
            if (damagedObjects[go.transform.position.ToString()] <= 0)
                Destroy(go);
            else
            {
                EditColor(MapController.AllTiles[go.name].Durability, damagedObjects[go.transform.position.ToString()], go);
            }
        }
        else
        {
            Debug.Log("hit new object");

            int durability = MapController.AllTiles[go.name].Durability;
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
        Debug.Log(c);
        go.GetComponent<SpriteRenderer>().color = new Color(c, c, c);
        Debug.Log(go.GetComponent<SpriteRenderer>().color);
    }

    void Destroy(GameObject go)
    {
        Debug.Log("Destroy object");

        go.GetComponent<SpriteRenderer>().enabled = false;
        go.GetComponent<BoxCollider2D>().enabled = false;
        brokenObjects.Add(go.transform.position.ToString());
        damagedObjects.Remove(go.transform.position.ToString());
    }
}
