using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPhoneJammer_Item : UsableItem
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Cancel()
    {
        
    }

    public override uint Use(ItemInfo item, Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, PlayerController.Instance.lookDir, 1, LayerMask.GetMask("walls"), -Mathf.Infinity, Mathf.Infinity);
        if (hit.collider != null)
        {
            var position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, 1f) + (Vector3)PlayerController.Instance.lookDir * -0.3f;
            Instantiate(Resources.Load<GameObject>("Prefabs/cellphonejammer"), position, Quaternion.identity, null);
            Inventory.Instance.RemoveItem(ItemList.ITEM_CELLPHONE_JAMMER.UID, 1);
        }
        return 0;
    }
}
