using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Explosive : UsableItem
{
    private class ExplosiveInfo
    {
        public float Timer { get; set; }
        public GameObject GameObject { get; set; }
    }

    private uint IdCounter = 1;
    private List<ExplosiveInfo> TimedExplosives = new List<ExplosiveInfo>();
    private SortedDictionary<uint, ExplosiveInfo> RemoteExplosives = new SortedDictionary<uint, ExplosiveInfo>();

    private void Update()
    {

        foreach(var exp in TimedExplosives)
        {
            exp.Timer -= Time.deltaTime;
            if (exp.Timer <= 0f)
            {
                TimedExplosives.Remove(exp);
                Debug.Log("We have exploded!");
            }
              
        }
    }

    public override uint Add(ItemInfo item, Vector3 pos)
    {
        if(item.UID == ItemList.ITEM_EXPLOSIVE_REMOTE.UID)
        {
            RemoteExplosives.Add(IdCounter++, new ExplosiveInfo
            {
                Timer = 0,
                GameObject = CreateExplosive(item, pos)
            });
            return IdCounter;

        }
        else
        {
            TimedExplosives.Add(new ExplosiveInfo
            {
                Timer = 10f,
                GameObject = CreateExplosive(item,pos)
            });
        }
        return 0;
    }

    private GameObject CreateExplosive(ItemInfo item, Vector3 pos)
    {
        var gameObj = new GameObject(item.Name);
     
        gameObj.transform.position = pos;
        gameObj.transform.parent = transform;
        gameObj.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(item.IconPath);
        return gameObj;
    }

    void Explode(ExplosiveInfo info)
    {

    }
}
