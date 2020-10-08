using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HaltAndShoot : Action
{
    public enum ReturnType : uint { NotFinished, Finished };
    private readonly AIWeaponHandler _weaponHandler;
    public HaltAndShoot(AI ai) : base(ai)
    {
        _weaponHandler = new AIWeaponHandler(ai.GetComponent<AudioSource>());
    }
    public override uint PerformAction()
    {
        
        //Shoot or do we still have time left?
        if (_weaponHandler.Shoot(ai.transform.position, PlayerController.Instance.transform.position))
            return (uint)ReturnType.Finished;

        return (uint)ReturnType.NotFinished;
    }

    public void ResetShootTimer()
    {
        _weaponHandler.ResetShootTimer();
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        return ActionE.Pursue;
    }
}
