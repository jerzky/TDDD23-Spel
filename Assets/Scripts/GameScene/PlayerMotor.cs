using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMotor : MonoBehaviour
{
    public static PlayerMotor Instance;
    Rigidbody2D rb;
    public Inventory Inventory { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        Inventory = new Inventory(FindObjectOfType<ItemBar>());

        if (!Inventory.AddItem(ItemList.ITEM_PISTOL.UID, 1))
            Debug.Log("Failed to add start item");

        if (!Inventory.AddItem(ItemList.ITEM_LOCKPICK.UID, 2))
            Debug.Log("Failed to add start item");

        if (!Inventory.AddItem(ItemList.ITEM_SLEDGEHAMMER.UID, 1))
            Debug.Log("Failed to add start item");

        if (!Inventory.AddItem(ItemList.ITEM_PISTOL_AMMO.UID, ItemList.ITEM_PISTOL_AMMO.InventoryStackSize))
            Debug.Log("Failed to add start item");

        if (!Inventory.AddItem(ItemList.ITEM_EXPLOSIVE_TIMED.UID, 64))
            Debug.Log("Failed to add start item");


        if (!Inventory.AddItem(ItemList.ITEM_AK47.UID, 1))
            Debug.Log("Failed to add start item");

        if (!Inventory.AddItem(ItemList.ITEM_MAC10.UID, 1))
            Debug.Log("Failed to add start item");
        Inventory.RemoveItem(ItemList.ITEM_LOCKPICK.UID, 1);

        WeaponController.Instance.ChangeWeaponSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMove(Vector2 dir, float speed)
    {
        rb.MovePosition(rb.position + dir.normalized * speed * Time.fixedDeltaTime);
    }

    public void Interact(Vector2 lookDir, uint itemID)
    {

        RaycastHit2D hit = Physics2D.Raycast(rb.position, lookDir, 2, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
        if (hit.collider == null)
        {
            return;
        }
        Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
        if(inter == null)
        {
            Debug.LogError("GameObject with interactable layer does not have script Interactable");
        }
        if (inter.Interact(itemID))
        {
            PlayerController.Instance.CurrentInteractable = inter;
            if(itemID != 0)
                ItemController.Instance.Use(ItemList.AllItems[itemID], inter.transform.position);
        }
        else
            PlayerController.Instance.CancelCurrentInteractable();
    }

    public void Attack(ItemInfo currentItem)
    {
        if (currentItem == null) 
            return;
        WeaponController.Instance.Shoot(currentItem.UID);
    }

    public void TakeDown(Vector2 lookDir)
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + lookDir * 0.5f, lookDir, 0.5f);
        if(hit.collider.CompareTag("humanoid"))
        {
            hit.collider.GetComponent<AI>().Incapacitate();
        }
    }

    public void UseItem(Vector2 lookDir)
    {
        var item = Inventory.Instance.GetCurrentItem();
        if (item != null)
        {
            if (item.ItemType == ItemType.Usable)
            {
                //PlayerMotor.Instance.UseItem(currentSelectedItem);
                PlayerController.Instance.CancelCurrentInteractable();
                ItemController.Instance.Use(item, transform.position);
            }
            else if (item.ItemType != ItemType.Usable)
            {
                PlayerController.Instance.CancelCurrentInteractable();
                Interact(lookDir, item.UID);
            }
        }
    }
}

