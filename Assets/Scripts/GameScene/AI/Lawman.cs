using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Lawman : AI
{
    public Pursue Pursue { get; set; }
    protected Sprite DeadHat;
    protected Sprite DeadHead;

    private protected float HaltTime = 2f;
    private protected float ShootTime = 2f;
    private AIWeaponHandler _weaponHandler;
   

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _weaponHandler = new AIWeaponHandler(GetComponent<AudioSource>(), ShootTime);
        Actions.Add(ActionE.LookAround, new LookAround(this));
        Pursue = new Pursue(this, FindObjectOfType<PlayerController>().transform, HaltTime);
        Actions.Add(ActionE.HaltAndShoot, new HaltAndShoot(this, _weaponHandler));
        Actions.Add(ActionE.Pursue, Pursue);
        DeadHead = Resources.Load<Sprite>("Textures/deadhead");
    }

    protected override void PlayerSeen()
    {

        switch (CurrentState)
        {
            case State.Investigate:
                CurrentState = State.Pursuit;
                CurrentAction = ActionE.Pursue;
                break;
            case State.Pursuit:
                if (CurrentAction != ActionE.Pursue && CurrentAction != ActionE.HaltAndShoot)
                {
                    // If we are not already in pursuit or shooting (looking for player), restart pursue.
                    Pursue.LastPlayerPos = PlayerController.Instance.transform.position;
                    CurrentAction = ActionE.Pursue;
                }
                break;
        }
    }

    public override void GetNextAction(uint lastActionReturnValue)
    {
        CurrentAction = Actions[CurrentAction].GetNextAction(CurrentState, lastActionReturnValue, CurrentAlertIntensity);
        if (CurrentAction == ActionE.None)
            CancelCurrentState();
    }

    protected void StartInvestigate(Vector2 position)
    {
        CurrentState = State.Investigate;
        SetPathToPosition(position);
        CurrentAction = ActionE.FollowPath;
    }

    protected override void DieAnimation(Vector3 dir)
    {
        GameObject temp = new GameObject("DeadHead");
        temp.AddComponent<SpriteRenderer>().sprite = DeadHead;
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
        temp.AddComponent<SpriteRenderer>().sprite = DeadHat;
    }

    protected override void IncapacitateFailedReaction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnVisionEnter(Collider2D col)
    {
        base.OnVisionEnter(col);
        if (col.CompareTag("lawmandestroy"))
        {
            _weaponHandler.Shoot(transform.position, col.transform.position);
        }
    }
}
