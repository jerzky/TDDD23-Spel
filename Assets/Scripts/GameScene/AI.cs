using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public enum Action { None, Idle, FollowPath, LookAround };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute };


public class AI : MonoBehaviour
{
    // Vision Variables
    [SerializeField]
    GameObject rotateVisionAround;
    List<Collider2D> inVision = new List<Collider2D>();
    SortedDictionary<string, float> justEnteredVision = new SortedDictionary<string, float>();

    // Collision Avoidance variables
    [SerializeField]
    public Transform leftOffsetPoint;
    [SerializeField]
    public Transform rightOffsetPoint;
    Vector2[] dirToLeft = { Vector2.down, Vector2.up, Vector2.left, Vector2.right, Vector2.down + Vector2.left, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.up + Vector2.right };
    Vector2[] dirToRight = { Vector2.up, Vector2.down, Vector2.right, Vector2.left, Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right };
    float[] angle = { 270f, 90f, 180f, 0f, 225f, 135f, 315f, 45f };
    Vector2 offset = Vector2.zero;
    float offsetForceMultiplier = 0f;
    float forceMultiplierSpeed = 2f;

    // Move Variables
    float walkingSpeed = 2f;
    float speedMultiplier = 1f;

    // Follow Path Variables
    public List<Node> path = new List<Node>();
    public bool isWaitingForPath = false;
    bool movingThroughDoor = false;
    bool walkingPassedDoor = false;
    float hasNotMovedTimer = 0f;
    Vector3 lastMovePos = Vector3.zero;
    Vector2 lastDoorPos = new Vector2(-1, -1);
    Vector2 lastDoorNeighbourPos = new Vector2(-1, -1);

    // Health
    int health = 100;
    bool isIncapacitated = false;


    // StateMachineVariables
    State idleState = State.None;
    State currentState = State.None;
    Action currentAction = Action.None;
    AlertType currentAlertType = AlertType.None;

    // Investigate variables
    float rotationSpeed = 90f;
    float maxLookAroundTime = 4f;
    float minLookAroundTime = 8f;
    float currentLookAroundTimer = 0f;

