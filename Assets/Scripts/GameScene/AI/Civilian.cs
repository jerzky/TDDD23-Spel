using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Civilian : AI
{
    float max = 100f;
    float fun;
    float food;
    float cash;
    float foodGainMultiplier = 4f;
    float cashGainMultiplier = 4f;
    float funGainMultiplier = 4f;
    float ffcMax = 300f;

    SimpleTimer switchCurrentRouteTimer = new SimpleTimer(30f);
    int currentFFC = -1;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Actions.Add(ActionE.Flee, new Flee(this));
        Actions.Add(ActionE.Freeze, new Freeze(this, 10f));
        Health = 75;
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        AiType = AI_Type.Civilian;
        sprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[8];
        sprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[9];
        sprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[10];
        sprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[11];

        fun = Random.Range(0, ffcMax);
        food = 0;//Random.Range(0, ffcMax);
        cash = Random.Range(0, ffcMax);
        ChooseRoute();
    }

    void Update()
    {
        if(CurrentAction == ActionE.None)
        {
            currentFFC = -1;
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
            case BuildingType.Appartment:
                if((building as ApartmentBuilding).GetApartment(transform.position) != null)
                    food += foodGainMultiplier * Time.deltaTime;
                if (food >= ffcMax && currentFFC == 0)
                    ChooseRoute();
                break;
            case BuildingType.Bank:
                cash += cashGainMultiplier * Time.deltaTime;
                if (cash >= ffcMax && currentFFC == 1)
                    ChooseRoute();
                break;
            case BuildingType.Bar:
                fun += funGainMultiplier * Time.deltaTime;
                if (fun >= ffcMax && currentFFC == 2)
                    ChooseRoute();
                break;
        }
        
    }

    private void ChooseRoute()
    {
        float val = Mathf.Min(food, cash);
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        if(currentFFC == 0 && val != food)
        {
            Building building = CurrentBuilding;
            if (building != null)
                (building as ApartmentBuilding).GetApartment(transform.position).Resident = null;
        }
        if (val == food)
        {
            currentFFC = 0;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Appartment, this);
            Path.Clear();
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            
        }
        else if (val == cash)
        {
            currentFFC = 1;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Bank, this);
            Path.Clear();
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
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

        if (Utils.LineOfSight(transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast")))
            if (PlayerController.Instance.IsHostile)
                CurrentAction = ActionE.Freeze;

        return true;
    }

    protected override void DieAnimation(Vector3 dir)
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayerSeen()
    {
        var building = CurrentBuilding;
        if (PlayerController.Instance.IsHostile || (building != null && building.PlayerReportedAsHostile))
        {
            CurrentState = State.Panic;
            CurrentAction = ActionE.Freeze;
            Path.Clear();
        }
    }

    
}
