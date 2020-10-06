using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public class Drill_Item : UsableItem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override uint Use(ItemInfo item, Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, PlayerController.Instance.lookDir, 2, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
        if(hit.collider != null)
        {
            Interactable inter = hit.collider.GetComponent<Interactable>();
            if(inter != null)
            {
                GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Drill"), pos + (Vector3)PlayerController.Instance.lookDir.normalized * 0f, Quaternion.identity, null);
                temp.GetComponent<Drill_Interactable>().StartDrilling(inter);
            }
        }
        
        return 0;
    }

    public override void Cancel()
    {
        
    }
}
