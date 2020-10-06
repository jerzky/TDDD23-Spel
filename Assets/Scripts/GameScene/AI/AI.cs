using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public enum ActionE { None, Idle, FollowPath, LookAround, Pursue };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute, Pursuit };
public enum AlertType { None, Guard_CCTV, Guard_Radio, Sound };
public enum AlertIntesity { Nonexistant, NonHostile, ConfirmedHostile }

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
    public float moveSpeed { 
        get
        {
            return currentState == State.Pursuit ? pursueSpeed : patrolSpeed;
        }
    }
    public const float pursueSpeed = 5f;
    public const float patrolSpeed = 3f;
    public float speedMultiplier = 1f;


    // Health
    int health = 100;
    bool isIncapacitated = false;
    GameObject cuffs;


    // StateMachineVariables
    State idleState = State.FollowRoute;
    State currentState = State.None;
    ActionE currentAction = ActionE.None;
    AlertType currentAlertIntesity = AlertType.None;
    AlertType currentAlertType = AlertType.None;
    

    // Follow Route variables
    public List<Node> path = new List<Node>();
    NodePath currentRoute;

    // Start is called before the first frame update
    void Start()
    {
        followPath = new FollowPath(this);
        actions.Add(ActionE.FollowPath, followPath);
        actions.Add(ActionE.LookAround, new LookAround(this));
        actions.Add(ActionE.Pursue, new Pursue(this, FindObjectOfType<PlayerController>().transform, followPath));
        currentState = State.FollowRoute;
        currentAction = ActionE.FollowPath;
        cuffs = Instantiate(Resources.Load<GameObject>("Prefabs/cuffs"), transform.position + new Vector3(0f, -0.3f, -1f), Quaternion.identity, transform);
        cuffs.SetActive(false);
    }

    private void Update()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Reset variables that need to be reset every frame for functionality.
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        if (isIncapacitated)
            return;

        if (actions[currentAction].PerformAction())
            GetNextActionE();

    }

    public bool Alert(Sound sound)
    {
        AlertIntesity alertIntesity = AlertIntesity.Nonexistant;

        Alert(sound.origin, AlertType.Sound, alertIntesity);
        return true;
    }


    public bool Alert(Vector2 position, AlertType alertType, AlertIntesity alertIntesity)
    {
        // TODO: SOMETHING NEEDS TO STOP AI FROM RESTARTING INVESTIGATION, BUT SOMEHOW CHANGE PATH SMOOTHLY, 
        // RIGHT NOW IT STOPS EVERY TIME IT HEARS A BULLET AND RECALCULATES PATH
        switch(alertType)
        {
            case AlertType.Guard_CCTV:
            case AlertType.Guard_Radio:
                StartInvestigate(position, alertType);
                break;
            case AlertType.Sound:
                StartInvestigate(position, alertType);
                // we need to check the current location, if we are in a common space, only confirmed hostile and maybe construction would trigger reaction
                // if we care in a staff only space, investigate it no matter what.
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
                currentAction = ActionE.FollowPath;
                break;
            case State.Pursuit:
                Pursuit();
                break;
        }
    }

    void Pursuit()
    {
        switch(currentAction)
        {
            case ActionE.Pursue:
                // Completely Lost Player
                // TODO: Search Action Here maybe?
                break;
        }
    }


    public void SetPathToPosition(Vector2 pos)
    {
        path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);
    }

    public void SetRoute(NodePath route)
    {
        currentRoute = route;
        currentState = State.FollowRoute;
        currentAction = ActionE.FollowPath;
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
        currentAction = ActionE.None;
    }

    void Investigate()
    {
        switch(currentAction)
        {
            case ActionE.FollowPath:
                currentAction = ActionE.LookAround;
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
        currentAction = ActionE.FollowPath;
    }

    public void OnVisionEnter(Collider2D col)
    {
        if(CompareTag(col.tag))
        {
            justEnteredVision.Add(col.gameObject.name, 0f);
        }
        if (col.CompareTag("Player"))
        {
            PlayerSeen();
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if(justEnteredVision.ContainsKey(col.gameObject.name))
        {
            justEnteredVision[col.gameObject.name] += Time.deltaTime;
            if(justEnteredVision[col.gameObject.name] > 0.15f)
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

    public void Injure(int damage, Vector3 dir)
    {
        CreateBloodSplatter();
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
            GeneralUI.Instance.Kills++;
            Destroy(gameObject);
        }
    }

    public void Incapacitate()
    {
        // raycast behind me
        Vector3 dir = transform.position - GetComponentInChildren<Vision>().transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir * 0.3f, dir, 1f);
        if (hit.collider == null ||!hit.collider.CompareTag("Player"))
        {
            // shoot that motherfucker
            return;
        }
        isIncapacitated = true;
        cuffs.SetActive(true);
    }

    public void ReleaseCuffs()
    {
        isIncapacitated = false;
        cuffs.SetActive(false);
    }


    void PlayerSeen() // guard implementation
    {
        switch (currentState)
        {
            case State.Investigate:
                currentState = State.Pursuit;
                currentAction = ActionE.Pursue;
                break;
        }
    }

    void CreateBloodSplatter()
    {
        GameObject blood = new GameObject("Blood");
        blood.transform.parent = transform;
        blood.transform.position = transform.position + Vector3.back;
        blood.AddComponent<SpriteRenderer>();
        blood.AddComponent<Blood>();
    }
}
