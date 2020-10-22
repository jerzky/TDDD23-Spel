using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FindRoomToClear : Action
{
    public enum ReturnType : uint
    {
        NotFinished,
        Finished,
        NoMoreRooms
    };

    private readonly Police _police;
    public Building.Room CurrentRoom { get; set; }
    public FindRoomToClear(Police ai) : base(ai)
    {
        _police = ai;

    }

    public override uint PerformAction()
    {

        var allNotCleared = _police.CoverBuilding.Rooms.Where(r => !r.IsCleared);
        var notCleared = allNotCleared as Building.Room[] ?? allNotCleared.ToArray();
        if (notCleared.Length == 0)
            return (uint) ReturnType.NoMoreRooms;


        var room = notCleared.FirstOrDefault(r => !r.IsTaken);

        if (room == default(Building.Room))
            return (uint) ReturnType.NoMoreRooms;
            // room = notCleared[Random.Range(0, notCleared.Length - 1)];

        _police.SetPathToPosition(room.Position);
        room.IsTaken = true;
        CurrentRoom = room;


        return (uint) ReturnType.Finished;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue != (uint) ReturnType.NoMoreRooms) 
            return ActionE.FollowPath;

        _police.SetPathToPosition(_police.MySpawnPoint);
        _police.CurrentState = State.PoliceGoToCar;
        return ActionE.FollowPath;
    }


}

public class ClearRoom : Action
{
    private readonly Police _police;
    private const float ClearRadius = 2f;
    public enum ReturnType : uint
    {
        NotFinished,
        PlayerFound,
        RoomCleared
    };

    public ClearRoom(Police ai) : base(ai)
    {
        _police = ai;
    }

    public override uint PerformAction()
    {

        var layerMask = ~LayerMask.GetMask("AI", "Ignore Raycast");
        if (Utils.LineOfSight(ai.transform.position, PlayerController.Instance.gameObject, layerMask))
            return (uint) ReturnType.PlayerFound;

        if(Physics2D.OverlapCircleAll(ai.transform.position, ClearRadius).Any(co =>
            co.gameObject.GetInstanceID() == PlayerController.Instance.gameObject.GetInstanceID()))
            return (uint)ReturnType.PlayerFound;

        _police.FindRoomToClear.CurrentRoom.IsCleared = true;
        return (uint)ReturnType.RoomCleared;

    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue != (uint) ReturnType.PlayerFound) 
            return ActionE.FindRoomToClear;

        _police.Alert(PlayerController.Instance.transform.position, AlertIntensity.ConfirmedHostile);
        return ActionE.Pursue;
    }
}