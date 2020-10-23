using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Bank : Building
{
    public static Bank Instance;
    protected readonly List<NodePath> _nodePaths = new List<NodePath>();
    protected readonly List<AI> _guards = new List<AI>();

    private readonly List<NodePath> _civilianNodePaths = new List<NodePath>();
    private int _civilianNodePathIndex = 0;
    [SerializeField] 
    private GameObject _nodeHolder;
    [SerializeField]
    private GameObject _civilianNodeHolder;

    // Start is called before the first frame update
    protected void Start()
    {
        Instance = this;
        BuildingType = BuildingType.Bank;
        LoadPathingNodes();
        var allGuards = GetComponentsInChildren<AI>();

        if (allGuards.Length > 1)
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

        var sizeX = 28 - 5;
        var sizeY = 99 - 77;
        var posX = 5 + sizeX / 2;
        var posY = 77 + sizeY / 2;
        BuildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));
        PoliceController.AddBuilding(this);
        SetUpCivilianRoutes();
        GenerateEntrances();
    }

    // Update is called once per frame
    void Update()
    {
        PoliceSpawnTimer.Tick();
    }

    protected void SetUpCivilianRoutes()
    {
        var rooms = _civilianNodeHolder.GetComponentsInChildren<Transform>();
        foreach (var r in rooms)
        {
            if (r.childCount == 0 || r.name == _civilianNodeHolder.name)
                continue;

            _civilianNodePaths.Add(NodePath.LoadPathNodesFromHolder(r.gameObject));
        }
    }


    protected void LoadPathingNodes()
    {

        var rooms = _nodeHolder.GetComponentsInChildren<Transform>();
        foreach (var r in rooms)
        {
            if (r.childCount == 0 || r.gameObject.GetInstanceID() == _nodeHolder.gameObject.GetInstanceID())
                continue;
            
            _nodePaths.Add(NodePath.LoadPathNodesFromHolder(r.gameObject));
        }
    }

    public override void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntensity, AI reporter = null)
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
                    SendGuardToInvestigate(pos, alertIntensity, reporter);
                break;
            case AlertType.Sound:
                SendGuardToInvestigate(pos, alertIntensity, reporter);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(alertType), alertType, null);
        }
    }

    private void SendGuardToInvestigate(Vector2 pos, AlertIntensity alertIntensity, AI reporter = null)
    {
        foreach (var guard in _guards.Where(guard => guard != null && guard != reporter))
        {
            //Alerts guards d
            guard.Alert(pos, alertIntensity);
        }
    }

    public override NodePath GetCivilianPath(AI ai)
    {
        if (_civilianNodePathIndex >= _civilianNodePaths.Count) _civilianNodePathIndex = _civilianNodePathIndex % _civilianNodePaths.Count;
        return _civilianNodePaths[_civilianNodePathIndex++];
    }
}
