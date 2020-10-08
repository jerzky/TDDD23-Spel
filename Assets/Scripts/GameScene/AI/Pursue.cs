using UnityEngine;


public class Pursue : Action
{
    public enum ReturnType : uint { NotFinished, InFireRange, ReadyToHaltAndShoot, LostLineOfSight }
    private readonly FollowPath _followPath;
    private readonly Transform _player;
    private readonly Threshold _pursueThreshold = new Threshold(9f, 7f);
    private Vector2 _currentFollowPos;
    private readonly SimpleTimer _haltTimer = new SimpleTimer(2f);
    bool b = false;
    public Vector2 LastPlayerPos { get; set; } = Vector2.zero;

    public Pursue(AI ai, Transform player, FollowPath followPath) : base(ai)
    {
        _player = player;
        _followPath = followPath;
    }

    public override uint PerformAction()
    {
        if (LineOfSight())
        {
            // Player is in line of sight, move towards player.
            LastPlayerPos = _player.position;

            //Check if we should halt or if we're already standing still
            if (!_pursueThreshold.GetValue(Vector2.Distance(LastPlayerPos, ai.transform.position)))
                 return (uint)ReturnType.InFireRange;

            if (_haltTimer.Tick())
            {
                _haltTimer.Reset();
                return (uint)ReturnType.ReadyToHaltAndShoot;
            }

            
            //We need to move closer to the player
            Vector2 dir = _player.position - ai.transform.position;
            ai.GetComponent<Rigidbody2D>()
                .MovePosition((Vector2) ai.transform.position + dir.normalized * 3f * Time.fixedDeltaTime);
                
            return (uint)ReturnType.NotFinished;
        }
        
        Debug.Log("We have no line of sight, follow path");
        // We lost line of sight, check if our current path is to the correct location
        if (_currentFollowPos == LastPlayerPos)
            return (uint)ReturnType.LostLineOfSight;

        // update path
        Debug.Log(LastPlayerPos + " p: " + _player.position);
        _currentFollowPos = LastPlayerPos;
        ai.SetPathToPosition(LastPlayerPos);
        return (uint)ReturnType.LostLineOfSight;
    }

    public bool LineOfSight()
    {
        Vector2 dir = (_player.position - ai.transform.position).normalized;
        var layerMask = ~LayerMask.GetMask("AI", "Ignore Raycast");
        var hit = Physics2D.Raycast(ai.transform.position, dir, Mathf.Infinity, layerMask);
        if (hit.collider == null)
            return false;
        Debug.Log("LINE OF SIGHT HIT: " + hit.collider.name);
        if (hit.collider.CompareTag("Player"))
        {
            LastPlayerPos = _player.transform.position;
            return true;
        }
        return false; 
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint)Pursue.ReturnType.InFireRange || lastActionReturnValue == (uint)Pursue.ReturnType.ReadyToHaltAndShoot)
        {
            if (PlayerController.Instance.IsHostile || alertIntensity == AlertIntensity.ConfirmedHostile)
            {
                (ai.actions[ActionE.HaltAndShoot] as HaltAndShoot).ResetShootTimer();
                return ActionE.HaltAndShoot;
            }
            else if (alertIntensity == AlertIntensity.NonHostile)
            {
                GeneralUI.Instance.TriggerInfoText("I would strongly recommend you leave.");
            }
            else if (alertIntensity == AlertIntensity.Nonexistant)
            {
                GeneralUI.Instance.TriggerInfoText("I don't wanna see you lurking around here again!");
            }
        }
        else if (lastActionReturnValue == (uint)Pursue.ReturnType.LostLineOfSight)
            return ActionE.FollowPath;

        return ActionE.None;
    }
}