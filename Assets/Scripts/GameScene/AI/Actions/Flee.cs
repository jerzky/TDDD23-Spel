using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Flee : Action
{
    public enum ReturnType { NotFinished, Finished, FollowPath }
    public Flee(AI ai) : base(ai)
    {

    }
    
    public override uint PerformAction()
    {
        if (Utils.LineOfSight(ai.transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast")))
            return (uint)ReturnType.Finished;

        if (ai.Path.Count > 0)
            return (uint)ReturnType.FollowPath;
        else
        {
            Vector2 to = new Vector2(Random.Range(0, 99), Random.Range(0, 99));
            ai.Path.Clear();
            ai.SetPathToPosition(to);
            return (uint)ReturnType.FollowPath;
        }
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (alertIntensity != AlertIntensity.ConfirmedHostile)
            return ActionE.None;

        switch (currentState)
        {
            case State.Panic:
                if (Utils.LineOfSight(ai.transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast")))
                {
                    // Can see player, return freeze if hostile, or none if safe
                    if (PlayerController.Instance.IsHostile)
                        return ActionE.Freeze;
                    else
                        return ActionE.None;
                }
                else
                {
                    // cant see player, return flee if hostile, or none if safe
                    if (lastActionReturnValue == (uint)ReturnType.FollowPath)
                        return ActionE.FollowPath;
                    if (ai.CurrentBuilding.PlayerReportedAsHostile)
                        return ActionE.Flee;
                    else
                        return ActionE.None;
                }
            default:
                throw new System.ArgumentException();
        }
    }
}
