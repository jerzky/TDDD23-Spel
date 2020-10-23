using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public enum AI_Type { Guard, Civilian, Bank_Worker, Construction_Worker, Police, Medical_Worker }
public enum ActionE { None, Idle, FollowPath, LookAround, Pursue, HaltAndShoot, FindPathToRouteNode, GotoCoverEntrance, HoldCoverEntrance, Freeze, Flee,
    FindRoomToClear, ClearRoom, WaitingForAllPolice, RemoveBody, Rebuild
};
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute, Pursuit, GotoCoverEntrance, StormBuilding, Panic, PoliceGoToCar, WaitingForAllPolice, Medical, Construction };

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

    public const float LOOKRANGE = 7f;

    protected Sprite[] Sprites = new Sprite[4];
    private Vector2 _lastPos; // used to determine movement direction to assign sprite


    public List<Collider2D> InVision = new List<Collider2D>();
    protected SortedDictionary<int, float> JustEnteredVision = new SortedDictionary<int, float>();

    public SortedDictionary<ActionE, Action> Actions = new SortedDictionary<ActionE, Action>();
    public FollowPath FollowPath;

    // Move Variables
    public float MoveSpeed => CurrentState == State.Pursuit ? PursueSpeed : PatrolSpeed;

    public const float PursueSpeed = 3f;
    public const float PatrolSpeed = 2f;
    public float SpeedMultiplier = 1f;
    public int LookDir = 0;
  

    // Health
    protected int Health = 100;


    private readonly SimpleTimer _incapacitateTimer = new SimpleTimer(20);
    protected bool IsIncapacitated = false;
    protected bool IsZipTied = false;
    protected GameObject Cuffs;

    protected GameObject DeadBodyHolder;
    protected Sprite DeadHead;


    // StateMachineVariables
    protected State IdleState = State.FollowRoute;
    protected ActionE IdleAction = ActionE.FollowPath;
    public State CurrentState { get; set; }
    public ActionE CurrentAction { get; set; } = ActionE.None;

    protected AlertIntensity CurrentAlertIntensity = AlertIntensity.Nonexistant;

    // Follow Route variables
    public List<Node> Path = new List<Node>();
    public NodePath CurrentRoute { get; protected set; }

    private GameObject _incapacitatedVisual;

    public AI_Type AiType = AI_Type.Guard;
    public Building CurrentBuilding
    {
        get
        {
            var building = BuildingController.Instance.GetBuilding(transform.position);
            return building;
        }
    }
    protected virtual void Start()
    {
        Cuffs = Instantiate(Resources.Load<GameObject>("Prefabs/cuffs"), transform.position + new Vector3(0f, -0.3f, -1f), Quaternion.identity, transform);
        Cuffs.SetActive(false);
        FollowPath = new FollowPath(this);
        Actions.Add(ActionE.FollowPath, FollowPath);
        Actions.Add(ActionE.FindPathToRouteNode, new FindPathToRouteNode(this));
        Actions.Add(ActionE.Idle, new IdleAtRouteNode(this));
        CurrentAction = IdleAction;
        CurrentState = IdleState;

        _incapacitatedVisual = Instantiate(Resources.Load<GameObject>("Prefabs/incap"), transform.position + new Vector3(-0.1f, 0.1f, -1f), Quaternion.identity, transform);
        DeadHead = Resources.Load<Sprite>("Textures/deadhead");
    }

    protected virtual void FixedUpdate()
    {
        // Reset velocity to 0, to remove any forces applied from collision. Otherwise characters will glide
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        if (!IsZipTied && _incapacitateTimer.TickFixed())
            IsIncapacitated = false;

        if (IsIncapacitated)
        {
            _incapacitatedVisual.SetActive(true);
            return;
        }
        else
            _incapacitatedVisual.SetActive(false);

        

        CheckForPlayerInVision();

        HandleCharacterRotation();
        uint actionReturnType = Actions[CurrentAction].PerformAction();
        if (actionReturnType != 0)
        {
            GetNextAction(actionReturnType);
        }

    }

    void CheckForPlayerInVision()
    {
        if (Utils.LineOfSight(transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast", "cctv")) &&
            Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < LOOKRANGE)
            PlayerSeen();

    }

    void HandleCharacterRotation()
    {
        Vector2 dir;
        if(CurrentAction == ActionE.Pursue || CurrentAction == ActionE.HaltAndShoot)
            dir = (Vector2)(PlayerController.Instance.transform.position - transform.position);
        else
            dir = (Vector2)transform.position - _lastPos;

        _lastPos = (Vector2)transform.position;
        if (dir != Vector2.zero)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))  LookDir = dir.x > 0f ? 3 : 2;
            else LookDir = dir.y > 0f ? 1 : 0;
            GetComponent<SpriteRenderer>().sprite = Sprites[LookDir];
            float angle = -Mathf.Atan2(dir.x, dir.y) * 180 / Mathf.PI;
            RotateVisionAround.transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

        CurrentAction = Actions[CurrentAction]
            .GetNextAction(CurrentState, lastActionReturnValue, CurrentAlertIntensity);
        if (CurrentAction == ActionE.None)
            CancelCurrentState();
    }

    public void SetPathToPosition(Vector2 pos)
    {
        Path.Clear();
        FollowPath.IsWaitingForPath = PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), pos, this);
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
        if(CompareTag(col.tag)) // if tag is humanoid
        {
            if(!InVision.Contains(col) && !JustEnteredVision.ContainsKey(col.gameObject.GetInstanceID()))
                JustEnteredVision.Add(col.gameObject.GetInstanceID(), 0f);
        }
        else if (col.CompareTag("Player"))
        {
            /*if (Utils.LineOfSight(transform.position, PlayerController.Instance.gameObject, ~LayerMask.GetMask("AI", "Ignore Raycast", "cctv")))
                PlayerSeen();*/
        }
        else if(col.CompareTag("broken"))
        {
            ConstructionWorker con = FindObjectOfType<ConstructionWorker>();
            if (con == null)
            {
                GameController.Instance.AddBreakable(col.gameObject);
                return;
            }
            con.AssignObject(col.gameObject);
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if (!JustEnteredVision.ContainsKey(col.gameObject.GetInstanceID()))
            return;

        JustEnteredVision[col.gameObject.GetInstanceID()] += Time.deltaTime;

        if (JustEnteredVision[col.gameObject.GetInstanceID()] <= 0.15f)
            return;

        InVision.Add(col);
        JustEnteredVision.Remove(col.gameObject.GetInstanceID());

    }

    public void OnVisionExit(Collider2D col)
    {
        if (!CompareTag(col.tag)) 
            return;

        JustEnteredVision.Remove(col.gameObject.GetInstanceID());
        InVision.Remove(col);
    }

    public void Injure(int damage, Vector3 dir)
    {
        CreateBloodSplatter();
        Health -= damage;
        if (Health <= 0)
        {
            DieAnimation(dir);
            OnDeath();
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

    public virtual void OnDeath()
    {
        if (AiType != AI_Type.Police)
            GameController.Instance.AddToRespawnQueue(new GameController.RespawnInfo(AiType, CurrentRoute));
        MedicalWorker med = FindObjectOfType<MedicalWorker>();
        DeadBodyHolder.tag = "body";
        if (med == null)
        {
            GameController.Instance.AddBody(DeadBodyHolder);
            return;
        }
        
        med.AssignBody(DeadBodyHolder);
    }
    public void Incapacitate()
    {
        Transform player = PlayerController.Instance.transform;
        // raycast behind me
        switch(LookDir)
        {
            case 0: // down
                if(transform.position.y > player.position.y || Mathf.Abs(transform.position.x - player.position.x) > 0.5f)
                    IncapacitateFailedReaction();
                break;
            case 1: // up
                if (transform.position.y < player.position.y || Mathf.Abs(transform.position.x - player.position.x) > 0.5f)
                    IncapacitateFailedReaction();
                break;
            case 2: // left
                if (transform.position.x > player.position.x || Mathf.Abs(transform.position.y - player.position.y) > 0.5f)
                    IncapacitateFailedReaction();
                break;
            case 3: // right
                if (transform.position.x < player.position.x || Mathf.Abs(transform.position.y - player.position.y) > 0.5f)
                    IncapacitateFailedReaction();
                break;
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
