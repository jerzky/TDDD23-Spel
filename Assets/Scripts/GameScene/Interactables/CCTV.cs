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

    // Start is called before the first frame update
    void Start()
    {
        cameraSprites = Resources.LoadAll<Sprite>("Textures/camerasprites");
        GetComponent<SpriteRenderer>().sprite = cameraSprites[2];
        SetCameraLookDir(new Vector3(0f, 0f, -90f));
        originalRotation = transform.rotation.eulerAngles;
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
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
}
