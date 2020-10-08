using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public abstract class Action
{
    protected AI ai;
    public Action(AI ai)
    {
        this.ai = ai;
    }

    public abstract uint PerformAction();
    public abstract ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity);
}

public class FindPathToRouteNode : Action
{
    public FindPathToRouteNode(AI ai) : base(ai)
    {

    }
    public override uint PerformAction()
    {
        ai.SetPathToPosition(ai.currentRoute.NextNode.Position);
        return 1;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        return ActionE.FollowPath;
    }
}

