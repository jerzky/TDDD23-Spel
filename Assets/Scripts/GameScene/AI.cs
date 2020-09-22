using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class AI : MonoBehaviour
{
    List<Node> path = new List<Node>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count == 0)
        {
            path = PathingController.Instance.GetPath(transform.position, new Vector3(Random.value * 100f, Random.value * 100f, 0f));
        }

         Vector2 dir = path[0].Position - transform.position;
         transform.Translate(new Vector3(dir.x, dir.y, 0f).normalized * 2f * Time.deltaTime);
         if(Vector2.Distance(transform.position, path[0].Position) < 0.25f)
         {
             Debug.Log("Remove");
             path.RemoveAt(0);
         }
    }
}