    // Follow Route variables
    NodePath currentRoute;
    float waitTimer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Reset variables that need to be reset every frame for functionality.
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        switch (currentAction)
        {
            case Action.None:
                break;
            case Action.Idle:
                break;
            case Action.FollowPath:
                FollowPath();
                break;
            case Action.LookAround:
                LookAround();
                break;
        }
    }


    public bool Alert(Vector2 position, AlertType alertType)
    {
        if (isIncapacitated)
            return false;

        switch(alertType)
        {
            case AlertType.Investigate:
                StartInvestigate(position, alertType);
                break;
        }

        return false;
    }

    public void GetNextAction(string from)
    {
        switch (currentState)
        {
            case State.Investigate:
                Investigate();
                break;
            case State.FollowRoute:
                Debug.Log("We finished the path to the desired node");
                FollowRoute();
                break;
        }
    }

    void FollowRoute()
    {
        // We should start following the path to the next route node here

        SetPathToPosition(currentRoute.GetNextNode("FollowRoute").Position);
        Debug.Log($"Set path to next desired node ({currentRoute.CurrentNode.Position.x}, {currentRoute.CurrentNode.Position.y})");
    }

    void StartFollowRoute()
    {
        currentState = State.FollowRoute;
        currentAction = Action.FollowPath;
        FollowRoute();
    }

    void CancelCurrentState()
    {
        CancelCurrentAction();
        currentState = idleState;
    }

    void CancelCurrentAction()
    {
        currentAction = Action.None;
    }

    public void SetRoute(NodePath route)
    {
     //   currentRoute = route;
     currentRoute = new NodePath("test", this, 
         new NodePath.RouteNode(new Vector2(98, 98), NodePath.RouteNodeType.Walk ),
         new NodePath.RouteNode(new Vector2(98, 95), NodePath.RouteNodeType.Walk),
         new NodePath.RouteNode(new Vector2(95, 95), NodePath.RouteNodeType.Walk),
         new NodePath.RouteNode(new Vector2(95, 98), NodePath.RouteNodeType.Walk));
        StartFollowRoute();
    }

    void Investigate()
    {
        switch(currentAction)
        {
            case Action.FollowPath:
                currentAction = Action.LookAround;
                currentLookAroundTimer = UnityEngine.Random.Range(minLookAroundTime, maxLookAroundTime);
                Debug.Log(currentLookAroundTimer);
                break;
            case Action.LookAround:
                CancelCurrentState();
                break;
        }
    }

    bool Wait()
    {
        Debug.Log("WAITING");
        waitTimer -= Time.fixedDeltaTime;
        return waitTimer <= 0f;
    }

    void StartInvestigate(Vector2 position, AlertType alertType)
    {
        CancelCurrentState();
        currentState = State.Investigate;
        SetPathToPosition(position);
        currentAction = Action.FollowPath;
    }

    void LookAround()
    {
        currentLookAroundTimer -= Time.fixedDeltaTime;
        if (currentLookAroundTimer <= 0f)
            GetNextAction("Lookaround");
        rotateVisionAround.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.fixedDeltaTime));
    }

    void SetPathToPosition(Vector2 pos)
    {
        path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);

    }

    void FollowPath()
    {
        if (path.Count <= 0)
            return;

        if (IsStuck())
            if (RecalculatePath())
                return;

        if (ShouldWaitForDoor())
            return;

        Vector2 dir = (path[0].Position - (Vector2)transform.position).normalized;
        SetOffset(FindClosestAIInVision());

        Debug.Log($"Moving towards node: ({path[0].Position}");
        Move(dir);
        if (Vector2.Distance(transform.position, path[0].Position) < 0.1f)
            FinishedNode();

        if (path.Count <= 0)
            GetNextAction("FollowPath");
    }

    bool RecalculatePath()
    {
        Debug.Log("Recalculating");

        hasNotMovedTimer = 0f;
        Node current = path[0];
        while (current.Child != null)
        {
            current = current.Child;
        }
        NodeToPathList(PathingController.Instance.FindPathExcluding(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), current.Position, new List<Vector2> { path[0].Position }));
        if (lastDoorNeighbourPos != new Vector2(-1, -1))
        {
            PathingController.Instance.DoorPassed(lastDoorNeighbourPos, this);
        }
        if (path.Count == 0)
        {
            isWaitingForPath = false;
            return true;
        }
        return false;
    }

    bool IsStuck()
    {
        

        if (Vector2.Distance(lastMovePos, transform.position) > 1f)
        {
            lastMovePos = transform.position;
            hasNotMovedTimer = 0f;
        }
        else
        {
            hasNotMovedTimer += Time.fixedDeltaTime;
            if ((hasNotMovedTimer > 10f && !movingThroughDoor) || (hasNotMovedTimer > 50f))
            {
                hasNotMovedTimer = 0f;
                return true;
            }
        }
        return false;
    }

    bool ShouldWaitForDoor()
    {
       // Debug.Log("ShouldWaitForDoor");

        if (!walkingPassedDoor && PathingController.Instance.IsDoorNeighbour(path[0].Position))
        {
            float distance = PathingController.Instance.RequestDoorAccess(path[0].Position, this);
            if (distance > 0)
            {
                // We are heading for a door, and do not have door access yet.
                lastDoorNeighbourPos = path[0].Position;
                if (Vector2.Distance(transform.position, path[0].Position) <= distance)
                    return true;
            }
            else
            {
                walkingPassedDoor = true;
                lastDoorNeighbourPos = path[0].Position;
            }
        }
        else if (walkingPassedDoor)
        {
            if (PathingController.Instance.GetNodeType(path[0].Position) == NodeType.Door)
            {
                // Next node is door
                Door door = PathingController.Instance.GetDoor(path[0].Position);
                movingThroughDoor = true;
                if (door.IsClosed())
                {
                    // open door if closed
                    door.Open();
                    return true;
                }
                else if (!door.IsOpen())
                {
                    // wait for door to fully open
                    return true;
                }
            }
        }
        return false;
    }

    Collider2D FindClosestAIInVision()
    {
        Collider2D closest = null;
        if (true)
        {
            if (inVision.Count > 0)
            {
                speedMultiplier = 0.5f;

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
        return closest;
    }

    void Move(Vector2 dir)
    {
        dir += offset.normalized * offsetForceMultiplier;

        float angle = Mathf.Atan2(dir.x, dir.y) * 180 / Mathf.PI;
        rotateVisionAround.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

        GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(dir.x, dir.y, 0f) * walkingSpeed * speedMultiplier * Time.fixedDeltaTime);
        speedMultiplier = 1f;
    }

    void FinishedNode()
    {
        movingThroughDoor = false;
        if (walkingPassedDoor && !PathingController.Instance.IsDoorNeighbour(path[0].Child.Position))
        {
            PathingController.Instance.DoorPassed(lastDoorNeighbourPos, this);
            walkingPassedDoor = false;
            lastDoorNeighbourPos = new Vector2(-1, -1);
        }
        path.RemoveAt(0);
        Debug.Log("Finished Node");

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
