using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    float timeOfDayTimer = 0f;
    float dayEnd = 60 * 60 * 24; // one day in seconds
    float timeMultiplier = 60 * 60; // one second is one hour
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeOfDayTimer += Time.deltaTime * timeMultiplier;
        if(timeOfDayTimer >= dayEnd)
        {
            timeOfDayTimer = 0f;
            // NEW DAY
        }
    }
}
