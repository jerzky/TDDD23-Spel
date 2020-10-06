using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public abstract class Gun : UsableItem
{
    protected float muzzleVelocity = 20f;
    protected float bulletMaxTravelDistance = 30f;
    protected uint ammoUID;
    protected AudioClip noAmmoAudio;
    protected AudioClip reloadAudio;
    protected AudioClip shootAudio;
    protected float rateOfFire;
    protected int magSize;
    protected int currentMagSize = 0;
    protected float cantShootTimer;
    protected GameObject bulletPrefab;
    protected float reloadSpeed;
    protected int damage;

    public override uint Use(ItemInfo item, Vector3 pos)
    {
        if (cantShootTimer > 0)
            return 0;
        uint temp = OutOfAmmo();
        if (temp == 0)
        {
            Shoot(item.SoundRadius);
            cantShootTimer = rateOfFire;
            WeaponController.Instance.AudioSource.clip = shootAudio;
            WeaponController.Instance.AudioSource.Play();
        }
        else if (temp == 1)
        {
            Reload();
        }
        return 0;
    }
    public uint Shoot(float soundRadius)
    {
        SoundController.Instance.GenerateSound(new Sound(WeaponController.Instance.WeaponEnd.transform.position, soundRadius, Sound.SoundType.Weapon));
        var bullet = Instantiate(bulletPrefab, WeaponController.Instance.WeaponEnd.transform.position, WeaponController.Instance.WeaponEnd.transform.rotation, WeaponController.Instance.bulletHolder.transform);
        var direction = WeaponController.Instance.WeaponEnd.transform.position - PlayerController.Instance.transform.position;
        bullet.GetComponent<Bullet>().SetBulletInfo(muzzleVelocity, bulletMaxTravelDistance, damage, direction, Bullet.ShooterType.Player);
        return 1;
    }
    public uint Reload()
    {
        uint temp = Inventory.Instance.GetItemCount(ammoUID);
        if (temp <= 0)
        {
            WeaponController.Instance.AudioSource.clip = noAmmoAudio;
            WeaponController.Instance.AudioSource.Play();
        }
        else
        {
            WeaponController.Instance.AudioSource.clip = reloadAudio;
            WeaponController.Instance.AudioSource.Play();
            currentMagSize = (temp >= magSize) ? magSize : (int)temp;
            cantShootTimer = reloadSpeed;
            Inventory.Instance.UpdateCurrentWeaponMag();
        }
        return 0;
    }
    public uint OutOfAmmo()
    {
        if (Inventory.Instance.GetItemCount(ammoUID) > 0)
        {
            if (currentMagSize-- <= 0)
                return 1;
            Inventory.Instance.RemoveItem(ammoUID, 1);
            return 0;
        }
        
        WeaponController.Instance.AudioSource.clip = noAmmoAudio;
        WeaponController.Instance.AudioSource.Play();
        return 2;
    }

    public override void Cancel()
    {
        
    }

    public int Ammo()
    {
        Debug.Log(currentMagSize);
        return currentMagSize;
    }
}
