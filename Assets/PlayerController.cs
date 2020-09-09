using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 20f;
    float sneakMultiplier = 0.5f;
    Vector2 lookDir = Vector2.zero;
    bool walking = false;

    Sprite[] playerSprites = new Sprite[4];
    SpriteRenderer sr;


   
    // Start is called before the first frame update
    void Start()
    {
        playerSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[25];
        playerSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[24];
        playerSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[26];
        playerSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[27];
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = playerSprites[0];
 
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyInputs();
    }

    void HandleKeyInputs()
    {
        Vector2 dir = Vector2.zero;
        float currentSpeed = speed;




        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
            lookDir = Vector2.right;
            sr.sprite = playerSprites[3];
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
            lookDir = Vector2.left;
            sr.sprite = playerSprites[2];
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
            lookDir = Vector2.down;
            sr.sprite = playerSprites[1];
        }

        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
            lookDir = Vector2.up;
            sr.sprite = playerSprites[0];
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = currentSpeed * sneakMultiplier;
        }
        if (dir != Vector2.zero)
        {
            PlayerMotor.Instance.PlayerMove(dir, currentSpeed);
        }
        else
            walking = false;
            


        if(Input.GetKeyDown(KeyCode.E))
        {
            PlayerMotor.Instance.Interact(lookDir);
        }

        if (Input.GetKey(KeyCode.F))
        {
           //PlayerMotor.Instance.EnterCar(lookDir);
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            // Open inventory
        }
        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };
        for (uint i = 0; i < keyCodes.Length; i++)
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                PlayerMotor.Instance.UseItemFromInventory(i, lookDir);
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
