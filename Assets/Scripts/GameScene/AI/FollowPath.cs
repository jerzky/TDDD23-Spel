using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : Action
{
    public enum ReturnType { NotFinished, Finished, StartedWithoutPath }
    public bool isWaitingForPath = false;
    bool movingThroughDoor = false;
    bool walkingPassedDoor = false;
    float hasNotMovedTimer = 0f;
    Vector3 lastMovePos = Vector3.zero;
    Vector2 lastDoorPos = new Vector2(-1, -1);
    Vector2 lastDoorNeighbourPos = new Vector2(-1, -1);

    // Collision Avoidance variables
    Vector2[] dirToLeft = { Vector2.down, Vector2.up, Vector2.left, Vector2.right, Vector2.down + Vector2.left, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.up + Vector2.right };
    Vector2[] dirToRight = { Vector2.up, Vector2.down, Vector2.right, Vector2.left, Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right };
    float[] angle = { 270f, 90f, 180f, 0f, 225f, 135f, 315f, 45f };
    Vector2 offset = Vector2.zero;
    float offsetForceMultiplier = 0f;
    float forceMultiplierSpeed = 2f;

    public FollowPath(AI ai) : base(ai)
    {

    }

    public override uint PerformAction()
    {
        if (ai.path.Count <= 0)
            return (uint) ReturnType.StartedWithoutPath;

        if (IsStuck())
            if (RecalculatePath())
                return (uint) ReturnType.NotFinished;

        if (ShouldWaitForDoor())
            return (uint)ReturnType.NotFinished;

        Vector2 dir = (ai.path[0].Position - (Vector2)ai.transform.position).normalized;
        SetOffset(FindClosestAIInVision());

        Move(dir);
        if (Vector2.Distance(ai.transform.position, ai.path[0].Position) < 0.1f)
            FinishedNode();

        if (ai.path.Count <= 0)
            return (uint)ReturnType.Finished;

        return (uint)ReturnType.NotFinished;
    }

    bool RecalculatePath()
    {
        hasNotMovedTimer = 0f;
        Node current = ai.path[0];
        while (current.Child != null)
        {
            current = current.Child;
        }
        NodeToPathList(PathingController.Instance.FindPathExcluding(new Vector2(Mathf.Round(ai.transform.position.x), Mathf.Round(ai.transform.position.y)), current.Position, new List<Vector2> { ai.path[0].Position }));
        if (lastDoorNeighbourPos != new Vector2(-1, -1))
        {
            PathingController.Instance.DoorPassed(lastDoorNeighbourPos, ai);
        }
        if (ai.path.Count == 0)
        {
            isWaitingForPath = false;
            return true;
        }
        return false;
    }

    Collider2D FindClosestAIInVision()
    {
        Collider2D closest = null;
        if (true)
        {
            if (ai.inVision.Count > 0)
            {
                ai.speedMultiplier = 1f;

                foreach (var v in ai.inVision)
                {
                    Vector2 vDir = Vector2.zero;
                    if (v.GetComponent<AI>().path.Count > 0 && v.GetComponent<AI>().path[0].Parent != null)
                        vDir = v.GetComponent<AI>().path[0].Position - v.GetComponent<AI>().path[0].Parent.Position;
                    Vector2 myDir = Vector2.one;
                    if (ai.path.Count > 0 && ai.path[0].Parent != null)
                        myDir = ai.path[0].Position - ai.path[0].Parent.Position;

                    RaycastHit2D hit = Physics2D.Raycast(ai.transform.position, v.transform.position - ai.transform.position);
                    if (!hit.collider.CompareTag("humanoid"))
                        continue;

                    if ((vDir != myDir || Vector2.Distance(ai.transform.position, v.transform.position) < 1f) && (closest == null || Vector2.Distance(v.transform.position, ai.transform.position) < Vector2.Distance(closest.transform.position, ai.transform.position)))
                    {
                        closest = v;
                    }
                }
            }
        }
        return closest;
    }

    bool IsStuck()
    {


        if (Vector2.Distance(lastMovePos, ai.transform.position) > 1f)
        {
            lastMovePos = ai.transform.position;
            hasNotMovedTimer = 0f;
        }
        else
        {
            hasNotMovedTimer += Time.fixedDeltaTime;
            if ((hasNotMovedTimer > 1f && !movingThroughDoor) || (hasNotMovedTimer > 2f))
            {
                hasNotMovedTimer = 0f;
                return true;
            }
        }
        return false;
    }

    bool ShouldWaitForDoor()
    {
        if (!walkingPassedDoor && PathingController.Instance.IsDoorNeighbour(ai.path[0].Position))
        {
            float distance = PathingController.Instance.RequestDoorAccess(ai.path[0].Position, ai);
            if (distance > 0)
            {
                // We are heading for a door, and do not have door access yet.
                lastDoorNeighbourPos = ai.path[0].Position;
                if (Vector2.Distance(ai.transform.position, ai.path[0].Position) <= distance)
                    return true;
            }
            else
            {
                walkingPassedDoor = true;
                lastDoorNeighbourPos = ai.path[0].Position;
            }
        }
        else if (walkingPassedDoor)
        {
            if (PathingController.Instance.GetNodeType(ai.path[0].Position) == NodeType.Door)
            {
                // Next node is door
                Door door = PathingController.Instance.GetDoor(ai.path[0].Position);
                movingThroughDoor = true;
                if (door.IsClosed())
                {
                    // open door if closed
                    door.Open();
                    return true;
                }
                else if (!door.IsOpen())
                {
                    // wait for door to fully open
                    return true;
                }
            }
        }
        return false;
    }

    void Move(Vector2 dir)
    {
        dir += offset.normalized * offsetForceMultiplier;

        float angle = Mathf.Atan2(dir.x, dir.y) * 180 / Mathf.PI;
        ai.rotateVisionAround.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

        ai.GetComponent<Rigidbody2D>().MovePosition((Vector2)ai.transform.position + dir.normalized * ai.moveSpeed * Time.fixedDeltaTime);
    }

    void FinishedNode()
    {
        movingThroughDoor = false;
        if (walkingPassedDoor && !PathingController.Instance.IsDoorNeighbour(ai.path[0].Child.Position))
        {
            PathingController.Instance.DoorPassed(lastDoorNeighbourPos, ai);
            walkingPassedDoor = false;
            lastDoorNeighbourPos = new Vector2(-1, -1);
        }
        ai.path.RemoveAt(0);
    }

    void SetOffset(Collider2D closest)
    {
        if (closest == null)
        {
            offsetForceMultiplier -= Time.fixedDeltaTime * forceMultiplierSpeed;
            if (offsetForceMultiplier <= 0)
            {
                offsetForceMultiplier = 0f;
                offset = Vector2.zero;
            }
        }
        else
        {
            Transform closestOffsetPoint = closest.GetComponent<AI>().leftOffsetPoint;
            if (Vector2.Distance(closest.GetComponent<AI>().rightOffsetPoint.position, ai.transform.position) < Vector2.Distance(closestOffsetPoint.position, ai.transform.position))
                closestOffsetPoint = closest.GetComponent<AI>().rightOffsetPoint;

            offset = closestOffsetPoint.position - closest.transform.position;
            offsetForceMultiplier += Time.fixedDeltaTime * forceMultiplierSpeed;
        }

    }

    public void NodeToPathList(Node startNode)
    {
        if (ai == null)
            return;
        Node node = startNode;
        if (node == null)
            return;
        ai.path.Clear();
        
        if (node.Child == null)
        {
            ai.path.Add(node);
            return;
        }
        
        Vector2 dir = node.Position - (Vector2)ai.transform.position;
        Vector2 prevdir = dir;
        node = node.Child;
        while (node != null)
        {
            dir = node.Position - node.Parent.Position;
            // Add Node to list if the direction changes
            if (dir != prevdir)
            {
                ai.path.Add(node.Parent);
            }
            else if (PathingController.Instance.IsDoorNeighbour(node.Parent.Position))
            {
                // We should check for doornodes and add the closest nodes to it aswell as the door node.
                ai.path.Add(node.Parent);
            }
            else if(node.Child == null)
            {
                ai.path.Add(node);
            }
            node = node.Child;
            prevdir = dir;
        }
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
    {
        if (lastActionReturnValue == (uint)ReturnType.StartedWithoutPath && currentState == State.FollowRoute)
            return ActionE.FindPathToRouteNode;

        switch(currentState)
        {
            case State.Pursuit:
                if ((ai as Guard).pursue.LineOfSight())
                    return ActionE.Pursue;
                else
                    return ActionE.LookAround;
            case State.Investigate:
                return ActionE.LookAround;
        }
        return ActionE.None;
    }
}
