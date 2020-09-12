using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Items;
using UnityEngine.UI;

public class Store : Interactable
{
    bool isOpen = false;
    HashSet<ItemInfo> items;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssignItems(HashSet<ItemInfo> items)
    {
        this.items = items;
    }

    public override void Cancel()
    {
        if (isOpen)
            StoreController.Instance.Toggle(false);
        isOpen = false;
    }

    public override bool Interact(uint itemIndex)
    {
        Debug.Log("STORE INTERACT" + itemIndex);
        if (itemIndex != 0) 
            return false;
        Debug.Log("STORE WITH ITEM 0");
        if (!isOpen)
            Open();
        else
            Cancel();
        return true;
    }

    void Open()
    {
        StoreController.Instance.Toggle(true);
        isOpen = !isOpen;
    }
}
