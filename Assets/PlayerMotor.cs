using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

    public static PlayerMotor Instance;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerMove(Vector2 dir, float speed)
    {
        rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
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
}
