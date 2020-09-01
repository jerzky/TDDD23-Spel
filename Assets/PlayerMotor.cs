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
        Inventory.AddItem(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMove(Vector2 dir, float speed)
    {
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
    }

    public void Interact(Vector2 lookDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, lookDir, 2, LayerMask.GetMask("interactables"), -Mathf.Infinity, Mathf.Infinity);
        if (hit.collider == null)
        {
            return;
        }
        Debug.Log(hit.collider.name);
        if(hit.collider.tag == "interactable")
        {
            hit.collider.gameObject.GetComponent<Interactable>().Interact(0);
          
        }
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
}
