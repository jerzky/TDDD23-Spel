using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public enum AI_Type { Guard, Civilian, Bank_Worker, Construction_Worker }
public enum ActionE { None, Idle, FollowPath, LookAround, Pursue, HaltAndShoot, FindPathToRouteNode, Freeze, Flee };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute, Pursuit, Panic };
public enum AlertType { None, Guard_CCTV, Guard_Radio, Sound };
public enum AlertIntensity { Nonexistant, NonHostile, ConfirmedHostile }

public abstract class AI : MonoBehaviour
{
    // Vision Variables
    [SerializeField]
    public GameObject RotateVisionAround;
    [SerializeField]
    public Transform LeftOffsetPoint;
    [SerializeField]
    public Transform RightOffsetPoint;


    public List<Collider2D> InVision = new List<Collider2D>();
    protected SortedDictionary<int, float> JustEnteredVision = new SortedDictionary<int, float>();

    public SortedDictionary<ActionE, Action> Actions = new SortedDictionary<ActionE, Action>();
    public FollowPath FollowPath;

    // Move Variables
    public float MoveSpeed => CurrentState == State.Pursuit ? PursueSpeed : PatrolSpeed;

    public const float PursueSpeed = 3f;
    public const float PatrolSpeed = 2f;
    public float SpeedMultiplier = 1f;


    // Health
    protected int Health = 100;


    private readonly SimpleTimer _incapacitateTimer = new SimpleTimer(20);
    protected bool IsIncapacitated = false;
    protected bool IsZipTied = false;
    protected GameObject Cuffs;


    // StateMachineVariables
    protected State IdleState = State.FollowRoute;
    protected ActionE IdleAction = ActionE.FollowPath;
    protected State CurrentState = State.None;
    protected ActionE CurrentAction = ActionE.None;
    protected AlertIntensity CurrentAlertIntensity = AlertIntensity.Nonexistant;
    

    // Follow Route variables
    public List<Node> Path = new List<Node>();
    public NodePath CurrentRoute { get; private set; }
    public AI_Type AiType = AI_Type.Guard;
    private Building lastBuilding { get; set; }
    public Building CurrentBuilding
    {
        get
        {
            var building = BuildingController.Instance.GetBuilding(transform.position);
            if (building != null)
                lastBuilding = building;

            if (lastBuilding == null)
                Debug.LogError("THIS SHOULDNT HAPPEN IN THIS TESTING PHASE");
            return lastBuilding;
        }
    }
    protected virtual void Start()
    {
        Cuffs = Instantiate(Resources.Load<GameObject>("Prefabs/cuffs"), transform.position + new Vector3(0f, -0.3f, -1f), Quaternion.identity, transform);
        Cuffs.SetActive(false);
        FollowPath = new FollowPath(this);
        Actions.Add(ActionE.FollowPath, FollowPath);
        Actions.Add(ActionE.FindPathToRouteNode, new FindPathToRouteNode(this));
    }

    void FixedUpdate()
    {
        if (!IsZipTied && _incapacitateTimer.TickFixed())
            IsIncapacitated = false;
        // Reset variables that need to be reset every frame for functionality.
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        if (IsIncapacitated)
            return;
        uint actionReturnType = Actions[CurrentAction].PerformAction();
        if (actionReturnType != 0)
        {
            GetNextAction(actionReturnType);
        }

    }

    public AI_Type Type()
    {
        return AiType;
    }

    public bool Alert(Sound sound)
    {
        switch (sound.soundType)
        {
            case Sound.SoundType.Weapon:
                CurrentAlertIntensity = AlertIntensity.ConfirmedHostile;
                break;
            case Sound.SoundType.Construction:
                CurrentAlertIntensity = AlertIntensity.NonHostile;
                break;
            default:
                CurrentAlertIntensity = AlertIntensity.Nonexistant;
                break;
        }

        if(AiType != AI_Type.Civilian)
        {
            var building = CurrentBuilding;
            if (building != null && CurrentAlertIntensity == AlertIntensity.ConfirmedHostile)
                building.OnAlert(sound.origin, AlertType.Sound, AlertIntensity.ConfirmedHostile);
        }
        

        return Alert(sound.origin, CurrentAlertIntensity);
    }

    public virtual bool Alert(Vector2 position, AlertIntensity alertIntesity) { Debug.LogError("IMPLEMENT ALERT IN ALL CLASSES DERIVED FROM AI"); return true;  }

    public virtual void GetNextAction(uint lastActionReturnValue)
    {
        CurrentAction = Actions[CurrentAction].GetNextAction(CurrentState, lastActionReturnValue, CurrentAlertIntensity);
        if (CurrentAction == ActionE.None)
            CancelCurrentState();
    }

    public void SetPathToPosition(Vector2 pos)
    {
        Path.Clear();
        PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);
    }

    public void GoToNextRouteNode()
    {
        SetPathToPosition(CurrentRoute.NextNode.Position);
    }

    public void SetRoute(NodePath route)
    {
        CurrentRoute = route;
        CurrentState = State.FollowRoute;
        CurrentAction = ActionE.FollowPath;
        SetPathToPosition(CurrentRoute.NextNode.Position);
    }

    protected void CancelCurrentState()
    {
        CurrentState = IdleState;
        CurrentAction = IdleAction;
        CurrentAlertIntensity = AlertIntensity.Nonexistant;
    }

    public virtual void OnVisionEnter(Collider2D col)
    {
        if(CompareTag(col.tag))
        {
            if(!InVision.Contains(col) && !JustEnteredVision.ContainsKey(col.gameObject.GetInstanceID()))
                JustEnteredVision.Add(col.gameObject.GetInstanceID(), 0f);
        }
        if (col.CompareTag("Player"))
        {
            if (Utils.LineOfSight(transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast", "cctv")))
                PlayerSeen();
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if(JustEnteredVision.ContainsKey(col.gameObject.GetInstanceID()))
        {
            JustEnteredVision[col.gameObject.GetInstanceID()] += Time.deltaTime;
            if(JustEnteredVision[col.gameObject.GetInstanceID()] > 0.15f)
            {
                InVision.Add(col);
                JustEnteredVision.Remove(col.gameObject.GetInstanceID());
            }
        }
    }

    public void OnVisionExit(Collider2D col)
    {
        if (CompareTag(col.tag))
        {
            JustEnteredVision.Remove(col.gameObject.GetInstanceID());
            InVision.Remove(col);
        }
    }

    public void Injure(int damage, Vector3 dir)
    {
        CreateBloodSplatter();
        Health -= damage;
        if (Health <= 0)
        {
            DieAnimation(dir);
            GeneralUI.Instance.Kills++;
            Destroy(gameObject);
            return;
        }

        if(CurrentState != State.Pursuit)
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
        IsIncapacitated = true;
        _incapacitateTimer.ResetTo(_incapacitateTimer.Max * UnityEngine.Random.Range(0.5f, 1.5f));
    }

    public void GetZipTied()
    {
        if (!IsIncapacitated)
        {
            IncapacitateFailedReaction();
            return;
        }
        IsZipTied = true;
        Cuffs.SetActive(true);
    }

    protected abstract void IncapacitateFailedReaction();

    public void ReleaseCuffs()
    {
        IsIncapacitated = false;
        Cuffs.SetActive(false);
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
