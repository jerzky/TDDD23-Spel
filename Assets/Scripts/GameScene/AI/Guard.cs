using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Lawman
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentState = State.FollowRoute;
        currentAction = ActionE.FollowPath;
        deadHat = Resources.Load<Sprite>("Textures/guardhat");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
    {
        if (isIncapacitated)
            return false;
        if (currentState == State.Pursuit)
            return true;

        currentAlertIntensity = alertIntesity;
        switch (currentAlertIntensity)
        {
            case AlertIntensity.NonHostile:
                StartInvestigate(position);
                break;
            case AlertIntensity.Nonexistant:
                StartInvestigate(position);
                break;
            case AlertIntensity.ConfirmedHostile:
                pursue.LastPlayerPos = position;
                currentState = State.Pursuit;
                currentAction = ActionE.Pursue;
                break;
        }

        return true;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }
}
