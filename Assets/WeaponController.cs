using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    GameObject currentWeapon;
    
    [SerializeField]
    WeaponFire WeaponEnd;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    public GameObject bulletHolder;


    public static WeaponController Instance;
    private readonly List<Bullet> ActiveBullets = new List<Bullet>();


    Vector3 playerOffset = new Vector3(-0.08f, 0.2f, 0f);
    bool WeaponEquiped = true;
    float previousAngle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!WeaponEquiped)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 player = gameObject.transform.position + playerOffset;

        float xDis = mousePosition.x - player.x;
        float yDis = mousePosition.y - player.y;

        float angle = -Mathf.Atan2(xDis, yDis) * 180 / Mathf.PI;

        
        currentWeapon.transform.RotateAround(transform.position + playerOffset, Vector3.forward, (angle - previousAngle));
        if ((previousAngle < 0 && angle > 0) || previousAngle > 0 && angle < 0)
            currentWeapon.transform.Rotate(Vector3.up, 180f);

        previousAngle = angle;
    }


    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, WeaponEnd.transform.position, WeaponEnd.transform.rotation, bulletHolder.transform);
    }
}
