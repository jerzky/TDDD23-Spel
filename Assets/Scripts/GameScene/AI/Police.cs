﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Police : Lawman
{
    public Building CurrentBuilding { get; private set; }


    protected override void Start()
    {
        Health = 150;
        HaltTime = 2f;
        ShootTime = 0.25f;
        AiType = AI_Type.Police;
        Actions.Add(ActionE.CoverEntrance, new CoverEntrance(this));
        sprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[4];
        sprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[5];
        sprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[6];
        sprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[7];
        base.Start();
        IdleState = State.None;
        IdleAction = ActionE.None;
        CurrentState = State.CoverEntrance;
        CurrentAction = ActionE.CoverEntrance;
        DeadHat = Resources.Load<Sprite>("Textures/guardhat");
    }








    public override bool Alert(Vector2 position, AlertIntensity alertIntesity)
    {
        return false;
    }


    public void SetCurrentState(State state) => CurrentState = state;

    public void SetCurrentAction(ActionE action) => CurrentAction = action;

    public void SetCurrentBuilding(Building building) => CurrentBuilding = building;

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }

    private static GameObject _standardPrefab;

    public static GameObject Generate(Vector2 position, State state, ActionE action, Building building)
    {
       Debug.Log($"Spawning police at pos: {position}");
        
        if (_standardPrefab == null)
            _standardPrefab = Resources.Load<GameObject>("Prefabs/POLICE");

        var police = Instantiate(_standardPrefab, position, Quaternion.identity,
            WeaponController.Instance.bulletHolder.transform);

        police.GetComponent<Police>().SetCurrentState(state);
        police.GetComponent<Police>().SetCurrentAction(action);
        police.GetComponent<Police>().SetCurrentBuilding(building);
        return police;
    }
}