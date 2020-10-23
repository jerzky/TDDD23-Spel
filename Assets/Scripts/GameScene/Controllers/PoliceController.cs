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

    private static readonly Dictionary<Building, PoliceCar> CurrentCars = new Dictionary<Building, PoliceCar>();
    Dictionary<BuildingType, SimpleTimer> _buildingTimers = new Dictionary<BuildingType, SimpleTimer>();
    
                

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        foreach (var v in BuildingController.Instance.Buildings)
            _buildingTimers.Add(v.BuildingType, new SimpleTimer(15f));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var v in _buildingTimers)
            v.Value.Tick();
    }

    public static void AddBuilding(Building b)
    {
        CurrentCars.Add(b, null);
    }
    
    public void NotifyPolice(Vector2 position)
    {
        
    }

    public void NotifyPolice(Building building)
    {
        if(_buildingTimers[building.BuildingType].Done)
        {
            SpawnPolice(building, State.GotoCoverEntrance, ActionE.GotoCoverEntrance);
            _buildingTimers[building.BuildingType].Reset();
        }
    }

    public void CallPolice(Vector2 callPosition, Building building)
    {
        var cellphoneJammers = FindObjectsOfType<CellPhoneJammer_Interactable>();
        foreach (var v in cellphoneJammers)
        {
            if (Vector2.Distance(v.transform.position, callPosition) < v.Distance)
                return;
        }

        Building closest = building;
        if(closest == null)
        {
            int closestIndex = 0;
            foreach (var v in BuildingController.Instance.Buildings)
                foreach (var e in v.Entrances)
                {
                    int i = 0;
                    if (Vector2.Distance(e.Location, callPosition) < Vector2.Distance(closest.Entrances[closestIndex].Location, callPosition))
                    {
                        closestIndex = i;
                        closest = v;
                    }
                    i++;
                }
        }
        
        NotifyPolice(closest);
    }
    

    private static void SpawnPolice(Building building, State state, ActionE action)
    {
    //    for(var i = 0; i < 5; i++)
       //     Police.Generate(building.PoliceSpawnPoint + new Vector2(i, 0), state, action, building);

       if (!CurrentCars.ContainsKey(building))
       {
           Debug.LogError("You have not added this building to the policecontroller!");
           return;
       }
       var newCar = PoliceCar.Generate(building).GetComponent<PoliceCar>();
        if (CurrentCars[building] == null)
       {
          
           CurrentCars[building] = newCar;
           return;

       }
        CurrentCars[building].Transfer(newCar);
        CurrentCars[building] = newCar;

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
