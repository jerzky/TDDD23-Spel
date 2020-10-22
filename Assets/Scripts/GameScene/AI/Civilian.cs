using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Civilian : AI
{
    float fun;
    float food;
    float cash;
    float foodGainMultiplier = 4f;
    float cashGainMultiplier = 4f;
    float funGainMultiplier = 4f;
    float ffcMax = 300f;
    BuildingType currentFFC = BuildingType.None;
    SimpleTimer afkTimer = new SimpleTimer(3f);
    bool afkTimerActive = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Actions.Add(ActionE.Flee, new Flee(this));
        Actions.Add(ActionE.Freeze, new Freeze(this, 10f));
        Health = 75;
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        IdleState = CurrentState;
        IdleAction = CurrentAction;
        AiType = AI_Type.Civilian;
        Sprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[8];
        Sprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[9];
        Sprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[10];
        Sprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[11];

        fun = UnityEngine.Random.Range(0, ffcMax);
        food = UnityEngine.Random.Range(0, ffcMax);
        cash = UnityEngine.Random.Range(0, ffcMax);
        Debug.Log("Food: " + food + " cash: " + cash);

        ChooseRoute();
    }

    protected override void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        var building = CurrentBuilding;
        if (building == null || CurrentState != State.FollowRoute || ((building.BuildingType != currentFFC)) ||
            (building.BuildingType == BuildingType.Apartment && (building as ApartmentBuilding).GetApartment(transform.position) == null)
            || Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < 35f)
        {
            afkTimerActive = false;
        }
        else
        {
            if(!afkTimerActive)
                afkTimer.Reset();
            afkTimerActive = true;
            
        }
        

        if(afkTimerActive)
        {
            afkTimer.Tick();
            if (afkTimer.Done)
                return;
        }

        base.FixedUpdate();
    }

    protected virtual void Update()
    {
        if(CurrentAction == ActionE.None)
        {
            currentFFC = BuildingType.None;
            ChooseRoute();
            return;
        }
        if(food > 0)
            food -= Time.deltaTime;
        if (fun > 0)
            fun -= Time.deltaTime;
        if (cash > 0)
            cash -= Time.deltaTime;
        Building building = CurrentBuilding;
        if (building == null || CurrentState != State.FollowRoute)
            return;
        switch(building.BuildingType)
        {
            case BuildingType.Apartment:
                if((building as ApartmentBuilding).GetApartment(transform.position) != null)
                    food += foodGainMultiplier * Time.deltaTime;
                if (food >= ffcMax && currentFFC == BuildingType.Apartment)
                    ChooseRoute();
                break;
            case BuildingType.Bank:
                cash += cashGainMultiplier * Time.deltaTime;
                if (cash >= ffcMax && currentFFC == BuildingType.Bank)
                    ChooseRoute();
                break;
            case BuildingType.Bar:
                fun += funGainMultiplier * Time.deltaTime;
                if (fun >= ffcMax && currentFFC == BuildingType.Bar;
                    ChooseRoute();
                break;
        }
        
    }

    private void ChooseRoute()
    {
        float val = Mathf.Min(food, cash, fun);
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        if (val == food)
        {
            currentFFC = BuildingType.Apartment;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Apartment, this);
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            Debug.Log("FOOD");
            
        }
        else if (val == cash)
        {
            currentFFC = BuildingType.Bank;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Bank, this);
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            Debug.Log("CASH");
        }
        else if(val == fun)
        {
            currentFFC = BuildingType.Bar;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Bar, this);
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            Debug.Log("FUN");
        }
    }

    public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
    {
        if (IsIncapacitated)
            return false;
        if (CurrentState == State.Panic)
            return true;
        if (alertIntesity != AlertIntensity.ConfirmedHostile)
            return true;

        CurrentState = State.Panic;
        CurrentAction = ActionE.Flee;
        Path.Clear();

        if (!Utils.LineOfSight(transform.position, PlayerController.Instance.gameObject,
            ~LayerMask.GetMask("AI", "Ignore Raycast"))) 
            return true;
        if (PlayerController.Instance.IsHostile)
            CurrentAction = ActionE.Freeze;

        return true;
    }

    protected override void DieAnimation(Vector3 dir)
    {
        DeadBodyHolder = new GameObject("deadbodyholder");
        DeadBodyHolder.transform.position = transform.position;
        GameObject temp = new GameObject("DeadHead");
        temp.AddComponent<SpriteRenderer>().sprite = DeadHead;
        temp.transform.position = transform.position + Vector3.forward * 10f;
        temp.transform.parent = DeadBodyHolder.transform;
        temp = new GameObject("DeadBody");
        temp.transform.position = transform.position + Vector3.forward * 11f;
        temp.AddComponent<SpriteRenderer>().sprite = Sprites[0];
        temp.transform.parent = DeadBodyHolder.transform;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayerSeen()
    {
        var building = CurrentBuilding;
        if (!PlayerController.Instance.IsHostile && (building == null || !building.PlayerReportedAsHostile)) 
            return;
       
        CurrentState = State.Panic;
        CurrentAction = ActionE.Freeze;
        Path.Clear();
    }
}
