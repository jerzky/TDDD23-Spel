
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
        var layerMask = ~LayerMask.GetMask("AI", "Ignore Raycast");
        return Utils.LineOfSight(_police.transform.position, PlayerController.Instance.gameObject, layerMask)
            ? (uint)ReturnType.FoundPlayer
            : (uint)ReturnType.NotFinished;
    }



    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue,
        AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint) ReturnType.FoundPlayer)
        {
            _police.SetCurrentState(State.Pursuit);
            return ActionE.Pursue;
        }
        return ActionE.HoldCoverEntrance;
    }
}

