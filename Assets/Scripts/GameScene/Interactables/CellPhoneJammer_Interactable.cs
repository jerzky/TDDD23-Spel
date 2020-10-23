using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPhoneJammer_Interactable : Interactable
{
    public float Distance { get; private set; } = Mathf.Infinity; // very high to test that it actually works
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("bullet"))
            Destroy(gameObject);
    }

    public override bool Interact(uint itemIndex)
    {
        if (itemIndex != 0)
            return false;
        Inventory.Instance.AddItem(ItemList.ITEM_CELLPHONE_JAMMER.UID, 1);
        Destroy(gameObject);
        return false;
    }

    public override void Cancel()
    {
        
    }

    public override string Name()
    {
        return "CellPhoneJammer";
    }
}
