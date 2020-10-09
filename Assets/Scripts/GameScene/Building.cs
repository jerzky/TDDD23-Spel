using System.Dynamic;
using UnityEngine;

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

    public virtual void OnAlert(Vector2 pos, AlertType alertType, AlertIntensity alertIntesity)
    {
        
    }
}