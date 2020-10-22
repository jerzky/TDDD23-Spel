using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Police : Lawman
{
    public Building CoverBuilding { get; private set; }
    public new AlertIntensity CurrentAlertIntensity => AlertIntensity.ConfirmedHostile;
    public FindRoomToClear FindRoomToClear { get; set; }
    public PoliceCar Car { get; set; }

    public Vector2 MySpawnPoint { get; set; }
    protected override void Start()
    {
        Health = 150;
        HaltTime = 2f;
        ShootTime = 2f;
        AiType = AI_Type.Police;
        Actions.Add(ActionE.GotoCoverEntrance, new GotoCoverEntrance(this));
        Actions.Add(ActionE.HoldCoverEntrance, new HoldCoverEntrance(this));
        FindRoomToClear = new FindRoomToClear(this);

        Actions.Add(ActionE.FindRoomToClear, FindRoomToClear);
        Actions.Add(ActionE.ClearRoom, new ClearRoom(this));
        Actions.Add(ActionE.WaitingForAllPolice, new WaitingForAllPolice(this) );

        sprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[4];
        sprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[5];
        sprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[6];
        sprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[7];
        base.Start();
        IdleState = State.GotoCoverEntrance;
        IdleAction = ActionE.GotoCoverEntrance;
        CurrentState = State.GotoCoverEntrance;
        CurrentAction = ActionE.GotoCoverEntrance;
        DeadHat = Resources.Load<Sprite>("Textures/guardhat");
    }




    protected override void PlayerSeen()
    {

        if (CurrentAction == ActionE.GotoCoverEntrance || CurrentAction == ActionE.HoldCoverEntrance)
            return;

        PoliceController.Instance.AlertAll(PlayerController.Instance.transform.position, this);
        base.PlayerSeen();
    }



    public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
    {
        Vector2 temp = Pursue.LastPlayerPos;
        //Pursue.LastPlayerPos = position;
        LastSeenPlayerLocation = PlayerController.Instance.transform.position;
        if (CurrentState == State.Pursuit)
            return true;
        if(temp != Pursue.LastPlayerPos)
             PoliceController.Instance.AlertAll(position);

        CurrentState = State.Pursuit;
        CurrentAction = ActionE.Pursue;

        return true;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        PoliceController.AllPolice.Remove(this);
        CoverBuilding.RemoveCoveringLawman(this);
        if (Car != null)
        {
            Car.MyPolice.Remove(this);

        }
    }

    public void SetCoverBuilding(Building building) => CoverBuilding = building;

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }

    private static GameObject _standardPrefab;

    public static GameObject Generate(Vector2 position, State state, ActionE action, Building building)
    {

        if (_standardPrefab == null)
            _standardPrefab = Resources.Load<GameObject>("Prefabs/POLICE");

        var police = Instantiate(_standardPrefab, position, Quaternion.identity,
            WeaponController.Instance.bulletHolder.transform);

        police.GetComponent<Police>().CurrentState = state;
        police.GetComponent<Police>().CurrentAction = action;
        police.GetComponent<Police>().SetCoverBuilding(building);

        PoliceController.AllPolice.Add(police.GetComponent<Police>());

        return police;
    }
}
