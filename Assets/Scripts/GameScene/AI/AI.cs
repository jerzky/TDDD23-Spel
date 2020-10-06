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

public enum AI_Type { Guard, Civilian, Bank_Worker, Construction_Worker }
public enum ActionE { None, Idle, FollowPath, LookAround, Pursue };
public enum State { None, Idle, IdleHome, Investigate, BathroomBreak, Civilian, FollowRoute, Pursuit };
public enum AlertType { None, Guard_CCTV, Guard_Radio, Sound };
public enum AlertIntesity { Nonexistant, NonHostile, ConfirmedHostile }

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

    public const float pursueSpeed = 5f;
    public const float patrolSpeed = 3f;
    public float speedMultiplier = 1f;


    // Health
    protected int health = 100;
    protected bool isIncapacitated = false;
    protected GameObject cuffs;


    // StateMachineVariables
    protected State idleState = State.FollowRoute;
    protected State currentState = State.None;
    protected ActionE currentAction = ActionE.None;
    protected AlertType currentAlertIntesity = AlertType.None;
    protected AlertType currentAlertType = AlertType.None;
    

    // Follow Route variables
    public List<Node> path = new List<Node>();
    protected NodePath currentRoute;

    AI_Type ai_type = AI_Type.Guard;
    // Start is called before the first frame update
    private void Start()
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

        if (isIncapacitated)
            return;

        if (actions[currentAction].PerformAction())
            GetNextActionE();

    }

    public AI_Type Type()
    {
        return ai_type;
    }

    public bool Alert(Sound sound)
    {
        AlertIntesity alertIntesity = AlertIntesity.Nonexistant;
        return Alert(sound.origin, AlertType.Sound, alertIntesity);
    }

    public abstract bool Alert(Vector2 position, AlertType alertType, AlertIntesity alertIntesity);


    public abstract void GetNextActionE();

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

    protected void CancelCurrentState()
    {
        CancelCurrentActionE();
        currentState = idleState;
        GetNextActionE();
    }

    protected void CancelCurrentActionE()
    {
        currentAction = ActionE.None;
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
            DieAnimation(dir);
            GeneralUI.Instance.Kills++;
            Destroy(gameObject);
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
