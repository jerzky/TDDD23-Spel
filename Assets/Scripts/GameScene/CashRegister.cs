using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashRegister : Interactable
{
    enum RobType{ GunPoint, Stealthy };
    float[] averageRobTime = { 15f, 10f };
    bool timerActive = false;
    float timeLeft = 0f;
    int robType = -1;
    int averageCashAmount = 500;
    float downTime = 300f;
    float downTimeTimer = 0f;
    bool isDown = false;
    float creditMultiplier = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDown)
        {
            downTimeTimer += Time.deltaTime;
            if(downTimeTimer > downTime)
            {
                downTimeTimer = 0f;
                isDown = false;
            }    
        }
        else if (timerActive)
            Timer();
    }

    public void Timer()
    {
        if (timeLeft < 0)
        {
            Cancel();
            GeneralUI.Instance.Credits += (int)(averageCashAmount * creditMultiplier);
            isDown = true;
        }
        else
            timeLeft -= Time.deltaTime;
    }

    public override bool Interact(uint itemIndex)
    {
        if (isDown)
            return false;
        if (itemIndex != 0 && ItemList.AllItems[itemIndex].ItemType == ItemType.Weapon)
        {
            timerActive = true;
            LoadingCircle.Instance.StartLoading();
            timeLeft = averageRobTime[(int)RobType.GunPoint];
            // Random chance that cashier pulls gun?
        }
        else
        {
            timeLeft = averageRobTime[(int)RobType.Stealthy];
            timerActive = true;
            LoadingCircle.Instance.StartLoading();
        }
        return true;
    }

    public override void Cancel()
    {
        timerActive = false;
        timeLeft = 0;
        robType = -1;
        LoadingCircle.Instance.StopLoading();
    }
}
