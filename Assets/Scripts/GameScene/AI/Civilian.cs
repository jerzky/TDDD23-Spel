using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Civilian : AI
{
    private float _fun;
    private float _food;
    private float _cash;
    private const float FoodGainMultiplier = 4f;
    private const float CashGainMultiplier = 4f;
    private const float FunGainMultiplier = 4f;
    private const float FfcMax = 300f;
    private int _currentFfc = -1;


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

        _fun = Random.Range(0, FfcMax);
        _food = Random.Range(0, FfcMax);
        _cash = Random.Range(0, FfcMax);
        Debug.Log("Food: " + _food + " cash: " + _cash);
        ChooseRoute();
    }

    protected virtual void Update()
    {
        if(CurrentAction == ActionE.None)
        {
            _currentFfc = -1;
            ChooseRoute();
            return;
        }
        if(_food > 0)
            _food -= Time.deltaTime;
        if (_fun > 0)
            _fun -= Time.deltaTime;
        if (_cash > 0)
            _cash -= Time.deltaTime;
        Building building = CurrentBuilding;
        if (building == null || CurrentState != State.FollowRoute)
            return;
        switch(building.BuildingType)
        {
            case BuildingType.Appartment:
                if((building as ApartmentBuilding).GetApartment(transform.position) != null)
                    _food += FoodGainMultiplier * Time.deltaTime;
                if (_food >= FfcMax && _currentFfc == 0)
                    ChooseRoute();
                break;
            case BuildingType.Bank:
                _cash += CashGainMultiplier * Time.deltaTime;
                if (_cash >= FfcMax && _currentFfc == 1)
                    ChooseRoute();
                break;
            case BuildingType.Bar:
                _fun += FunGainMultiplier * Time.deltaTime;
                if (_fun >= FfcMax && _currentFfc == 2)
                    ChooseRoute();
                break;
        }
        
    }

    private void ChooseRoute()
    {
        float val = Mathf.Min(_food, _cash);
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        if (val == _food)
        {
            _currentFfc = 0;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Appartment, this);
            Path.Clear();
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            Debug.Log("FOOD");
            
        }
        else if (val == _cash)
        {
            _currentFfc = 1;
            CurrentRoute = BuildingController.Instance.GetCivilianNodePath(BuildingType.Bank, this);
            Path.Clear();
            SetPathToPosition(CurrentRoute.CurrentNode.Position);
            Debug.Log("CASH");
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
