using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pursue : Action
{
    Transform player;
    public Vector2 lastPlayerPos = Vector2.zero;
    private Vector2 currentFollowPos;
    private FollowPath followPath;
    public Pursue(AI ai, Transform player, FollowPath followPath) : base(ai)
    {
        this.player = player;
        this.followPath = followPath;
    }
    public override bool PerformAction()
    {
        if(LineOfSight())
        {
            Debug.Log("Pursuing, in line of sight");
            // Player is in line of sight, move towards player.
            Vector2 dir = player.position - ai.transform.position;
            ai.GetComponent<Rigidbody2D>().MovePosition(ai.transform.position + new Vector3(dir.x, dir.y, 0f).normalized * ai.walkingSpeed * Time.fixedDeltaTime);
            lastPlayerPos = player.position;
            return false;
        }
        else
        {
            // We lost line of sight, check if our current path is to the correct location
            if(currentFollowPos != lastPlayerPos)
            {
                // update path
                currentFollowPos = lastPlayerPos;
                ai.SetPathToPosition(lastPlayerPos);
            }
            return followPath.PerformAction();
        }
    }

    public bool LineOfSight()
    {
        Vector2 dir = (player.position - ai.transform.position).normalized;
        int layerMask = ~LayerMask.GetMask("AI");
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ai.transform.position, dir, Mathf.Infinity, layerMask);
        Debug.Log(hit.collider.name);
        if (hit.collider.CompareTag("Player"))
            return true;
        return false;
    }
}
