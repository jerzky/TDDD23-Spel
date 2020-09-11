using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
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

    uint[,] actionToKeys;
    ControlAction[] itemBarActions = { ControlAction.Itembar_1, ControlAction.Itembar_2, ControlAction.Itembar_3, ControlAction.Itembar_4, ControlAction.Itembar_5, ControlAction.Itembar_6, ControlAction.Itembar_7, ControlAction.Itembar_8 };

    ItemController itemController;

    
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

        SortedDictionary<ControlAction, ControlInfo> atk = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
        actionToKeys = new uint[atk.Count, 2];
        foreach (var v in atk)
        {
            actionToKeys[(uint)v.Key, 0] = (uint)v.Value.KeyCode1;
            actionToKeys[(uint)v.Key, 1] = (uint)v.Value.KeyCode2;

        }




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

    bool GetInput(Func<KeyCode, bool> func, ControlAction ca)
    {
        return func((KeyCode)actionToKeys[(uint)ca, 0]) || func((KeyCode)actionToKeys[(uint)ca, 1]);
    }

    void HandleKeyInputs()
    {
        if (GetInput(Input.GetKey, ControlAction.Right))
        {
            movementDirection += Vector2.right;
            lookDir = Vector2.right;
            sr.sprite = playerSprites[3];
        }

        if (GetInput(Input.GetKey, ControlAction.Left))
        {
            movementDirection += Vector2.left;
            lookDir = Vector2.left;
            sr.sprite = playerSprites[2];
        }

        if (GetInput(Input.GetKey, ControlAction.Down))
        {
            movementDirection += Vector2.down;
            lookDir = Vector2.down;
            sr.sprite = playerSprites[1];
        }

        if (GetInput(Input.GetKey, ControlAction.Up))
        {
            movementDirection += Vector2.up;
            lookDir = Vector2.up;
            sr.sprite = playerSprites[0];
        }

        if (GetInput(Input.GetKey, ControlAction.Sneak))
        {
            currentSpeed = speed * sneakMultiplier;
        }



        if (GetInput(Input.GetKeyDown, ControlAction.Interact))
        {
            CancelCurrentInteractable();
            PlayerMotor.Instance.Interact(lookDir, 0);
        }

        if (GetInput(Input.GetKeyDown, ControlAction.UseItem))
        {
            var item = Inventory.Instance.GetCurrentItem();
            if(item.ItemType == ItemType.Usable)
            {
                //PlayerMotor.Instance.UseItem(currentSelectedItem);
                ItemController.Instance.Use(item, transform.position);
            }
            else if(item.ItemType == ItemType.None)
            {
                CancelCurrentInteractable();
                PlayerMotor.Instance.Interact(lookDir, currentSelectedItem);
            }
        }


        if (GetInput(Input.GetKeyDown, ControlAction.Inventory))
        {
            PlayerMotor.Instance.Inventory.OpenInventory();
        }

        for (uint i = 0; i < itemBarActions.Length; i++)
        {
            if (GetInput(Input.GetKeyDown, itemBarActions[i]))
            {
                currentSelectedItem = i;
                Inventory.Instance.UpdateCurrentItem(i);
                WeaponController.Instance.ChangeWeaponSprite();
            }
        }

        if (GetInput(Input.GetKeyDown, ControlAction.Shoot))
        {
            CancelCurrentInteractable();
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // over UI element
                Inventory.Instance.SelectItem();
            }
            else
            {
                // NOT over UI element
                Inventory.Instance.DeSelectItem();

                // attack?
                PlayerMotor.Instance.Attack(Inventory.Instance.GetCurrentItem());
            }
        }

        // temporary
 

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerMotor.Instance.Inventory.AddItem(1, 1);
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
