using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : Action
{

    float rotationSpeed = 90f;
    float maxLookAroundTime = 4f;
    float minLookAroundTime = 8f;
    float currentLookAroundTimer = 0f;
    bool firstRun = true;

    public LookAround(AI ai) : base(ai)
    {

    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        // if we ever see anyone while walking around, the playerSeen function in ai would be called. Therefore we can always return none.
        return ActionE.None;
    }

    public override uint PerformAction()
    {
        if(firstRun)
        {
            firstRun = false;
            currentLookAroundTimer = UnityEngine.Random.Range(minLookAroundTime, maxLookAroundTime);
        }
        currentLookAroundTimer -= Time.fixedDeltaTime;
        if (currentLookAroundTimer <= 0f)
        {
            firstRun = true;
            return 1;
        }

        ai.rotateVisionAround.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.fixedDeltaTime));

        return 0;
    }
}
