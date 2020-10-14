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
        // call onalert on our building incase other guards are looking in the wrong place or this is first time player is noticed.
        // only call onalert if player is hostile or has been reported hostile in this building
        var building = CurrentBuilding;
        if(building != null)
            if (PlayerController.Instance.IsHostile || building.PlayerReportedAsHostile)
                building.OnAlert(PlayerController.Instance.transform.position, AlertType.Guard_Radio, AlertIntensity.ConfirmedHostile);

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
        temp.AddComponent<SpriteRenderer>().sprite = sprites[0];
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
            Debug.Log("Found a lawmandestroy");
            _weaponHandler.Shoot(transform.position, col.transform.position, true);
        }
    }
}
