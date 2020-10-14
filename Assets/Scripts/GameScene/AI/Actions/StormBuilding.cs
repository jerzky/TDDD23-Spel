using UnityEngine;

public class StormBuilding : Action
{
    public enum ReturnType : uint {NotFinished, Finished };

    private readonly Police _police;

    public StormBuilding(Police ai) : base(ai)
    {
        _police = ai;
   
    }

    public override uint PerformAction()
    {



        return (uint)ReturnType.NotFinished;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        return ActionE.StormBuilding;
    }


}

