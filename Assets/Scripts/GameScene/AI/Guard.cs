using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : AI
{
    public Pursue pursue;
    // Start is called before the first frame update
    void Start()
    {
        followPath = new FollowPath(this);
        actions.Add(ActionE.FollowPath, followPath);
        actions.Add(ActionE.LookAround, new LookAround(this));
        pursue = new Pursue(this, FindObjectOfType<PlayerController>().transform, followPath);
        actions.Add(ActionE.Pursue, pursue);
        currentState = State.FollowRoute;
        currentAction = ActionE.FollowPath;
        cuffs = Instantiate(Resources.Load<GameObject>("Prefabs/cuffs"), transform.position + new Vector3(0f, -0.3f, -1f), Quaternion.identity, transform);
        cuffs.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void PlayerSeen()
    {
        
        switch (currentState)
        {
            case State.Investigate:
                currentState = State.Pursuit;
                currentAction = ActionE.Pursue;
                break;
        }
    }

    public override bool Alert(Vector2 position, AlertType alertType, AlertIntesity alertIntesity)
    {
        if (isIncapacitated)
            return false;
        if (currentState == State.Pursuit)
            return true;

        switch (alertIntesity)
        {
            case AlertIntesity.NonHostile:
                StartInvestigate(position, alertType);
                break;
            case AlertIntesity.Nonexistant:
                StartInvestigate(position, alertType);
                break;
            case AlertIntesity.ConfirmedHostile:
                pursue.LastPlayerPos = position;
                currentState = State.Pursuit;
                currentAction = ActionE.Pursue;
                break;
        }

        return true;
    }

    public override void GetNextActionE()
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
        switch (currentAction)
        {
            case ActionE.Pursue:
                // Completely Lost Player
                // TODO: Search Action Here maybe?
                break;
        }
    }

    void Investigate()
    {
        switch (currentAction)
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

    protected override void DieAnimation(Vector3 dir)
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
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }
}
