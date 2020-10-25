
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
        //   if (!_police.CoverBuilding.IsWithin(PlayerController.Instance.transform.position))
        //       return (uint) ReturnType.FoundPlayer;

        var dontStorm = _police.CoverBuilding.Contains(AI_Type.Civilian) &&
                    _police.CoverBuilding.Contains(AI_Type.Construction_Worker) &&
                    _police.CoverBuilding.Contains(AI_Type.Medical_Worker);

        if (dontStorm)
        {
            Debug.Log($"DONT STORM: {dontStorm}");
            return (uint)ReturnType.NotFinished;
        }
            


        return (uint) ReturnType.FoundPlayer;
    }



    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue,
        AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint) ReturnType.FoundPlayer)
        {
            Debug.Log($"FOUND PLAYER. STORMING");
            _police.CurrentState = State.StormBuilding;
            return ActionE.FindRoomToClear;
        }
        return ActionE.HoldCoverEntrance;
    }
}

