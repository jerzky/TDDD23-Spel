using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchableContainer : Openable
{
    LinkedList<int> items = new LinkedList<int>();
    float timeLeft = 0f;
    bool timerActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
            Timer();
            
    }

    public override void AssignUnlockItems(HashSet<uint> set)
    {
        unlockItems = set;
    }

    public void Timer()
    {
        if (timeLeft < 0)
        {
            isLocked = false;
            Cancel();
        }
        else
            timeLeft -= Time.deltaTime;
    }

    public override void Cancel()
    {
        timerActive = false;
        LoadingCircle.Instance.StopLoading();
        currentItem = 0;
    }

    public override void Open()
    {
        // somehow give player the items in the container.
        Debug.Log("container open");
    }

    public override void UnLock()
    {
        timeLeft = ItemList.AllItems[currentItem].AverageUseTime;
        LoadingCircle.Instance.StartLoading();
        timerActive = true;
        Debug.Log("Attempting LockPicking");
    }

    
}
