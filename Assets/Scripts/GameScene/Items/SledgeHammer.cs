using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class SledgeHammer : MeleeWeapon
{
    float maxRotation = 135f;
    float currentRotation = 0f;
    float rotationSpeed = 300f;
    Vector3 dir;

    void Start()
    {
    }

    void Update()
    {
        MeleeUpdate();  

    }
    public override bool SwingAnimation()
    {
        if(!WeaponController.Instance.AnimationActive)
        {
            Debug.Log("CHANGING ANIMATION ACTIVE TO TRUE IN SLEDGEHAMMER:");

            
        }


        animationObject.transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0f, 0f));
        currentRotation += rotationSpeed * Time.deltaTime;
        if (currentRotation > maxRotation)
        {
            currentRotation = 0f;
            RaycastHit2D hit = Physics2D.Raycast(animationObject.transform.position, dir, 0.5f);
            
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("breakable"))
                    BreakableController.Instance.HitObject(hit.collider.gameObject, ItemList.ITEM_SLEDGEHAMMER.UID);
                else if (hit.collider.CompareTag("humanoid"))
                    hit.collider.GetComponent<AI>().Injure(ItemList.ITEM_SLEDGEHAMMER.HumanDamage, hit.collider.transform.position - transform.position);
                else if (hit.collider.CompareTag("Player"))
                    PlayerController.Instance.Injure(ItemList.ITEM_SLEDGEHAMMER.HumanDamage);
            }
            Destroy(animationObject);
            return true;
        }

        return false;
    }

    public override uint Swing()
    {
        WeaponController.Instance.AudioSource.clip = Resources.Load<AudioClip>("Sounds/SledgeSwoosh");
        WeaponController.Instance.AudioSource.Play();
        animationObject.transform.Rotate(new Vector3(maxRotation, 0f, 0f));
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - animationObject.transform.position;
        SwingAnimation();
        return 0;
    }
}
