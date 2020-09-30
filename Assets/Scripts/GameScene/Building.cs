﻿using UnityEngine;
public enum AlertType
{
    Investigate,

}
public class Building : MonoBehaviour
{

    public bool IsSomeoneMonitoringCCTV => _securityStation.IsMonitored;

    [SerializeField]
    private SecurityStation _securityStation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnAlert(AlertType alertType, Vector2 pos)
    {

    }
}