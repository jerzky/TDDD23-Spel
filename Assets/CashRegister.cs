using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashRegister : Interactable
{
    enum RobType{ GunPoint, Stealthy };
    float[] averageRobTime = { 10f, 5f };
    bool timerStarted = false;
    float currentTime = 0f;
    int robType = -1;
    int averageCashAmount = 500;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted)
        {
            // Handle UI event which shows a timer?
            if (false)
            {
                // check if player walked away before finishing?
            }
            currentTime += Time.deltaTime;
            if(currentTime > averageRobTime[robType]) // add a multiplier to change time?
            {
                currentTime = 0;
                // Add cash to player here?
            }
        }
    }

    public override bool Interact(uint itemIndex)
    {
        if (IsWeapon(itemIndex))
        {
            robType = (int)RobType.GunPoint;
            timerStarted = true;
            // Random chance that cashier pulls gun?
        }
        else
        {
            // check if cashier is in vision?
            robType = (int)RobType.Stealthy;
            timerStarted = true;
        }
        return true;
    }

    bool IsWeapon(uint xx)
    {
        //TODO: fix a real way to determine if a item index is a weapon.
        return xx == 5;
    }
}
