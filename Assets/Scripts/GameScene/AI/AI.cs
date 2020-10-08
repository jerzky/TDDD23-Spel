using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public enum AI_Type { Guard, Civilian, Bank_Worker, Construction_Worker }
public enum ActionE { None, Idle, FollowPath, LookAround, Pursue, HaltAndShoot, FindPathToRouteNode };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute, Pursuit };
public enum AlertType { None, Guard_CCTV, Guard_Radio, Sound };
public enum AlertIntensity { Nonexistant, NonHostile, ConfirmedHostile }

public abstract class AI : MonoBehaviour
{
    // Vision Variables
    [SerializeField]
    public GameObject rotateVisionAround;
    [SerializeField]
    public Transform leftOffsetPoint;
    [SerializeField]
    public Transform rightOffsetPoint;


    public List<Collider2D> inVision = new List<Collider2D>();
    protected SortedDictionary<string, float> justEnteredVision = new SortedDictionary<string, float>();

    public SortedDictionary<ActionE, Action> actions = new SortedDictionary<ActionE, Action>();
    public FollowPath followPath;

    // Move Variables
    public float moveSpeed => currentState == State.Pursuit ? pursueSpeed : patrolSpeed;

    public const float pursueSpeed = 3f;
    public const float patrolSpeed = 2f;
    public float speedMultiplier = 1f;


    // Health
    protected int health = 100;
    SimpleTimer incapacitateTimer = new SimpleTimer(20);
    protected bool isIncapacitated = false;
    protected bool isZipTied = false;
    protected GameObject cuffs;


    // StateMachineVariables
    protected State idleState = State.FollowRoute;
    protected ActionE idleAction = ActionE.FollowPath;
    protected State currentState = State.None;
    protected ActionE currentAction = ActionE.None;
    protected AlertIntensity currentAlertIntensity = AlertIntensity.Nonexistant;
    

    // Follow Route variables
    public List<Node> path = new List<Node>();
    public NodePath currentRoute { get; private set; }
    AI_Type ai_type = AI_Type.Guard;

    protected virtual void Start()
    {
        cuffs = Instantiate(Resources.Load<GameObject>("Prefabs/cuffs"), transform.position + new Vector3(0f, -0.3f, -1f), Quaternion.identity, transform);
        cuffs.SetActive(false);
        followPath = new FollowPath(this);
        actions.Add(ActionE.FollowPath, followPath);
        actions.Add(ActionE.FindPathToRouteNode, new FindPathToRouteNode(this));
    }

    void FixedUpdate()
    {
        if (!isZipTied && incapacitateTimer.TickFixed())
            isIncapacitated = false;
        // Reset variables that need to be reset every frame for functionality.
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        if (isIncapacitated)
            return;

        uint actionReturnType = actions[currentAction].PerformAction();
        if (actionReturnType != 0)
        {
            GetNextAction(actionReturnType);
        }

    }

    public AI_Type Type()
    {
        return ai_type;
    }

    public bool Alert(Sound sound)
    {
        switch (sound.soundType)
        {
            case Sound.SoundType.Weapon:
                currentAlertIntensity = AlertIntensity.ConfirmedHostile;
                break;
            case Sound.SoundType.Construction:
                currentAlertIntensity = AlertIntensity.NonHostile;
                break;
            default:
                currentAlertIntensity = AlertIntensity.Nonexistant;
                break;
        }
        return Alert(sound.origin, currentAlertIntensity);
    }

    public virtual bool Alert(Vector2 position, AlertIntensity alertIntesity) { Debug.LogError("IMPLEMENT ALERT IN ALL CLASSES DERIVED FROM AI"); return true;  }

    public virtual void GetNextAction(uint lastActionReturnValue)
    {
        currentAction = actions[currentAction].GetNextAction(currentState, lastActionReturnValue, currentAlertIntensity);
        if (currentAction == ActionE.None)
            CancelCurrentState();
    }

    public void SetPathToPosition(Vector2 pos)
    {
        path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);
    }

    public void GoToNextRouteNode()
    {
        SetPathToPosition(currentRoute.NextNode.Position);
    }

    public void SetRoute(NodePath route)
    {
        currentRoute = route;
        currentState = State.FollowRoute;
        currentAction = ActionE.FollowPath;
        SetPathToPosition(currentRoute.NextNode.Position);
    }

    protected void CancelCurrentState()
    {
        currentState = idleState;
        currentAction = idleAction;
        currentAlertIntensity = AlertIntensity.Nonexistant;
    }

    public void OnVisionEnter(Collider2D col)
    {
        if(CompareTag(col.tag))
        {
            if(!inVision.Contains(col) && !justEnteredVision.ContainsKey(col.gameObject.name))
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
            DieAnimation(dir);
            GeneralUI.Instance.Kills++;
            Destroy(gameObject);
            return;
        }

        if(currentState != State.Pursuit)
        {
            Alert(PlayerController.Instance.transform.position, AlertIntensity.ConfirmedHostile);
        }
    }

    protected abstract void DieAnimation(Vector3 dir);

    public void Incapacitate()
    {
        // raycast behind me
        Vector3 dir = transform.position - GetComponentInChildren<Vision>().transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir * 0.3f, dir, 1f);
        if (hit.collider == null ||!hit.collider.CompareTag("Player"))
        {
            IncapacitateFailedReaction();
            return;
        }
        isIncapacitated = true;
        incapacitateTimer.ResetTo(incapacitateTimer.Max * UnityEngine.Random.Range(0.5f, 1.5f));
    }

    public void GetZipTied()
    {
        if (!isIncapacitated)
        {
            IncapacitateFailedReaction();
            return;
        }
        isZipTied = true;
        cuffs.SetActive(true);
    }

    protected abstract void IncapacitateFailedReaction();

    public void ReleaseCuffs()
    {
        isIncapacitated = false;
        cuffs.SetActive(false);
    }


    protected abstract void PlayerSeen();

    void CreateBloodSplatter()
    {
        GameObject blood = new GameObject("Blood");
        blood.transform.parent = transform;
        blood.transform.position = transform.position + Vector3.back;
        blood.AddComponent<SpriteRenderer>();
        blood.AddComponent<Blood>();
    }
}
