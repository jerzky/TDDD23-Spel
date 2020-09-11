using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchableContainer : Interactable
{
    bool isLocked = true;
    LinkedList<int> items = new LinkedList<int>();
    uint currentItem = 0;
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

    public void Timer()
    {
        if (timeLeft < 0)
        {
            timerActive = false;
            isLocked = false;
            LoadingCircle.Instance.StopLoading();
            OpenContainer();
        }
        else
            timeLeft -= Time.deltaTime;
    }

    public override bool Interact(uint itemIndex)
    {
        if(isLocked)
        {
            if (itemIndex == ItemList.ITEM_LOCKPICK.UID)
            {
                currentItem = ItemList.ITEM_LOCKPICK.UID;
                timeLeft = ItemList.ITEM_LOCKPICK.AverageUseTime;
                LoadingCircle.Instance.StartLoading();
                timerActive = true;
                Debug.Log("Attempting LockPicking");
                return true;
            }
        }
        else
        { 
            OpenContainer();
        }
        
        return false;
    }

    public override void Cancel()
    {
        timerActive = false;
        LoadingCircle.Instance.StopLoading();
        currentItem = 0;
    }

    public void OpenContainer()
    {
        // somehow give player the items in the container.
        Debug.Log("container open");
    }
}
