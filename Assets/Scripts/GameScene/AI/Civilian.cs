using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Civilian : AI
{

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
        SetRoute(new NodePath("testroute", this, new NodePath.RouteNode(new Vector2(12, 90), NodePath.RouteNodeType.Walk), new NodePath.RouteNode(new Vector2(12, 82), NodePath.RouteNodeType.Walk)));
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
                CurrentAction = ActionE.Freeze; // if we can see player and he is carrying weapon freeze instead of flee

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
