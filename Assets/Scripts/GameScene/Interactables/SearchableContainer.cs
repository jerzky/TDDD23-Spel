using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchableContainer : Openable
{
    LinkedList<int> items = new LinkedList<int>();
    float timeLeft = 0f;
    bool timerActive = false;
    SimpleTimer openedTimer = new SimpleTimer(300f);
    bool opened = false;
    // Start is called before the first frame update
    void Start()
    {
        AssignUnlockItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
            Timer();

        openedTimer.Tick();
    }

    public override void AssignUnlockItems()
    {
        unlockItems = new HashSet<uint> { ItemList.ITEM_LOCKPICK.UID, ItemList.ITEM_DRILL.UID };
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
        if(Random.Range(0, 10) < 1 && openedTimer.Done)
        {
            GeneralUI.Instance.Credits += (int)(100f * Random.Range(0.5f, 2f));
            openedTimer.Reset();
        }
    }

    public override void UnLock()
    {
        timeLeft = ItemList.AllItems[currentItem].AverageUseTime;
        LoadingCircle.Instance.StartLoading();
        timerActive = true;
    }

    public override string Name()
    {
        return "Searchable Container";
    }
}
