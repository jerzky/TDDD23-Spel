using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityStation : Interactable
{
    // Start is called before the first frame update

    public bool IsMonitored { get; set; }

    void Start()
    {
        IsMonitored = true; // For testing
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Interact(uint itemIndex)
    {
        IsMonitored = !IsMonitored;
        return true;
    }

    public override void Cancel()
    {
        
    }
}
