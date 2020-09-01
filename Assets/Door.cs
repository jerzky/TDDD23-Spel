using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Door : Interactable
{

    bool isLocked = false;
    bool moving = false;
    bool open = false;
    Vector3 originalPosition;
    Vector2 openingDir = Vector2.left;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = openingDir;
        if (open)
            dir = openingDir * -1;

        if (moving)
        {
            transform.position += dir * 0.5f * Time.deltaTime;
            if (Mathf.Abs((transform.position - originalPosition).magnitude) > 1)
            {
                moving = false;
                open = !open;
                transform.position = originalPosition + dir;
                originalPosition = transform.position;
            }
        }
    }

    public override void Interact(int itemIndex)
    {
        moving = true;
    }
}
