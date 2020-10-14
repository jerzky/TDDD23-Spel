using UnityEngine;


public class CoverEntrance : Action
{
    public enum ReturnType : uint { NotFinished, Finished };

    private readonly Police _police;
    private Vector2 _currentEntrance;
    public CoverEntrance(Police ai) : base(ai)
    {
        _police = ai;
        SetBestEntrance();
    }

    public override uint PerformAction()
    {




        return (uint)ReturnType.NotFinished;
    }

    private Vector2 FindCoverPosition()
    {

    }


    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {

        return ActionE.CoverEntrance;
    }


    private void SetBestEntrance()
    {
        var building = _police.CurrentBuilding;
        var bestEntrance = building.FindBestEntrance();
        building.AddToEnterance(_police, bestEntrance);
        _currentEntrance = bestEntrance;
    }
}