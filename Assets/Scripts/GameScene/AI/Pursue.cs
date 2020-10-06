using UnityEngine;

public class Pursue : Action
{
    private readonly FollowPath _followPath;
    private readonly Transform _player;
    private readonly Threshold _pursueThreshold = new Threshold(7f, 5f);
    private readonly AIWeaponHandler _weaponHandler;
    private Vector2 _currentFollowPos;

    public Vector2 LastPlayerPos { get; private set; } = Vector2.zero;

    public Pursue(AI ai, Transform player, FollowPath followPath) : base(ai)
    {
        _player = player;
        _followPath = followPath;
        _weaponHandler = new AIWeaponHandler(ai.GetComponent<AudioSource>());
    }

    public override bool PerformAction()
    {
        if (LineOfSight())
        {
            // Player is in line of sight, move towards player.
            LastPlayerPos = _player.position;

            //Check if we should halt or if we're already standing still
            var needsToPursue = _pursueThreshold.GetValue(Vector2.Distance(LastPlayerPos, ai.transform.position));

            //Shoot or do we still have time left?
            if (!_weaponHandler.Shoot(ai.transform.position, _player.transform.position, needsToPursue))
                return false;

            //IDK if we need this, by having this the ai wont get closer than 5f
            if (!needsToPursue)
                return false;
            
            //We need to move closer to the player
            Vector2 dir = _player.position - ai.transform.position;
            ai.GetComponent<Rigidbody2D>()
                .MovePosition((Vector2) ai.transform.position + dir.normalized * 3f * Time.fixedDeltaTime);

            return false;
        }

        // We lost line of sight, check if our current path is to the correct location
        if (_currentFollowPos == LastPlayerPos)
            return _followPath.PerformAction();

        // update path
        _currentFollowPos = LastPlayerPos;
        ai.SetPathToPosition(LastPlayerPos);
        return _followPath.PerformAction();
    }

    public bool LineOfSight()
    {
        Vector2 dir = (_player.position - ai.transform.position).normalized;
        var layerMask = ~LayerMask.GetMask("AI");
        var hit = Physics2D.Raycast(ai.transform.position, dir, Mathf.Infinity, layerMask);
        return hit.collider.CompareTag("Player");
    }
}