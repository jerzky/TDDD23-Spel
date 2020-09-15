using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeWeapon : UsableItem
{
    protected GameObject animationObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override uint Use(ItemInfo item, Vector3 pos)
    {
        animationObject = new GameObject("MeleeAnimiationObject");
        animationObject.transform.position = WeaponController.Instance.weaponGO.transform.position;
        animationObject.transform.rotation = WeaponController.Instance.weaponGO.transform.rotation;
        animationObject.transform.parent = WeaponController.Instance.transform;
        animationObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(item.IconPath);
        WeaponController.Instance.weaponGO.GetComponent<SpriteRenderer>().enabled = false;
        WeaponController.Instance.AnimationActive = true;
        Swing();
        return 0;
    }

    protected void MeleeUpdate()
    {
        if (WeaponController.Instance.AnimationActive)
            if (SwingAnimation())
            {
                Debug.Log("CHANGING ANIMATION ACTIVE TO FALSE IN MELEEWEAPON:");
                WeaponController.Instance.AnimationActive = false;
                if (Inventory.Instance.GetCurrentItem().ItemType == ItemType.Weapon)
                    WeaponController.Instance.weaponGO.GetComponent<SpriteRenderer>().enabled = true;
            }
    }

    public abstract uint Swing();
    public abstract bool SwingAnimation();

}
