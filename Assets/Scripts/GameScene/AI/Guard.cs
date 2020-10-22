using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Lawman
{
    // Start is called before the first frame update
    protected override void Start()
    {
        Health = 100;
        HaltTime = 2f;
        ShootTime = 0.5f;
        sprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[60];
        sprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[61];
        sprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[62];
        sprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[63];
        base.Start();
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        DeadHat = Resources.Load<Sprite>("Textures/guardhat");
    }

    protected override void FixedUpdate()
    {
        if(CurrentState != State.FollowRoute || Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < 35f)
            base.FixedUpdate();
    }

    protected override void PlayerSeen()
    {
        var building = CurrentBuilding;
        if (building != null)
            if (PlayerController.Instance.IsHostile || building.PlayerReportedAsHostile)
            {
                building.OnAlert(PlayerController.Instance.transform.position, AlertType.Guard_Radio, AlertIntensity.ConfirmedHostile, this);
                base.PlayerSeen();
            }
                
        
    }

    public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
    {
        if (IsIncapacitated)
            return false;
        //Pursue.LastPlayerPos = position;
        LastSeenPlayerLocation = PlayerController.Instance.transform.position;
        if (CurrentState == State.Pursuit)
            return true;

        CurrentAlertIntensity = alertIntesity;
        switch (CurrentAlertIntensity)
        {
            case AlertIntensity.NonHostile:
                StartInvestigate(position);
                break;
            case AlertIntensity.Nonexistant:
                StartInvestigate(position);
                break;
            case AlertIntensity.ConfirmedHostile:
                CurrentState = State.Pursuit;
                CurrentAction = ActionE.Pursue;
                break;

        }

        return true;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }
}
