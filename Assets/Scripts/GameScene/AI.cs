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

    Vector2[] route = { new Vector2(17, 98), new Vector2(22, 98), new Vector2(17, 91), new Vector2(22, 91) };
    int index = 0;
    int indexChange = 1;

    // Start is called before the first frame update
    void Start()
    {
        followPath = new FollowPath(this);
        actions.Add(ActionE.FollowPath, followPath);
        actions.Add(ActionE.LookAround, new LookAround(this));
        currentState = State.FollowRoute;
        currentActionE = ActionE.FollowPath;
        SetPathToPosition(route[index++],"AIstart");
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
            GetNextActionE("FixedUpdate");
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

    public void GetNextActionE(string from)
    {
        switch (currentState)
        {
            case State.Investigate:
                Investigate();
                break;
            case State.FollowRoute:
                if (index > 3)
                    index = 0;
                SetPathToPosition(route[index++], "GetNextActionE");
                currentActionE = ActionE.FollowPath;
                break;
        }
    }


    void SetPathToPosition(Vector2 pos, string from)
    {
        Debug.Log("Setpathtopos called from" + from + " pos: " + pos);
        path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this, "SETPATHTOPOS");
    }

    void GoToNextNode(Vector2 pos)
    {
        path.Clear();
        path.Add(new Node(pos, null, 0f, Vector2.zero));
    }

    void CancelCurrentState()
    {
        CancelCurrentActionE();
        currentState = idleState;
    }

    void CancelCurrentActionE()
    {
        currentActionE = ActionE.None;
    }

    public void SetRoute(NodePath route)
    {

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
        SetPathToPosition(position, "startInvestigate");
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
}
