using System;
using System.Collections.Generic;
using System.Dynamic;
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
public enum BuildingType { Bank, Appartment, Bar }

public abstract class Building : MonoBehaviour
{

    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;

    [SerializeField] 
    public List<Vector2> Entrances;

    [SerializeField]
    public Vector2 PoliceSpawnPoint;

    private readonly Dictionary<Vector2, List<Police>> _entranceCover = new Dictionary<Vector2, List<Police>>();


    protected List<BuildingPart> _buildingParts = new List<BuildingPart>();
    private readonly SimpleTimer _playerHostileTimer = new SimpleTimer(30);
    protected readonly SimpleTimer PoliceSpawnTimer = new SimpleTimer(60);


    public bool PlayerReportedAsHostile { get; private set; } = false;
    public BuildingType BuildingType { get; protected set; }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        foreach (var entrance in Entrances)
        {
            _entranceCover.Add(entrance, new List<Police>());
        }
    }

    void Update()
    {
        if (PlayerReportedAsHostile && _playerHostileTimer.TickAndReset())
            ResetPlayerHostility();
    }

    public Vector2 FindBestEntrance()
    {
        var lowest = int.MaxValue;
        var best = Vector2.zero;
        foreach (var pair in _entranceCover)
        {
            if (pair.Value.Count >= lowest)
                continue;

            lowest = pair.Value.Count;
            best = pair.Key;

            if (lowest == 0)
                break;
        }
        return best;


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

    public bool IsEntranceCovered(Vector2 entrance)
    {
        if (!_entranceCover.ContainsKey(entrance))
            return false;

        return _entranceCover[entrance].Count > 0;
    }

    public void AddToEnterance(Police police, Vector2 entrance)
    {
        if (!_entranceCover.ContainsKey(entrance))
            return;
        _entranceCover[entrance].Add(police);
    }

    public void RemoveFromEnterance(Police police, Vector2 entrance)
    {
        if (!_entranceCover.ContainsKey(entrance))
            return;

        _entranceCover[entrance].Remove(police);
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

    public abstract NodePath GetCivilianPath();
}