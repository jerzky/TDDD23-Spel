using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Action
{
    public enum ReturnType { NotFinished, Finished }
    SimpleTimer freezeTimer;
    SimpleTimer lookForLawman;
    public Freeze(AI ai, float freezeTime) : base(ai)
    {
        freezeTimer = new SimpleTimer(freezeTime);
        lookForLawman = new SimpleTimer(freezeTime/3);
    }
    
    public override uint PerformAction()
    {
        // Do nothing until we either see a lawman or have waited sufficiently
        if (lookForLawman.TickAndReset())
            if (CanSeeLawMan())
                return (uint)ReturnType.Finished;

        if (!freezeTimer.TickAndReset())
            return (uint)ReturnType.NotFinished;

        return (uint)ReturnType.Finished;
    }

    bool CanSeeLawMan()
    {
        foreach(var v in ai.InVision)
        {
            if (v.CompareTag("humanoid") && (int)v.GetComponent<AI>().AiType < (uint)AI_Type.Civilian)
                return true;
        }
        return false;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (alertIntensity != AlertIntensity.ConfirmedHostile)
            return ActionE.None;

        switch(currentState)
        {
            case State.Panic:
                if(Utils.LineOfSight(ai.transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast")))
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
                 //   Debug.Log("FREEZE IS OVER, WE DO NOT HAVE LINE OF SIGHT");
                    var building = ai.CurrentBuilding;
                    if (building != null && building.PlayerReportedAsHostile)
                    {
                      //  Debug.Log("PLAYER REPORTED AS HOSTILE TRUE");
                        return ActionE.Flee;
                    }
                    else
                    {
                     //   Debug.Log("PLAYER NOT REPORTED AS HOSTILE");

                        return ActionE.None;
                    }
                }
            default:
                throw new System.ArgumentException();
        }
    }
}
