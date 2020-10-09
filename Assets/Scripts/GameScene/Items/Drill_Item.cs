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
        RaycastHit2D hit = Physics2D.Raycast(pos, PlayerController.Instance.lookDir, 1f, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
        if(hit.collider != null)
        {
            Interactable inter = hit.collider.GetComponent<Interactable>();
            if(inter != null)
            {
                var position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, 1f) + (Vector3)PlayerController.Instance.lookDir * -0.3f;
                GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Drill"), position, Quaternion.identity, null);
                temp.GetComponent<Drill_Interactable>().StartDrilling(inter);
                Inventory.Instance.RemoveItem(ItemList.ITEM_DRILL.UID, 1);
            }
        }
        
        return 0;
    }

    public override void Cancel()
    {
        
    }
}
