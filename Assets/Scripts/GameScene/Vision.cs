using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public enum VisionType { CCTV, GUARD, WORKER, BANKCASHIER, STORECASHIER };
    public enum LocationType { BANK, STORE };
    PolygonCollider2D pc;
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
        if (col.CompareTag("Player"))
        {
            // Player is in Vision
            // TODO: CHECK IF PLAYER IS HOSTILE


            Debug.Log("Player in Vision");
        }
    }
}
