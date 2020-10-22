using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public abstract class Action
{
    protected AI ai;

    protected Action(AI ai)
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
        if (ai == null)
            return (uint)ReturnType.NotFinished;
        if (ai.CurrentRoute == null)
            return (uint)ReturnType.NoRoute;
        if(Vector2.Distance(ai.transform.position, ai.CurrentRoute.CurrentNode.Position) < 0.5f)
        {
            var act = ai.CurrentRoute.CurrentNode.CallOnAchieved;
            act?.Invoke();
        }
        if (!hasWaited && ai.CurrentRoute.CurrentNode.Type == NodePath.RouteNodeType.Idle && Vector2.Distance(ai.transform.position, ai.CurrentRoute.CurrentNode.Position) < 0.5f)
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
        if (ai == null)
            return (uint)ReturnType.NotFinished;

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

public class RemoveBody : Action
{
    public enum ReturnType { NotFinished, Finished, NextBody }
    float range = 0.5f;
    public List<GameObject> Bodies = new List<GameObject>();
    public RemoveBody(AI ai) : base(ai)
    {
        
    }
   
    public override uint PerformAction()
    {
        
        if (Vector2.Distance(ai.transform.position, Bodies[0].transform.position) > 2f)
            return NextBody();
        GameObject.Destroy(Bodies[0]);
        Bodies.RemoveAt(0);
        if (Bodies.Count > 0)
            return NextBody();
        return (uint)ReturnType.Finished;
    }
    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        switch(lastActionReturnValue)
        {
            case (uint)ReturnType.NextBody:
                return ActionE.FollowPath;
            default:
                return ActionE.None;
        }
    }

    uint NextBody()
    {
        Bodies.Sort((a, b) => Vector2.Distance(ai.transform.position, a.transform.position).CompareTo(Vector2.Distance(ai.transform.position, b.transform.position)));
        ai.SetPathToPosition(Bodies[0].transform.position);
        return (uint)ReturnType.NextBody;
    }

}

public class RebuildObject : Action
{
    public enum ReturnType { NotFinished, Finished, NextObject }
    float range = 1f;
    public List<GameObject> Objects = new List<GameObject>();
    public RebuildObject(AI ai) : base(ai)
    {

    }

    public override uint PerformAction()
    {

        if (Vector2.Distance(ai.transform.position, Objects[0].transform.position) > range)
            return NextObjects();
        BreakableController.Instance.Rebuild(Objects[0].transform.position);
        Objects.RemoveAt(0);
        if (Objects.Count > 0)
            return NextObjects();
        return (uint)ReturnType.Finished;
    }
    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        switch (lastActionReturnValue)
        {
            case (uint)ReturnType.NextObject:
                return ActionE.FollowPath;
            default:
                return ActionE.None;
        }
    }

    uint NextObjects()
    {
        Objects.Sort((a, b) => Vector2.Distance(ai.transform.position, a.transform.position).CompareTo(Vector2.Distance(ai.transform.position, b.transform.position)));
        ai.SetPathToPosition(Objects[0].transform.position);
        return (uint)ReturnType.NextObject;
    }

}

