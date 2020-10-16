﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Bank : Building
{
    public static Bank Instance;
    private readonly List<NodePath> _nodePaths = new List<NodePath>();
    private readonly List<AI> _guards = new List<AI>();


    [SerializeField] 
    private GameObject _nodeHolder;

    // Start is called before the first frame update
    protected override void Start()
    {
        Instance = this;
        LoadPathingNodes();
        var allGuards = GetComponentsInChildren<AI>();

        if (allGuards.Length > 0)
        {
            foreach (var guard in allGuards)
            {
                _guards.Add(guard);
            }

            _nodePaths[0].Guard = _guards[0];
            _nodePaths[1].Guard = _guards[1];

            _guards[0].SetRoute(_nodePaths[0]);
            _guards[1].SetRoute(_nodePaths[1]);
        }

        var sizeX = 23 - 0;
        var sizeY = 99 - 77;
        var posX = 0 + sizeX / 2;
        var posY = 77 + sizeY / 2;
        _buildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));


        base.Start();
    }


    // Update is called once per frame
    void Update()
    {
        PoliceSpawnTimer.Tick();
    }


    private void LoadPathingNodes()
    {

        var rooms = _nodeHolder.GetComponentsInChildren<Transform>();
        foreach (var r in rooms)
        {
            if (r.childCount == 0 || r.gameObject.GetInstanceID() == _nodeHolder.gameObject.GetInstanceID())
                continue;
            
            _nodePaths.Add(NodePath.LoadPathNodesFromHolder(r.gameObject));
        }
/*
        foreach (var path in _nodePaths)
        {
            Debug.Log($"Name: {path.Name}, Count: {path.Nodes.Count}");
        }*/
    }

    public override void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntensity)
    {
        base.OnAlert(pos, alertType, alertIntensity);
       
        switch (alertType)
        {
            case AlertType.Guard_CCTV:
                if(PoliceSpawnTimer.TickAndReset())
                    PoliceController.Instance.NotifyPolice(this);
               // SendGuardToInvestigate(pos, alertIntensity);
                break;
            case AlertType.None:
                break;
            case AlertType.Guard_Radio:
                // if it is confirmed hostile send more guards
                if(alertIntensity == AlertIntensity.ConfirmedHostile)
                    SendGuardToInvestigate(pos, alertIntensity);
                break;
            case AlertType.Sound:
                SendGuardToInvestigate(pos, alertIntensity);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(alertType), alertType, null);
        }
    }

    private void SendGuardToInvestigate(Vector2 pos, AlertIntensity alertIntensity)
    {
        foreach (var guard in _guards.Where(guard => guard != null))
        {
            //Alerts guards d
            guard.Alert(pos, alertIntensity);
        }
    }

}
