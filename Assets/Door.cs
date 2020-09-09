using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Door : Interactable
{
    bool timerActive = false;
    bool isLocked = true;
    bool moving = false;
    bool open = false;
    Vector3 originalPosition;
    Vector2 openingDir = Vector2.left;
    float doorSpeed = 1f;
    uint currentItem = 0;
    float timeLeft = 0f;

    // Start is called before the first frame update
    void Start()
    {
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
        if (timerActive)
            Timer();
        else if (moving)
            MoveDoor();
    }

    public void Timer()
    {
        if (timeLeft < 0)
        {
            timerActive = false;
            Debug.Log("DOOR OPEN");
            isLocked = false;
            LoadingCircle.Instance.StopLoading();
        }
        else
            timeLeft -= Time.deltaTime;
    }

    public void MoveDoor()
    {
        Vector3 dir = openingDir;
        if (open)
            dir = openingDir * -1;

        transform.position += dir * doorSpeed * Time.deltaTime;
        if (Mathf.Abs((transform.position - originalPosition).magnitude) > 1)
        {
            moving = false;
            open = !open;
            transform.position = originalPosition + dir;
            originalPosition = transform.position;
        }
    }

    public override bool Interact(uint itemIndex)
    {
        if(isLocked)
        {
            if(itemIndex == ItemList.ITEM_LOCKPICK.UID)
            {
                currentItem = ItemList.ITEM_LOCKPICK.UID;
                timeLeft = ItemList.ITEM_LOCKPICK.AverageUseTime;
                LoadingCircle.Instance.StartLoading();
                timerActive = true;
                Debug.Log("Attempting LockPicking");
                return true;
            }
            Debug.Log("Door is locked.");
            return false;
        }
       else
       {
            moving = true;
       }
        return false;
    }

    public override void Cancel()
    {
        timerActive = false;
        LoadingCircle.Instance.StopLoading();
        currentItem = 0;
    }
}
