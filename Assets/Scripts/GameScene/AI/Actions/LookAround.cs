using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LookAround : Action
{
    private readonly Police _police;
    private const float ClearRadius = 15f;
    public enum ReturnType : uint
    {
        NotFinished,
        PlayerFound,
        Finished
    };

    public LookAround(AI ai) : base(ai)
    {
        _police = ai as Police;
    }

    public override uint PerformAction()
    {

        var layerMask = ~LayerMask.GetMask("AI", "Ignore Raycast", "cctv");
        if (Utils.LineOfSight(ai.transform.position, PlayerController.Instance.gameObject, layerMask))
            return (uint)ReturnType.PlayerFound;
/*
        if (Physics2D.OverlapCircleAll(ai.transform.position, ClearRadius).Any(co =>
             co.gameObject.GetInstanceID() == PlayerController.Instance.gameObject.GetInstanceID()))
            return (uint)ReturnType.PlayerFound;

        */
        return (uint)ReturnType.Finished;

    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        switch(currentState)
        {
            case State.StormBuilding:
                if (lastActionReturnValue == (uint)ReturnType.Finished)
                {
                    _police.FindRoomToClear.CurrentRoom.IsCleared = true;
                    return ActionE.FindRoomToClear;
                }
                else if(lastActionReturnValue == (uint)ReturnType.PlayerFound)
                    _police.Alert(PlayerController.Instance.transform.position, AlertIntensity.ConfirmedHostile);

                return ActionE.Pursue;
                break;
            default:
                return ActionE.None;
                break;
        }
        
    }
}
