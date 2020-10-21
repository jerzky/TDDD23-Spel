using System;
using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoliceController : MonoBehaviour
{
    public static PoliceController Instance;
    public static List<Police> AllPolice = new List<Police>();
    private int _currentlyWaiting = 0;



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NotifyPolice(Building building)
    {
        SpawnPolice(building, State.GotoCoverEntrance, ActionE.GotoCoverEntrance);
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

    private static void SpawnPolice(Building building, State state, ActionE action)
    {
    //    for(var i = 0; i < 5; i++)
       //     Police.Generate(building.PoliceSpawnPoint + new Vector2(i, 0), state, action, building);
       PoliceCar.Generate(building);
    }

    public void AlertAll(Vector2 pos, Police reporter = null)
    {
        foreach (var police in AllPolice)
        {
            if(police != reporter)
                police.Alert(pos, AlertIntensity.ConfirmedHostile);
        }
    }


}
