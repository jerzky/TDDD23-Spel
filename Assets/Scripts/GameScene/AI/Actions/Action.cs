using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Remoting.Messaging;
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
    public enum ReturnType { NotFinished, NewPathCreated, Idle, NoRoute }
    bool hasWaited = false;
    public FindPathToRouteNode(AI ai) : base(ai)
    {

    }
    public override uint PerformAction()
    {
        if (ai.CurrentRoute == null)
            return (uint)ReturnType.NoRoute;

        if (!hasWaited && ai.CurrentRoute.CurrentNode.Type == NodePath.RouteNodeType.Idle)
        {
            hasWaited = true;
            return (uint)ReturnType.Idle;
        }
        hasWaited = false;
        ai.SetPathToPosition(ai.CurrentRoute.NextNode.Position);
        return (uint)ReturnType.NewPathCreated;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint)ReturnType.NoRoute)
            return ActionE.None;
        if (lastActionReturnValue == (uint)ReturnType.Idle)
            return ActionE.Idle;
        return ActionE.FollowPath;
    }
}

public class IdleAtRouteNode : Action
{
    public enum ReturnType { NotFinished, Finished }
    SimpleTimer timer = new SimpleTimer(9756873f);
    public IdleAtRouteNode(AI ai) : base(ai)
    {
        timer.Reset();
    }
    public override uint PerformAction()
    {
        if (timer.CurrentTime == 9756873f)
            timer.ResetTo(ai.CurrentRoute.CurrentNode.IdleTime);

        if (timer.TickFixedAndReset())
            return (uint)ReturnType.Finished;

        return (uint)ReturnType.NotFinished;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        return ActionE.FollowPath;
    }
}

