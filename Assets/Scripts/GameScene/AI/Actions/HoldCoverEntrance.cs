
using UnityEngine;

public class HoldCoverEntrance : Action
{
    public enum ReturnType : uint
    {
        NotFinished,
        FoundPlayer
    };

    private readonly Police _police;

    public HoldCoverEntrance(Police ai) : base(ai)
    {
        _police = ai;
    }

    public override uint PerformAction()
    {
        if (_police.CoverBuilding.IsWithin(PlayerController.Instance.transform.position))
            return (uint) ReturnType.FoundPlayer;

        if (_police.CoverBuilding.Contains(AI_Type.Civilian))
            return (uint)ReturnType.NotFinished;


        return (uint)ReturnType.FoundPlayer;
    }



    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue,
        AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint) ReturnType.FoundPlayer)
        {
            _police.SetCurrentState(State.StormBuilding);
            return ActionE.FindRoomToClear;
        }
        return ActionE.HoldCoverEntrance;
    }
}

