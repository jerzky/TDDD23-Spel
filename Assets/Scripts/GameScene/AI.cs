using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Transactions;
using UnityEngine;
using UnityEngineInternal;

public class AI : MonoBehaviour
{
    [SerializeField]
    GameObject rotateVisionAround;
    [SerializeField]
    public Transform leftOffsetPoint;
    [SerializeField]
    public Transform rightOffsetPoint;

    public List<Node> path = new List<Node>();
    
    public bool isWaitingForPath = false;

    Vector2[] dirToLeft = { Vector2.down, Vector2.up, Vector2.left, Vector2.right, Vector2.down + Vector2.left, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.up + Vector2.right };
    Vector2[] dirToRight = { Vector2.up, Vector2.down, Vector2.right, Vector2.left, Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right };
    float[] angle = { 270f, 90f, 180f, 0f, 225f, 135f, 315f, 45f };

    float walkingSpeed = 5f;


    Vector2 offset = Vector2.zero;
    float offsetForceMultiplier = 0f;
    float forceMultiplierSpeed = 2f;
    
    List<Collider2D> inVision = new List<Collider2D>();
    SortedDictionary<string, float> justEnteredVision = new SortedDictionary<string, float>();

    bool movingThroughDoor = false;
    bool walkingPassedDoor = false;
    float timer = 0f;
    Vector3 lastMovePos = Vector3.zero;

    Vector2 lastDoorPos = new Vector2(-1, -1);
    Vector2 lastDoorNeighbourPos = new Vector2(-1, -1);

