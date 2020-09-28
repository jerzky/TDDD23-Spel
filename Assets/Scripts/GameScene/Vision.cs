using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public enum VisionType { CCTV, GUARD, WORKER, BANKCASHIER, STORECASHIER };
    public enum LocationType { BANK, STORE };
    PolygonCollider2D pc;
    [SerializeField]
    VisionType visionType;
    LocationType locationType;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PolygonCollider2D>();
        visionType = VisionType.CCTV;
        locationType = LocationType.BANK;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                gameObject.GetComponentInParent<AI>().EnteredVision(col);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                gameObject.GetComponentInParent<AI>().InVision(col);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                gameObject.GetComponentInParent<AI>().ExitVision(col);
                break;
        }
    }
}
