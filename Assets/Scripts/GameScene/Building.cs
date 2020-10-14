using System;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Building : MonoBehaviour
{

    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;

    [SerializeField] 
    public List<Vector2> Entrances;

    [SerializeField]
    public Vector2 PoliceSpawnPoint;

    private readonly Dictionary<Vector2, List<Police>> _entranceCover = new Dictionary<Vector2, List<Police>>();


    void Start()
    {
        foreach (var entrance in Entrances)
        {
            _entranceCover.Add(entrance, new List<Police>());
        }
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
        
    }
}