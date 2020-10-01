﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : Action
{

    float rotationSpeed = 90f;
    float maxLookAroundTime = 4f;
    float minLookAroundTime = 8f;
    float currentLookAroundTimer = 0f;

    public LookAround(AI ai) : base(ai)
    {

    }

    public override bool PerformAction()
    {
        currentLookAroundTimer -= Time.fixedDeltaTime;
        if (currentLookAroundTimer <= 0f)
            return true;

        ai.rotateVisionAround.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.fixedDeltaTime));
        return false;
    }

    public override void SetUp()
    {
        currentLookAroundTimer = UnityEngine.Random.Range(minLookAroundTime, maxLookAroundTime);
    }
}