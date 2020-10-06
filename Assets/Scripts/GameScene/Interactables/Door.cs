using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Door : Openable
{
    protected bool timerActive = false;
    bool moving = false;
    bool open = false;
    Vector3 originalPosition;
    Vector2 openingDir = Vector2.left;
    float doorSpeed = 1f;
    protected float timeLeft = 0f;
    public float timerMultiplier = 1f;
    [SerializeField]
    public bool isVertical = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3(0, 0, -1);
        originalPosition = transform.position;
        if(isVertical)
        {
            openingDir = Vector2.up;
        }
        AssignUnlockItems(new HashSet<uint> { ItemList.ITEM_LOCKPICK.UID, ItemList.ITEM_DRILL.UID });
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
            Timer();
        else if (moving)
            MoveDoor();
    }

    public override void AssignUnlockItems(HashSet<uint> set)
    {
        unlockItems = set;
    }

    public void Timer()
    {
        if (timeLeft < 0)
        {
            Cancel();
            isLocked = false;
        }
        else
            timeLeft -= Time.deltaTime;
    }

    public virtual void MoveDoor()
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

    public bool IsOpen()
    {
        if (moving)
            return false;
        return open;
    }

    public bool IsClosed()
    {
        if (moving)
            return false;
        return !open;
    }

    public override void Cancel()
    {
        timerActive = false;
        LoadingCircle.Instance.StopLoading();
        currentItem = 0;
    }

    public override void Open()
    {
        moving = true;
    }

    public void Close()
    {
        if(open)
        {
            moving = true;
        }
    }

    public override void UnLock()
    {
        timeLeft = ItemList.AllItems[currentItem].AverageUseTime * timerMultiplier;
        LoadingCircle.Instance.StartLoading();
        timerActive = true;
    }
}
