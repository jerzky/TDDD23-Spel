using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    float speed = 5f;
    float sneakMultiplier = 0.5f;
    Vector2 movementDirection;
    float currentSpeed = 5f;
    Vector2 lookDir = Vector2.zero;

    Sprite[] playerSprites = new Sprite[4];
    SpriteRenderer sr;

    EventSystem EventSystem;
    uint currentSelectedItem = 0;
    public Interactable CurrentInteractable;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        playerSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[25];
        playerSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[24];
        playerSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[26];
        playerSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[27];
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = playerSprites[0];
        EventSystem = FindObjectOfType<EventSystem>();
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
            CancelCurrentInteractable();
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


        
        if (Input.GetKeyDown(KeyCode.E))
        {
            CancelCurrentInteractable();
            PlayerMotor.Instance.Interact(lookDir, currentSelectedItem);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerMotor.Instance.Inventory.RemoveItem(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerMotor.Instance.Inventory.AddItem(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            PlayerMotor.Instance.Inventory.OpenInventory();
        }
        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };
        for (uint i = 0; i < keyCodes.Length; i++)
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                currentSelectedItem = i;
                Inventory.Instance.UpdateCurrentItem(i);
                //PlayerMotor.Instance.UseItemFromInventory(i, lookDir);
                // Use/equip? item i
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            CancelCurrentInteractable();
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Inventory.Instance.SelectItem();
            }
            else
                Inventory.Instance.DeSelectItem();
        }
    }

    void CancelCurrentInteractable()
    {
        if (CurrentInteractable != null)
        {
            CurrentInteractable.Cancel();
            CurrentInteractable = null;
        }
    }
}
