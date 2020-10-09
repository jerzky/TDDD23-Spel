using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public enum VisionType { CCTV, GUARD, WORKER, BANKCASHIER, STORECASHIER };

    HashSet<Collider2D> inVision = new HashSet<Collider2D>();

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

    private void OnVision(Collider2D col)
    {
        if(inVision.Contains(col))
            OnVisionStay(col);
        else
            OnVisionEnter(col);
    }

    private void OnVisionEnter(Collider2D col)
    {
        if (!inVision.Contains(col))
        {
            inVision.Add(col);
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
    }

    private void OnVisionStay(Collider2D col)
    {
        if (!inVision.Contains(col))
            return;
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

    private void OnVisionExit(Collider2D col)
    {
        if (!inVision.Contains(col))
            return;
        inVision.Remove(col);
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

    bool LineOfSight(Collider2D col)
    {
        if (Utils.LineOfSight(GetComponentInParent<Transform>().position, col.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast", "cctv")))
            return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(LineOfSight(col))
            OnVisionEnter(col);
        else
            OnVisionExit(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (LineOfSight(col))
        {
            // We can see col, OnVision will handle seen object
            OnVision(col);
        }
        else
        {
            // We cant see col
            OnVisionExit(col);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        OnVisionExit(col);
    }
}
