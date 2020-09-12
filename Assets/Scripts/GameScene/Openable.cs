using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public abstract class Openable : Interactable
{

    protected bool isLocked = true;
    protected HashSet<uint> unlockItems = new HashSet<uint>();
    protected uint currentItem = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void AssignUnlockItems(HashSet<uint> set);

    public override bool Interact(uint itemUID)
    {
        if (isLocked)
        {
            if (unlockItems.Contains(itemUID))
            {
                Debug.Log("OPENABLE IS UNLOCKING");
                currentItem = itemUID;
                UnLock();
                return true;
            }
            else
                Debug.Log("OPENABLE IS LOCKED");
        }
        else if(itemUID == 0)
        {
            Debug.Log("OPENABLE IS OPENING");
            Open();
            return true;
        }
        return false;
    }

    public override void Cancel()
    {
        currentItem = 0;
    }

    public abstract void Open();

    public abstract void UnLock();

}
