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
    State idleState = State.FollowRoute;
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
                SetPathToPosition(currentRoute.NextNode.Position);
                currentActionE = ActionE.FollowPath;
                break;
        }
    }


    void SetPathToPosition(Vector2 pos)
    {
        path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);
    }

    public void SetRoute(NodePath route)
    {
        currentRoute = route;
        currentState = State.FollowRoute;
        currentActionE = ActionE.FollowPath;
        SetPathToPosition(currentRoute.NextNode.Position);
    }

    void CancelCurrentState()
    {
        CancelCurrentActionE();
        currentState = idleState;
        GetNextActionE();
    }

    void CancelCurrentActionE()
    {
        currentActionE = ActionE.None;
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

    public void Injure(int damage)
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
            float y = UnityEngine.Random.Range(0.7f, 1.3f);
            float x = UnityEngine.Random.Range(-1f, 1f);
            float r = -90f;
            r *= x;
            temp.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, r));
            temp.transform.position = transform.position + new Vector3(x, y, 9f);
            temp.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/guardhat");
            Destroy(gameObject);
        }
    }
}
