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

        SetBestEntrance();
        SetCoverPosition();
     //   Debug.Log($"HoldPos {_currentEntrance} = {PathingController.Instance.IsClear(_currentEntrance)}");
        
        return (uint) ReturnType.FollowPath;
    }

    private void SetBestEntrance()
    {
        var building = _police.CoverBuilding;
        _currentEntrance = building.AddCoveringLawman(_police);

    }

    private void SetCoverPosition()
    {

        ai.Path.Clear();
        ai.SetPathToPosition(_currentEntrance);
    }


    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        //Here we need to check if we need to abort and start pursuing or storming the building
       // if (ai.Path.Count > 0)
            return ActionE.FollowPath;

       // return ActionE.FollowPath;
    }



}