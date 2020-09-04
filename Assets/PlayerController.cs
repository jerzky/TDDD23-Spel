using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 5f;
    float sneakMultiplier = 0.5f;
    Vector2 movementDirection;
    float currentSpeed = 5f;
    Vector2 lookDir = Vector2.zero;

    Sprite[] playerSprites = new Sprite[4];
    SpriteRenderer sr;

<<<<<<< Updated upstream
=======
    private Inventory Inventory { get; set; }

>>>>>>> Stashed changes
    // Start is called before the first frame update
    void Start()
    {
        playerSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[25];
        playerSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[24];
        playerSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[26];
        playerSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[27];
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = playerSprites[0];
<<<<<<< Updated upstream
=======
        Inventory = new Inventory(FindObjectOfType<ItemBar>());
        Inventory.AddItem(0, 1);
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyInputs();
    }

    private void FixedUpdate()
    {
        if (movementDirection != Vector2.zero)
        {
            PlayerMotor.Instance.PlayerMove(movementDirection, currentSpeed);
            movementDirection = Vector2.zero;
            currentSpeed = speed;
        }
    }

    void HandleKeyInputs()
    {




        if (Input.GetKey(KeyCode.D))
        {
            movementDirection += Vector2.right;
            lookDir = Vector2.right;
            sr.sprite = playerSprites[3];
        }

        if (Input.GetKey(KeyCode.A))
        {
            movementDirection += Vector2.left;
            lookDir = Vector2.left;
            sr.sprite = playerSprites[2];
        }

        if (Input.GetKey(KeyCode.S))
        {
            movementDirection += Vector2.down;
            lookDir = Vector2.down;
            sr.sprite = playerSprites[1];
        }

        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += Vector2.up;
            lookDir = Vector2.up;
            sr.sprite = playerSprites[0];
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = speed * sneakMultiplier;
        }
            


        if(Input.GetKeyDown(KeyCode.E))
        {
            PlayerMotor.Instance.Interact(lookDir);
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            Inventory.OpenInventory();
        }
        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                // Use/equip? item i
            }
        }
        bool weaponEquiped = true;
        if(Input.GetKeyDown(KeyCode.Mouse0) && weaponEquiped)
        {
            // Shoot/hit?
        }
    }
}
