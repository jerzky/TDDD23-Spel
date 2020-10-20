using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    float speed = 5f;
    float sneakMultiplier = 0.5f;
    public Vector2 MovementDirection { get; private set; }

    public float CurrentSpeed { get; private set; } = 5f;

    public Vector2 lookDir = Vector2.zero;

    Sprite[] playerSprites = new Sprite[4];
    SpriteRenderer sr;

    EventSystem EventSystem;
    uint currentSelectedItem = 0;
    public Interactable CurrentInteractable;

    uint[,] actionToKeys;
    public uint[,] ActionToKeys { get => actionToKeys; }
    private ControlAction[] itemBarActions = { ControlAction.Itembar_1, ControlAction.Itembar_2, ControlAction.Itembar_3, ControlAction.Itembar_4, ControlAction.Itembar_5, ControlAction.Itembar_6, ControlAction.Itembar_7, ControlAction.Itembar_8 };
    public ControlAction[] ItemBarActions { get => itemBarActions; }

    ItemController itemController;
    [SerializeField]
    GameObject heartbeat;

    SimpleTimer healthResetTimer = new SimpleTimer(5f);
    public bool IsHostile { 
        get
        {
            if (Inventory.Instance.GetCurrentItem() == null)
                return false;
            return Inventory.Instance.GetCurrentItem().ItemType == ItemType.Weapon;
        }
    }
    
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

        if(healthResetTimer.TickAndReset())
        {
            heartbeat.SetActive(false);
            GeneralUI.Instance.Health = 100;
        }
    }

    private void FixedUpdate()
    {
        if (MovementDirection != Vector2.zero)
        {
            CancelCurrentInteractable();
            PlayerMotor.Instance.PlayerMove(MovementDirection.normalized, CurrentSpeed);
            MovementDirection = Vector2.zero;
            CurrentSpeed = speed;
            GetComponent<AudioSource>().enabled = true;

        }
        else
            GetComponent<AudioSource>().enabled = false;

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    bool GetInput(Func<KeyCode, bool> func, ControlAction ca)
    {
        return func((KeyCode)actionToKeys[(uint)ca, 0]) || func((KeyCode)actionToKeys[(uint)ca, 1]);
    }

    void HandleKeyInputs()
    {
        if (GetInput(Input.GetKey, ControlAction.Right))
        {
            MovementDirection += Vector2.right;
            lookDir = Vector2.right;
            sr.sprite = playerSprites[3];
        }

        if (GetInput(Input.GetKey, ControlAction.Left))
        {
            MovementDirection += Vector2.left;
            lookDir = Vector2.left;
            sr.sprite = playerSprites[2];
        }

        if (GetInput(Input.GetKey, ControlAction.Down))
        {
            MovementDirection += Vector2.down;
            lookDir = Vector2.down;
            sr.sprite = playerSprites[1];
        }

        if (GetInput(Input.GetKey, ControlAction.Up))
        {
            MovementDirection += Vector2.up;
            lookDir = Vector2.up;
            sr.sprite = playerSprites[0];
        }

        if (GetInput(Input.GetKey, ControlAction.Sneak))
        {
            CurrentSpeed = speed * sneakMultiplier;
        }
       


        if (GetInput(Input.GetKeyDown, ControlAction.Interact))
        {
            PlayerMotor.Instance.Interact(lookDir, 0);
        }

        if (GetInput(Input.GetKeyDown, ControlAction.UseItem))
        {
            PlayerMotor.Instance.UseItem(lookDir);
        }


        if (GetInput(Input.GetKeyDown, ControlAction.Inventory))
        {
            PlayerMotor.Instance.Inventory.OpenInventory();
        }

        if (GetInput(Input.GetKeyDown, ControlAction.Reload))
        {
            PlayerMotor.Instance.Reload();
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

        // HANDLES UI CLICKS
        if (GetInput(Input.GetKeyDown, ControlAction.Shoot))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // over UI element
                if(EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.CompareTag("infobox"))
                    Inventory.Instance.SelectItem();

            }
        }
        // HANDLES INGAME CLICKS
        if (GetInput(Input.GetKey, ControlAction.Shoot))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                // NOT over UI element
                Inventory.Instance.DeSelectItem();
                bool storeIsOpen = StoreController.Instance.IsOpen();
                CancelCurrentInteractable();
                // attack?
                if (!storeIsOpen)
                    PlayerMotor.Instance.Attack(Inventory.Instance.GetCurrentItem());
                
            }
        }

        if(GetInput(Input.GetKeyDown, ControlAction.TakeDown))
        {
            PlayerMotor.Instance.TakeDown(lookDir);
        }

        if (Input.GetKeyDown(KeyCode.F1))
            GameController.Instance.SaveGame();
    }

    public void CancelCurrentInteractable()
    {
        if (CurrentInteractable != null)
        {
            CurrentInteractable.Cancel();
            CurrentInteractable = null;
        }
        //ItemController.Instance.CancelCurrentItem();
    }

    public void Injure(int damage)
    {
        healthResetTimer.Reset();
        GeneralUI.Instance.Health -= damage;
        if (GeneralUI.Instance.Health <= 0)
        {
            return;
            // you lost motherfucker
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            #endif
        }
        else if(GeneralUI.Instance.Health <= 50)
        {
            heartbeat.SetActive(true);
        }
    }
}
