using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour
{
    public static PoliceController Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallPolice(Vector2 callPosition)
    {
        var cellphoneJammers = FindObjectsOfType<CellPhoneJammer_Interactable>();
        foreach(var v in cellphoneJammers)
        {
            if (Vector2.Distance(v.transform.position, callPosition) < v.Distance)
                return;
        }
    }
}
