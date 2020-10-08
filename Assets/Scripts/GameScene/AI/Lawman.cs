using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lawman : AI
{
    public Pursue pursue;
    protected Sprite deadHat;
    protected Sprite deadHead;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        actions.Add(ActionE.LookAround, new LookAround(this));
        pursue = new Pursue(this, FindObjectOfType<PlayerController>().transform, followPath);
        actions.Add(ActionE.HaltAndShoot, new HaltAndShoot(this));
        actions.Add(ActionE.Pursue, pursue);
        deadHead = Resources.Load<Sprite>("Textures/deadhead");
    }

    protected override void PlayerSeen()
    {

        switch (currentState)
        {
            case State.Investigate:
                currentState = State.Pursuit;
                currentAction = ActionE.Pursue;
                break;
            case State.Pursuit:
                if (currentAction != ActionE.Pursue && currentAction != ActionE.HaltAndShoot)
                {
                    // If we are not already in pursuit or shooting (looking for player), restart pursue.
                    pursue.LastPlayerPos = PlayerController.Instance.transform.position;
                    currentAction = ActionE.Pursue;
                }
                break;
        }
    }

    public override void GetNextAction(uint lastActionReturnValue)
    {
        currentAction = actions[currentAction].GetNextAction(currentState, lastActionReturnValue, currentAlertIntensity);
        if (currentAction == ActionE.None)
            CancelCurrentState();
    }

    protected void StartInvestigate(Vector2 position)
    {
        currentState = State.Investigate;
        SetPathToPosition(position);
        currentAction = ActionE.FollowPath;
    }

    protected override void DieAnimation(Vector3 dir)
    {
        GameObject temp = new GameObject("DeadHead");
        temp.AddComponent<SpriteRenderer>().sprite = deadHead;
        temp.transform.position = transform.position + Vector3.forward * 10f;
        temp = new GameObject("DeadBody");
        temp.transform.position = transform.position + Vector3.forward * 11f;
        temp.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        temp = new GameObject("DeadHat");
        float r = UnityEngine.Random.Range(0.4f, 0.8f);
        Vector3 hatDir = new Vector3(r * dir.normalized.x, dir.normalized.y * r, 9f);
        float rot = -90f;
        rot *= hatDir.x;
        temp.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rot));
        temp.transform.position = transform.position + hatDir;
        temp.AddComponent<SpriteRenderer>().sprite = deadHat;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }
}
