﻿using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Door : Interactable
{

    bool isLocked = true;
    bool moving = false;
    bool open = false;
    Vector3 originalPosition;
    Vector2 openingDir = Vector2.left;
    float doorSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "door";
        base.Constructor();
        transform.position += new Vector3(0, 0, -1);
        originalPosition = transform.position;
        if(GetComponent<SpriteRenderer>().sprite.name == "Doors_20") // temporary fix will only work for blue doors
        {
            openingDir = Vector2.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = openingDir;
        if (open)
            dir = openingDir * -1;

        if (moving)
        {
            transform.position += dir * doorSpeed * Time.deltaTime;
            if (Mathf.Abs((transform.position - originalPosition).magnitude) > 1)
            {
                moving = false;
                open = !open;
                transform.position = originalPosition + dir;
                originalPosition = transform.position;
            }
        }
    }

    public override bool Interact(uint itemIndex)
    {
        if(isLocked)
        {
            if(itemIndex == ItemList.ITEM_LOCKPICK)
            {
                //Random in the future?
                //and timer
                isLocked = false;
                // moving = true;

                LoadingCircle.Instance.StartLoading();
                Debug.Log("Door is unlocked.");
                return true;
            }
            Debug.Log("Door is locked.");
            return false;
        }
       else
       {
            moving = true;
       }
        return true;
    }
}
