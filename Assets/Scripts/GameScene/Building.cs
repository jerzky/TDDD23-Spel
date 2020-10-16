﻿﻿using System.Collections.Generic;
using System.Dynamic;
 using System.Linq;
 using UnityEngine;


public class BuildingPart
{
    public Vector2 Position { get; private set; }
    public Vector2 Size { get; private set; }
    public BuildingPart(Vector2 pos, Vector2 size)
    {
        Position = pos;
        Size = size;
    }

    public bool IsWithin(Vector2 pos)
    {
        if (pos.x > Position.x + Size.x / 2 || pos.x < Position.x - Size.x / 2)
            return false;
        if (pos.y > Position.y + Size.y / 2 || pos.y < Position.y - Size.y / 2)
            return false;

        return true;
    }
}
public class Building : MonoBehaviour
{

    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;

    [SerializeField] 
    public List<Entrance> Entrances;

    [SerializeField]
    public Vector2 PoliceSpawnPoint;



    protected List<BuildingPart> _buildingParts = new List<BuildingPart>();
    private readonly SimpleTimer _playerHostileTimer = new SimpleTimer(30);
    protected readonly SimpleTimer PoliceSpawnTimer = new SimpleTimer(60);


    public bool PlayerReportedAsHostile { get; private set; } = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        foreach (var entrance in Entrances)
        {
            entrance.GenerateTiles();
        }
    }

    void Update()
    {
        if (PlayerReportedAsHostile && _playerHostileTimer.TickAndReset())
            ResetPlayerHostility();
    }

    public Vector2 AddCoveringLawman(Lawman lawman)
    {
        var lowest = int.MaxValue;
        Entrance best = null;
        foreach (var entrance in Entrances.Where(entrance => entrance.GetTileCount() < lowest))
        {
            lowest = entrance.GetTileCount();
            best = entrance;

            if (lowest == 0)
                break;
        }

        if (best == null)
            return (Vector2) lawman.transform.position;

        var pos = best.TryAddLawman(lawman, out var success);
        return success ? pos : (Vector2) lawman.transform.position;

    }


    protected virtual void ReportPlayerAsHostile()
    {
        PlayerReportedAsHostile = true;
        _playerHostileTimer.Reset();
    }
    protected virtual void ResetPlayerHostility()
    {
        PlayerReportedAsHostile = false;
    }





    public virtual void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntesity)
    {
        if (alertIntesity == AlertIntensity.ConfirmedHostile)
        {
            ReportPlayerAsHostile();
            PoliceController.Instance.CallPolice(pos);
        }
    }

    public bool IsWithin(Vector2 position)
    {
        foreach(var v in _buildingParts)
        {
            if (v.IsWithin(position))
                return true;
        }
        return false;
    }
}