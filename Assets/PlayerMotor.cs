using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMotor : MonoBehaviour
{

    public static PlayerMotor Instance;
    Rigidbody2D rb;
    private Inventory Inventory { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        Inventory = new Inventory(FindObjectOfType<ItemBar>());
        if (!Inventory.AddItem(ItemList.ITEM_LOCKPICK, 1))
            Debug.Log("Failed to add start item");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMove(Vector2 dir, float speed)
    {
        rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
        Debug.Log(Screen.height);
    }

    public void Interact(Vector2 lookDir)
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
        inter.Interact(0);
    }
    public void UseItem(uint index, Vector2 lookDir)
    {
        /*
          RaycastHit2D hit = Physics2D.Raycast(rb.position, lookDir, 2, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
          if (hit.collider == null)
          {
              if (item.MustInteract)
                  return;
              else
                  item.Use(0);
          }
          Debug.Log(hit.collider.name);
          if (hit.collider.tag == "interactable")
          {
              item.Interact(hit.collider.gameObject.GetComponent<Interactable>());
          }*/

    }
    public void UseItemFromInventory(uint inventorySpot, Vector2 lookDir)
    {

        var item = Inventory.ItemsVisible[inventorySpot];
     
        if (item == null)
        {
            Debug.Log(string.Format("There is no item in spot: {0}", inventorySpot));
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(rb.position, lookDir, 2, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
        if (hit.collider == null)
        {
            return;
        }
        Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
        if (inter == null)
        {
            Debug.LogError("GameObject with interactable layer does not have script Interactable");


        }
        inter.Interact(item.UID);
    }

}

