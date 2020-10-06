﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public class AK47 : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        ammoUID = ItemList.ITEM_PISTOL_AMMO.UID;
        noAmmoAudio = Resources.Load<AudioClip>("Sounds/noammo");
        reloadAudio = Resources.Load<AudioClip>("Sounds/ak_reload");
        shootAudio = Resources.Load<AudioClip>("Sounds/ak_fire");
        rateOfFire = (60f/600f);
        cantShootTimer = rateOfFire;
        reloadSpeed = 3.2f; // lowest reload time which allows sound to finish = 4.5f

        magSize = 30;
        damage = ItemList.ITEM_AK47.HumanDamage;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/bullet");
        if (bulletPrefab == null)
            Debug.Log("BULLET NULL????");
    }

    void Update()
    {
        if (cantShootTimer >= 0)
            cantShootTimer -= Time.deltaTime;
    }
}
