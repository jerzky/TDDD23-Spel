using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Constructor() 
    {
        tag = "interactable";
    }
    public abstract void Interact(int itemIndex);
}
