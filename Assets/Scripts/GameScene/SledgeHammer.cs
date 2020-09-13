using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class SledgeHammer : Weapon
{
    bool animationActive = false;
    float maxRotation = 135f;
    float currentRotation = 0f;
    float rotationSpeed = 400f;
    Vector3 dir;
    public SledgeHammer(ItemInfo info) : base(info)
    {

    }
    public override bool Use(GameObject go)
    {
        if(!animationActive)
        {
            animationActive = true;
            go.transform.Rotate(new Vector3(maxRotation, 0f, 0f));
            dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - go.transform.position;
        }

        
        go.transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0f, 0f));
        currentRotation += rotationSpeed * Time.deltaTime;
        if (currentRotation > maxRotation)
        {
            animationActive = false;
            currentRotation = 0f;
            RaycastHit2D hit = Physics2D.Raycast(go.transform.position, dir, 0.5f);
            
            if (hit.collider != null)
            {
                BreakableController.Instance.HitObject(hit.collider.gameObject, info.UID);
                WeaponController.Instance.AudioSource.clip = Resources.Load<AudioClip>("Sounds/Hit2");
                WeaponController.Instance.AudioSource.Play();
            }
            
            return true;
        }

        return false;
    }
}
