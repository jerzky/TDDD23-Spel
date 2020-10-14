using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Assets.Items;
using System.Net.Sockets;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public GameObject weaponGO;
    [SerializeField]
    public WeaponFire WeaponEnd;
    [SerializeField]
    public GameObject bulletHolder;
    [SerializeField]
    public AudioSource AudioSource;

    GameObject playerGO;

    SortedDictionary<uint, Weapon> weapons = new SortedDictionary<uint, Weapon>();

    uint currentWeapon = 0;
    bool animationActive = false;
    int debugCounter = 0;
    public bool AnimationActive { get { return animationActive; } set { animationActive = value; } }
    GameObject animationObject;


    public static WeaponController Instance;
    private readonly List<Bullet> ActiveBullets = new List<Bullet>();


    Vector3 playerOffset = new Vector3(-0.08f, 0.1f, 0f);
    bool WeaponEquiped = true;
    float previousAngle = 0f;

    public SimpleTimer cantShootTimer = new SimpleTimer(0f);

    private bool changeWeaponSpriteWhenPossible = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        playerGO = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (WeaponEquiped)
            HandleWeaponRotation();


        cantShootTimer.Tick();
        if (changeWeaponSpriteWhenPossible && cantShootTimer.Done)
        {
            changeWeaponSpriteWhenPossible = false;
            ChangeWeaponSprite();
        }
    }

    public void HandleWeaponRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 player = playerGO.transform.position + playerOffset;

        float xDis = mousePosition.x - player.x;
        float yDis = mousePosition.y - player.y;

        float angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;


        weaponGO.transform.RotateAround(playerGO.transform.position + playerOffset, Vector3.forward, (angle - previousAngle));
        if ((previousAngle <= 0 && angle >= 0) || previousAngle > 0 && angle < 0)
            weaponGO.transform.Rotate(Vector3.up, 180f);

        previousAngle = angle;
    }



    public void Shoot(uint weaponUID)
    {
        if (AnimationActive || !cantShootTimer.Done || (ItemList.AllItems[weaponUID].ItemType != ItemType.Weapon && ItemList.AllItems[weaponUID].ItemType != ItemType.MeleeWeapon)) 
            return;

        currentWeapon = weaponUID;
        ItemController.Instance.Use(ItemList.AllItems[weaponUID], playerGO.transform.position);
    }

    public void ChangeWeaponSprite()
    {

        if(!cantShootTimer.Done)
        {
            changeWeaponSpriteWhenPossible = true;
            return;
        }

        ItemInfo info = Inventory.Instance.GetCurrentItem();
        if (info == null)
            return;
        if (info.ItemType == ItemType.Weapon || info.ItemType == ItemType.MeleeWeapon)
        {
            weaponGO.GetComponent<SpriteRenderer>().enabled = true;
            weaponGO.GetComponent<SpriteRenderer>().sprite = info.Sprite;
        }  
        else
            weaponGO.GetComponent<SpriteRenderer>().enabled = false;
    }


}