    bool closeTheDoor = false;
    int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if (path.Count <= 0 && !isWaitingForPath)
        {
            float x = UnityEngine.Random.Range(46, 123);
            float y = UnityEngine.Random.Range(34, 66);
            
            if (PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), new Vector2(x, y), this))
            {
                isWaitingForPath = true;
            }
        }
        if (path.Count <= 0)
        {
            return;
        }
        float speed = walkingSpeed;
        isWaitingForPath = false;

        if (Vector2.Distance(lastMovePos, transform.position) > 1f)
        {
            lastMovePos = transform.position;
            timer = 0f;
        }
        else
        {
            timer += Time.fixedDeltaTime;
            if ((timer > 1f && !movingThroughDoor) || (timer > 2f))
            {
                timer = 0f;
                Node current = path[0];
                while(current.Child != null)
                {
                    current = current.Child;
                }
                NodeToPathList(PathingController.Instance.FindPathExcluding(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), current.Position, new List<Vector2>{ path[0].Position }));
                if(lastDoorNeighbourPos != new Vector2(-1, -1))
                {
                    PathingController.Instance.DoorPassed(lastDoorNeighbourPos, this);
                }
                if (path.Count == 0)
                {
                    isWaitingForPath = false;
                    return;
                }
            }
        }

        if (!walkingPassedDoor && PathingController.Instance.IsDoorNeighbour(path[0].Position))
        {
            float distance = PathingController.Instance.RequestDoorAccess(path[0].Position, this);
            if (distance > 0)
            {
                // We are heading for a door, and do not have door access yet.
                lastDoorNeighbourPos = path[0].Position;
                if(Vector2.Distance(transform.position, path[0].Position) <= distance)
                    return;
            }
            else
            {
                walkingPassedDoor = true;
                lastDoorNeighbourPos = path[0].Position;
            }
        }
        else if(walkingPassedDoor)
        {
            if(PathingController.Instance.GetNodeType(path[0].Position) == NodeType.Door)
            {
                // Next node is door
                Door door = PathingController.Instance.GetDoor(path[0].Position);
                movingThroughDoor = true;
                if (door.IsClosed())
                {
                    // open door if closed
                    door.Open();
                    return;
                }
                else if (!door.IsOpen())
                {
                    // wait for door to fully open
                    return;
                }
            }
        }

        Vector2 dir = (path[0].Position - (Vector2)transform.position).normalized;
        Collider2D closest = null;
        if(true)
        {
            if (inVision.Count > 0)
            {
                speed *= 0.5f;

                foreach (var v in inVision)
                {
                    Vector2 vDir = Vector2.zero;
                    if (v.GetComponent<AI>().path.Count > 0 && v.GetComponent<AI>().path[0].Parent != null)
                        vDir = v.GetComponent<AI>().path[0].Position - v.GetComponent<AI>().path[0].Parent.Position;
                    Vector2 myDir = Vector2.one;
                    if (path.Count > 0 && path[0].Parent != null)
                        myDir = path[0].Position - path[0].Parent.Position;

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, v.transform.position - transform.position);
                    if (hit.collider.gameObject.name.Substring(0, 2) != "AI")
                        continue;

                    if ((vDir != myDir || Vector2.Distance(transform.position, v.transform.position) < 1f) && (closest == null || Vector2.Distance(v.transform.position, transform.position) < Vector2.Distance(closest.transform.position, transform.position)))
                    {
                        closest = v;
                    }
                }
            }
        }
        SetOffset(closest);
        dir += offset.normalized * offsetForceMultiplier;

        float angle = Mathf.Atan2(dir.x, dir.y) * 180 / Mathf.PI;
        rotateVisionAround.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

        GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(dir.x, dir.y, 0f) * speed * Time.fixedDeltaTime);
        if (Vector2.Distance(transform.position, path[0].Position) < 0.1f)
        {
            movingThroughDoor = false;
            if (walkingPassedDoor && !PathingController.Instance.IsDoorNeighbour(path[0].Child.Position))
            {
                PathingController.Instance.DoorPassed(lastDoorNeighbourPos, this);
                walkingPassedDoor = false;
                lastDoorNeighbourPos = new Vector2(-1, -1);
            }
            path.RemoveAt(0);
        }
    }

    void SetOffset(Collider2D closest)
    {
        if(closest == null)
        {
            offsetForceMultiplier -= Time.fixedDeltaTime * forceMultiplierSpeed;
            if (offsetForceMultiplier <= 0)
            {
                offsetForceMultiplier = 0f;
                offset = Vector2.zero;
            }
        }
        else
        {
            Transform closestOffsetPoint = closest.GetComponent<AI>().leftOffsetPoint;
            if (Vector2.Distance(closest.GetComponent<AI>().rightOffsetPoint.position, transform.position) < Vector2.Distance(closestOffsetPoint.position, transform.position))
                closestOffsetPoint = closest.GetComponent<AI>().rightOffsetPoint;

            offset = closestOffsetPoint.position - closest.transform.position;
            offsetForceMultiplier += Time.fixedDeltaTime * forceMultiplierSpeed;
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public void TellToStay(Collision2D col)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    public void SetNodePath(NodePath path)
    {

    }
    public void OnVisionEnter(Collider2D col)
    {
        if(CompareTag(col.tag))
        {
            justEnteredVision.Add(col.gameObject.name, 0f);
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if(justEnteredVision.ContainsKey(col.gameObject.name))
        {
            justEnteredVision[col.gameObject.name] += Time.deltaTime;
            if(justEnteredVision[col.gameObject.name] > 0.25f)
            {
                inVision.Add(col);
                justEnteredVision.Remove(col.gameObject.name);
            }
        }
    }

    public void OnVisionExit(Collider2D col)
    {
        if (CompareTag(col.tag))
        {
            justEnteredVision.Remove(col.gameObject.name);
            inVision.Remove(col);
        }
    }

    public void Injure(int damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }

    public void NodeToPathList(Node node)
    {
        if (node == null)
            return;
        path.Clear();
        path.Add(node);
        if (node.Child == null)
            return;
        Vector2 dir = node.Position - (Vector2)transform.position;
        Vector2 prevdir = dir;
        node = node.Child;
        while (node != null)
        {
            dir = node.Position - node.Parent.Position;
            // Add Node to list if the direction changes
            if (dir != prevdir)
            {
                path.Add(node.Parent);
            }
            else if(PathingController.Instance.IsDoorNeighbour(node.Parent.Position))
            {
                // We should check for doornodes and add the closest nodes to it aswell as the door node.
                path.Add(node.Parent);
            }
            node = node.Child;
            prevdir = dir;
        }
    }
}
