using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Items;

public abstract class Openable : Interactable
{
    [SerializeField]
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
                currentItem = itemUID;
                UnLock();
                return true;
            }
            else
            {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Locked"),  new Vector3(transform.position.x, transform.position.y, -5f));
            }
        }
        else if(itemUID == 0)
        {
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
