using Assets.Items;
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
        cantShootTimer = rateOfFire;
        reloadSpeed = 1f;

        magSize = 16;
        currentMagSize = 16;
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
