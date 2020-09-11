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
        public ItemInfo ItemInfo { get; set; }

        public bool Removed { get; set; } = false;
        public bool HasExploded { get; set; } = false;
        public uint SpriteExplosionIndex { get; set; } = 0;
    }

    private uint IdCounter = 1;
    private List<ExplosiveInfo> TimedExplosives = new List<ExplosiveInfo>();
    private SortedDictionary<uint, ExplosiveInfo> RemoteExplosives = new SortedDictionary<uint, ExplosiveInfo>();

    private const float EXPLOSIVE_RADIUS = 2f;
    private Sprite[] ExplosionSprites;

    private void Start()
    {
        ExplosionSprites = Resources.LoadAll<Sprite>("Textures/ExplosionSprites");
    }
    private const float EXPLOSION_FRAME_DELAY = 0.07f;
    private void Update()
    {

        foreach(var exp in TimedExplosives)
        {
            exp.Timer -= Time.deltaTime;
            if (exp.Timer <= 0f)
            {
                if (!exp.HasExploded)
                {           
                    exp.Timer = EXPLOSION_FRAME_DELAY;
                    exp.HasExploded = true;
                }
                else
                {
                    if (exp.SpriteExplosionIndex >= ExplosionSprites.Length)
                    {
                        Destroy(exp.GameObject);
                        exp.Removed = true;
                    }
                    else
                    {
                        exp.GameObject.transform.localScale = new Vector3(4, 4, 1);
                        exp.GameObject.GetComponent<SpriteRenderer>().sprite = ExplosionSprites[exp.SpriteExplosionIndex++];
                        exp.Timer = EXPLOSION_FRAME_DELAY;

                        if(exp.SpriteExplosionIndex == 10)
                        {
                            Explode(exp);
                        }
                    }
                }
            }           
        }
        TimedExplosives.RemoveAll(e => e.Removed);
    }

    public override uint Add(ItemInfo item, Vector3 pos)
    {
        if(item.UID == ItemList.ITEM_EXPLOSIVE_REMOTE.UID)
        {
            RemoteExplosives.Add(IdCounter++, new ExplosiveInfo
            {
                Timer = 0,
                GameObject = CreateExplosive(item, pos),
                ItemInfo = item
            });
            return IdCounter;

        }
        else
        {
            TimedExplosives.Add(new ExplosiveInfo
            {
                Timer = 2f,
                GameObject = CreateExplosive(item,pos),
                ItemInfo = item
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

        var colliders = Physics2D.OverlapCircleAll(info.GameObject.transform.position, EXPLOSIVE_RADIUS);
        foreach (var c in colliders)
        {
            if (c.gameObject.CompareTag("humanoid"))
            {

            }
            else
            {
               BreakableController.Instance.HitObject(c.gameObject, info.ItemInfo.UID);
            }
        }
    
    }
}
