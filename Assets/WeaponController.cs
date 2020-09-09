﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Assets.Items;
using System.Net.Sockets;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    GameObject weaponGO;
    
    [SerializeField]
    WeaponFire WeaponEnd;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    public GameObject bulletHolder;

    SortedDictionary<uint, Weapon> weapons = new SortedDictionary<uint, Weapon>();

    uint currentWeapon = 0;
    bool animationActive = false;
    GameObject animationObject;


    public static WeaponController Instance;
    private readonly List<Bullet> ActiveBullets = new List<Bullet>();


    Vector3 playerOffset = new Vector3(-0.08f, 0.2f, 0f);
    bool WeaponEquiped = true;
    float previousAngle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        weapons.Add(ItemList.ITEM_SLEDGEHAMMER.UID, new SledgeHammer(ItemList.ITEM_SLEDGEHAMMER));
    }

    // Update is called once per frame
    void Update()
    {
        if (WeaponEquiped)
            HandleWeaponRotation();

        if(animationActive)
            if (weapons[currentWeapon].Use(animationObject))
            {
                weaponGO.GetComponent<SpriteRenderer>().enabled = true;
                animationActive = false;
                currentWeapon = 0;
                Destroy(animationObject);
            }

    }

    public void HandleWeaponRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 player = gameObject.transform.position + playerOffset;

        float xDis = mousePosition.x - player.x;
        float yDis = mousePosition.y - player.y;

        float angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;


        weaponGO.transform.RotateAround(transform.position + playerOffset, Vector3.forward, (angle - previousAngle));
        if ((previousAngle <= 0 && angle >= 0) || previousAngle > 0 && angle < 0)
            weaponGO.transform.Rotate(Vector3.up, 180f);

        previousAngle = angle;
    }


    public void Shoot(uint weaponUID)
    {
        if (animationActive) 
            return;

        if (weaponUID == ItemList.ITEM_SLEDGEHAMMER.UID)
        {
            currentWeapon = ItemList.ITEM_SLEDGEHAMMER.UID;
            animationActive = true;
            animationObject = Instantiate(weaponGO, weaponGO.transform.position, weaponGO.transform.rotation, transform);
            weaponGO.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(weaponUID == 3)
        {
            GameObject bullet = Instantiate(bulletPrefab, WeaponEnd.transform.position, WeaponEnd.transform.rotation, bulletHolder.transform);
        }
    }

    public void ChangeWeaponSprite()
    {
        ItemInfo info = Inventory.Instance.GetCurrentItem();
        if (info.WeaponType != WeaponType.None)
        {
            weaponGO.GetComponent<SpriteRenderer>().enabled = true;
            weaponGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(info.IconPath);
        }  
        else
            weaponGO.GetComponent<SpriteRenderer>().enabled = false;
    }
}
