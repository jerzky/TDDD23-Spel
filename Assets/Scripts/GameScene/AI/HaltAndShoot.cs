using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HaltAndShoot : Action
{
    private readonly Lawman _ai;

    public enum ReturnType : uint { NotFinished, Finished };
    private readonly AIWeaponHandler _weaponHandler;
    public HaltAndShoot(Lawman ai, AIWeaponHandler weaponHandler ) : base(ai)
    {
        _ai = ai;
        _weaponHandler = weaponHandler;
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
