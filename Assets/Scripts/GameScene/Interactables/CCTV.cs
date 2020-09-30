using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : Interactable
{
    bool isRotating = true;
    Sprite[] cameraSprites;

    float maxRotation = 45f;
    Vector3 direction = new Vector3(0f, 0f, 1f);
    float rotationSpeed = 10f;
    Vector3 originalRotation;
    Vector3 currentAngle = Vector3.zero;
    public enum LocationType { BANK, STORE };
    LocationType locationType;

    float timer = 0f;
    [SerializeField]
    float playerNoticedDelay = 1f;
    bool timerActive = false;
    [SerializeField]
    bool isInEmployeeAreaOnly = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraSprites = Resources.LoadAll<Sprite>("Textures/camerasprites");
        GetComponent<SpriteRenderer>().sprite = cameraSprites[2];
        SetCameraLookDir(new Vector3(0f, 0f, -90f));
        originalRotation = transform.rotation.eulerAngles;
        locationType = LocationType.BANK;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        if (timerActive)
            timer += Time.deltaTime;
        else if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            // player hostile and noticed
            HostilePlayerNoticed();
            timer = 0f;
        }    
    }

    void SetCameraLookDir(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    void RotateCamera()
    {
        currentAngle += direction * Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Euler(originalRotation + currentAngle);
        if(direction.z > 0 && currentAngle.z > 45|| direction.z < 0 && currentAngle.z < -45)
        {
            direction *= -1;
        }
    }

    public override void Cancel()
    {
        
    }

    public override bool Interact(uint itemIndex)
    {
        return false;
    }

    public void OnVisionEnter(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Check if player is hostile
            // this bool should be kept in some kind of controller
            bool playerHasBeenSeenAsHostileBefore = false;
            if (Inventory.Instance.GetCurrentItem().ItemType == ItemType.Weapon)
            {
                HostilePlayerNoticed();
            }
            else if (playerHasBeenSeenAsHostileBefore)
            {
                // start timer
                timerActive = true;
            }
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            
        }
    }

    public void OnVisionExit(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            timerActive = false;
        }
    }

    void HostilePlayerNoticed()
    {
        switch (locationType)
        {
            case LocationType.BANK:
                // Alarm guards
                // Call Police
                // Set off alarm
                break;
        }
    }
}
