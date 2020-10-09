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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var parentPos = GetComponentInParent<Transform>().position;
        var targetPos = col.transform.position;
        var hit = Physics2D.Raycast(parentPos + (targetPos - parentPos) * 0.35f, targetPos - parentPos);
        if (hit.collider == null || hit.collider.GetInstanceID() != col.GetInstanceID())
            return;

        switch (visionType)
        {
            case VisionType.GUARD:
                GetComponentInParent<Transform>().GetComponentInParent<AI>().OnVisionEnter(col);
                break;
            case VisionType.CCTV:
                GetComponentInParent<Transform>().GetComponentInParent<CCTV>().OnVisionEnter(col);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch (visionType)
        {
            case VisionType.GUARD:
                GetComponentInParent<Transform>().GetComponentInParent<AI>().OnVisionStay(col);
                break;
            case VisionType.CCTV:
                GetComponentInParent<Transform>().GetComponentInParent<CCTV>().OnVisionStay(col);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {

        switch (visionType)
        {
            case VisionType.GUARD:
                GetComponentInParent<Transform>().GetComponentInParent<AI>().OnVisionExit(col);
                break;
            case VisionType.CCTV:
                GetComponentInParent<Transform>().GetComponentInParent<CCTV>().OnVisionExit(col);
                break;
        }
    }
}
