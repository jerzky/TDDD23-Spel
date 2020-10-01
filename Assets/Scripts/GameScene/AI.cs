using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public enum ActionE { None, Idle, FollowPath, LookAround };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute };


public class AI : MonoBehaviour
{
    // Vision Variables
    [SerializeField]
    public GameObject rotateVisionAround;
    [SerializeField]
    public Transform leftOffsetPoint;
    [SerializeField]
    public Transform rightOffsetPoint;


    public List<Collider2D> inVision = new List<Collider2D>();
    SortedDictionary<string, float> justEnteredVision = new SortedDictionary<string, float>();

    public SortedDictionary<ActionE, Action> actions = new SortedDictionary<ActionE, Action>();
    public FollowPath followPath;

    // Move Variables
    public float walkingSpeed = 2f;
    public float speedMultiplier = 1f;


    // Health
    int health = 100;
    bool isIncapacitated = false;


    // StateMachineVariables
    State idleState = State.None;
    State currentState = State.None;
    ActionE currentActionE = ActionE.None;
    AlertType currentAlertType = AlertType.None;

    // Investigate variables
    

    // Follow Route variables
    public List<Node> path = new List<Node>();
    NodePath currentRoute;
    float waitTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        followPath = new FollowPath(this);
        actions.Add(ActionE.FollowPath, followPath);
        actions.Add(ActionE.LookAround, new LookAround(this));
        currentState = State.FollowRoute;
        currentActionE = ActionE.FollowPath;
    }

    private void Update()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Reset variables that need to be reset every frame for functionality.
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        if (actions[currentActionE].PerformAction())
            GetNextActionE();
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

    public void GetNextActionE()
    {
        switch (currentState)
        {
            case State.Investigate:
                Investigate();
                break;
            case State.FollowRoute:
                SetPathToPosition(currentRoute.Nodes[currentRoute.CurrentNodeIndex++].Position);
                currentActionE = ActionE.FollowPath;
                break;
        }
    }


    void SetPathToPosition(Vector2 pos)
    {
        // We should start following the path to the next route node here
        path.Clear();
        followPath.SetPathToPosition(currentRoute.GetNextNode("FollowRoute").Position);
        Debug.Log($"Set path to next desired node ({currentRoute.CurrentNode.Position.x}, {currentRoute.CurrentNode.Position.y})");
    }

    public void SetRoute(NodePath route)
    {
        currentRoute = route;
        currentState = State.FollowRoute;
        currentActionE = ActionE.FollowPath;
        SetPathToPosition(currentRoute.Nodes[currentRoute.CurrentNodeIndex++].Position);
    }

    void CancelCurrentState()
    {
        CancelCurrentActionE();
        currentState = idleState;
    }

    void CancelCurrentActionE()
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
        switch(currentActionE)
        {
            case ActionE.FollowPath:
                currentActionE = ActionE.LookAround;
                actions[currentActionE].SetUp();
                break;
            case ActionE.LookAround:
                CancelCurrentState();
                break;
        }
    }

    void StartInvestigate(Vector2 position, AlertType alertType)
    {
        CancelCurrentState();
        currentState = State.Investigate;
        SetPathToPosition(position);
        currentActionE = ActionE.FollowPath;
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
        if(CompareTag(col.tag) || col.CompareTag("Player"))
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
        if (CompareTag(col.tag) || col.CompareTag("Player"))
        {
            justEnteredVision.Remove(col.gameObject.name);
            inVision.Remove(col);
        }
    }

    public void Injure(int damage, Vector3 dir)
    {
        health -= damage;
        if (health <= 0)
        {
            GameObject temp = new GameObject("DeadHead");
            temp.transform.position = transform.position + Vector3.forward * 10f;
            temp.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/deadhead");
            temp = new GameObject("DeadBody");
            temp.transform.position = transform.position + Vector3.forward * 11f;
            temp.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            temp = new GameObject("GuardHat");
            float r = UnityEngine.Random.Range(0.4f, 0.8f);
            Vector3 hatDir = new Vector3(r * dir.normalized.x, dir.normalized.y * r, 9f);
            float rot = -90f;
            rot *= hatDir.x;
            temp.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rot));
            temp.transform.position = transform.position + hatDir;
            temp.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/guardhat");
            Destroy(gameObject);
        }
    }
}
