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
public class Building : MonoBehaviour
{

    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;

    protected List<BuildingPart> _buildingParts = new List<BuildingPart>();
    SimpleTimer playerHostileTimer = new SimpleTimer(30);
    public bool PlayerReportedAsHostile { get; private set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerReportedAsHostile && playerHostileTimer.TickAndReset())
            ResetPlayerHostility();

    }

    protected virtual void ReportPlayerAsHostile()
    {
        PlayerReportedAsHostile = true;
        playerHostileTimer.Reset();
    }
    protected virtual void ResetPlayerHostility()
    {
        PlayerReportedAsHostile = false;
    }

    public virtual void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntesity)
    {
        if (alertIntesity == AlertIntensity.ConfirmedHostile)
            ReportPlayerAsHostile();
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