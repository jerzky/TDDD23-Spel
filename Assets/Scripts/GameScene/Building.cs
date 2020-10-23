using System;
using System.Collections.Generic;
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
public enum BuildingType { None, Bank, Apartment, Bar, BigBank }

public enum PoliceCarAlignment
{
    Vertical,
    Horizontal
}
public abstract class Building : MonoBehaviour
{
    [Serializable]
    public class Room
    {
        [SerializeField]
        public Vector2 Position;

        public bool IsCleared { get; set; } = false;
        public bool IsTaken { get; set; } = false;
    }




    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;

    [SerializeField] 
    public List<Entrance> Entrances;

    [SerializeField]
    public List<Room> Rooms;

    [SerializeField]
    public Vector2 PoliceCarSpawnPoint;

    [SerializeField]
    public Vector2 PoliceSpawnPoint;


    [SerializeField]
    public PoliceCarAlignment PoliceCarAlignment;



    protected List<BuildingPart> BuildingParts = new List<BuildingPart>();
    private readonly SimpleTimer _playerHostileTimer = new SimpleTimer(30);
    protected readonly SimpleTimer PoliceSpawnTimer = new SimpleTimer(10);


    public bool PlayerReportedAsHostile { get; private set; } = false;
    public BuildingType BuildingType { get; protected set; }
    // Start is called before the first frame update
    protected void GenerateEntrances()
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


    public void RemoveCoveringLawman(Lawman lawman)
    {
        foreach (var entrance in Entrances)
        {
            entrance.RemoveLawman(lawman);
        }
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

    public virtual void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntesity, AI reporter = null)
    {
        if (alertIntesity == AlertIntensity.ConfirmedHostile)
        {
            ReportPlayerAsHostile();
            PoliceController.Instance.CallPolice(pos, this);
        }
    }

    public bool IsWithin(Vector2 position)
    {
        return BuildingParts.Any(v => v.IsWithin(position));
    }

    public bool IsWithin(AI ai)
    {
        return IsWithin(ai.gameObject.transform.position);
    }


    public abstract NodePath GetCivilianPath(AI ai);

    public bool Contains(AI_Type type)
    {
        return FindObjectsOfType<AI>().Any(ai => ai.AiType == type);
    }
}