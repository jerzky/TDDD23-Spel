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
    protected GameObject bulletPrefab;
    protected float reloadSpeed;
    protected int damage;

    public override uint Use(ItemInfo item, Vector3 pos)
    {
        if (!WeaponController.Instance.cantShootTimer.Done)
            return 0;
        uint temp = OutOfAmmo();
        if (temp == 0)
        {
            Shoot(item.SoundRadius);
            WeaponController.Instance.cantShootTimer.ResetTo(rateOfFire);
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
        var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bullet.transform.position;
        bullet.GetComponent<Bullet>().SetBulletInfo(muzzleVelocity, bulletMaxTravelDistance, damage, direction, Bullet.ShooterType.Player);
        return 1;
    }
    public uint Reload()
    {
        if (currentMagSize == magSize)
            return 0;
        if (!WeaponController.Instance.cantShootTimer.Done)
            return 0;

        uint temp = Inventory.Instance.GetItemCount(ammoUID);
        if (temp <= 0)
        {
            WeaponController.Instance.AudioSource.clip = noAmmoAudio;
            WeaponController.Instance.AudioSource.Play();
        }
        else
        {
            SoundController.Instance.GenerateSound(new Sound(WeaponController.Instance.weaponGO.transform.position, 3f, Sound.SoundType.Weapon));
            WeaponController.Instance.AudioSource.clip = reloadAudio;
            WeaponController.Instance.AudioSource.Play();
            if(currentMagSize < 0)
                currentMagSize = 0;
            int neededBullets = magSize - currentMagSize;
            currentMagSize += (temp >= neededBullets) ? neededBullets : (int)temp;
            Inventory.Instance.RemoveItem(ammoUID, (uint)neededBullets);
            WeaponController.Instance.cantShootTimer.ResetTo(reloadSpeed);
            Inventory.Instance.UpdateCurrentWeaponMag();
        }
        return 0;
    }
    public uint OutOfAmmo()
    {
        if (currentMagSize-- <= 0)
        {
            if (Inventory.Instance.GetItemCount(ammoUID) <= 0)
            {
                WeaponController.Instance.AudioSource.clip = noAmmoAudio;
                WeaponController.Instance.AudioSource.Play();
                return 2;
            }
            
            return 1;
        }
        Inventory.Instance.UpdateCurrentWeaponMag();
        return 0;
    }

    public override void Cancel()
    {
        
    }

    public int Ammo()
    {
        return currentMagSize;
    }
}
