using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public class SilencedPistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        ammoUID = ItemList.ITEM_PISTOL_AMMO.UID;
        noAmmoAudio = Resources.Load<AudioClip>("Sounds/noammo");
        reloadAudio = Resources.Load<AudioClip>("Sounds/reload");
        shootAudio = Resources.Load<AudioClip>("Sounds/silencedGunfire");
        rateOfFire = 0.35f;
        cantShootTimer = rateOfFire;
        reloadSpeed = 1f;

        magSize = 16;
        currentMagSize = 16;
        damage = ItemList.ITEM_SILENCED_PISTOL.HumanDamage;

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
