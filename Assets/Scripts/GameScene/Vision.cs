using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public enum VisionType { CCTV, GUARD, WORKER, BANKCASHIER, STORECASHIER };
    
    PolygonCollider2D pc;
    [SerializeField]
    VisionType visionType;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PolygonCollider2D>();
        visionType = VisionType.CCTV;
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
                gameObject.GetComponentInParent<AI>().OnVisionEnter(col);
                break;
            case VisionType.CCTV:
                gameObject.GetComponentInParent<CCTV>().OnVisionEnter(col);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                gameObject.GetComponentInParent<AI>().OnVisionStay(col);
                break;
            case VisionType.CCTV:
                gameObject.GetComponentInParent<CCTV>().OnVisionStay(col);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                gameObject.GetComponentInParent<AI>().OnVisionExit(col);
                break;
            case VisionType.CCTV:
                gameObject.GetComponentInParent<CCTV>().OnVisionExit(col);
                break;
        }
    }
}
