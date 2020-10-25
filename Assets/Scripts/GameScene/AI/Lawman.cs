using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Lawman : AI
{
    public Pursue Pursue { get; set; }
    protected Sprite DeadHat;
    

    private protected float HaltTime = 2f;
    private protected float ShootTime = 2f;
    private AIWeaponHandler _weaponHandler;
    public static Vector2 LastSeenPlayerLocation { get; set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _weaponHandler = new AIWeaponHandler(GetComponent<AudioSource>(), ShootTime);
        Actions.Add(ActionE.LookAround, new LookAround(this));
        Pursue = new Pursue(this, FindObjectOfType<PlayerController>().transform, HaltTime);
        Actions.Add(ActionE.HaltAndShoot, new HaltAndShoot(this, _weaponHandler));
        Actions.Add(ActionE.Pursue, Pursue);
    }

    protected override void PlayerSeen()
    {
        // call onalert on our building incase other guards are looking in the wrong place or this is first time player is noticed.
        // only call onalert if player is hostile or has been reported hostile in this building

        //Pursue.LastPlayerPos = PlayerController.Instance.transform.position;
        LastSeenPlayerLocation = PlayerController.Instance.transform.position;
        switch (CurrentState)
        {
            case State.Pursuit:
                if(CurrentAction != ActionE.HaltAndShoot)
                    CurrentAction = ActionE.Pursue;
                break;
            default:
                CurrentState = State.Pursuit;
                CurrentAction = ActionE.Pursue;
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
        DeadBodyHolder = new GameObject("deadbodyholder");
        GameObject temp = new GameObject("DeadHead");
        temp.AddComponent<SpriteRenderer>().sprite = DeadHead;
        temp.transform.position = transform.position + Vector3.forward * 10f;
        temp.transform.parent = DeadBodyHolder.transform;
        temp = new GameObject("DeadBody");
        temp.transform.position = transform.position + Vector3.forward * 11f;
        temp.AddComponent<SpriteRenderer>().sprite = Sprites[0];
        temp.transform.parent = DeadBodyHolder.transform;
        temp = new GameObject("DeadHat");
        float r = UnityEngine.Random.Range(0.4f, 0.8f);
        Vector3 hatDir = new Vector3(r * dir.normalized.x, dir.normalized.y * r, 9f);
        float rot = -90f;
        rot *= hatDir.x;
        temp.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rot));
        temp.transform.position = transform.position + hatDir;
        temp.AddComponent<SpriteRenderer>().sprite = DeadHat;
        temp.transform.parent = DeadBodyHolder.transform;
    }

    protected override void IncapacitateFailedReaction()
    {
        
    }

    public override void OnVisionEnter(Collider2D col)
    {
        base.OnVisionEnter(col);
        if (col.CompareTag("lawmandestroy"))
        {
            _weaponHandler.Shoot(transform.position, col.transform.position, true);
        }
    }
}
