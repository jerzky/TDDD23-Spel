using UnityEngine;

public class GotoCoverEntrance : Action
{
    public enum ReturnType : uint { NotFinished, Finished, FollowPath };

    private readonly Police _police;
    private Vector2 _currentEntrance;
    public GotoCoverEntrance(Police ai) : base(ai)
    {
        _police = ai;

    }

    public override uint PerformAction()
    {
        if (ai.Path.Count > 0)
            return (uint)ReturnType.FollowPath;
        
        SetBestEntrance();
        SetCoverPosition();
        return (uint)ReturnType.NotFinished;
    }
    private void SetBestEntrance()
    {
        var building = _police.CurrentBuilding;
        var bestEntrance = building.FindBestEntrance();
      
        building.AddToEnterance(_police, bestEntrance);
        _currentEntrance = bestEntrance;
    }

    private static float count = 0;
    private void SetCoverPosition()
    {

        ai.Path.Clear();
        var pos = new Vector2(11 + count, 65);
        count += 0.5f;
        ai.SetPathToPosition(pos);
        Debug.Log($"Set the police cover to: {pos}");
    }


    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        //Here we need to check if we need to abort and start pursuing or storming the building
       // if (ai.Path.Count > 0)
            return ActionE.FollowPath;

       // return ActionE.FollowPath;
    }



}