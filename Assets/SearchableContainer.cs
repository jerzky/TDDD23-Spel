using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchableContainer : Interactable
{
    bool isLocked = false;
    int lockpick = 555; // set real value here
    LinkedList<int> items = new LinkedList<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Interact(uint itemIndex)
    {
        if(itemIndex == lockpick || !isLocked)
        {
            

            if(items.Count == 0)
            {
                Debug.Log("container empty");
            }
            else
            {
                Debug.Log("container opened");
            }
        }
        return true;
    }
}
