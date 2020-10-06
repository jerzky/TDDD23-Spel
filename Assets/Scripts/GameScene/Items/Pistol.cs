﻿using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pistol : Gun
{

    // Start is called before the first frame update
    void Start()
    {
        ammoUID = ItemList.ITEM_PISTOL_AMMO.UID;
        noAmmoAudio = Resources.Load<AudioClip>("Sounds/noammo");
        reloadAudio = Resources.Load<AudioClip>("Sounds/reload");
        shootAudio = Resources.Load<AudioClip>("Sounds/gunfire");
        rateOfFire = 0.35f;
        reloadSpeed = 1f;

        magSize = 16;
        damage = ItemList.ITEM_SILENCED_PISTOL.HumanDamage;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/bullet");
        if (bulletPrefab == null)
            Debug.Log("BULLET NULL????");
    }

    void Update()
    {
    }
}
