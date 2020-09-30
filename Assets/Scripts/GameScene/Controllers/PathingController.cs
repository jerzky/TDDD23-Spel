using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum NodeType { Clear, Blocked, Door, DoorNeighbour, AvailableAction, Visited };
public class Node 
{
    public Vector2 Position;
    public Node Parent;
    public float Distance;
    public Node Child;
    public Vector2 Dir;
    public Node(Vector2 position, Node parent, float distance, Vector2 dir)
    {
        Position = position;
        Parent = parent;
        Distance = distance;
        Dir = dir;
    }
}

public class PathFindingQueueItem
{
    public Vector2 StartPos { get; }
    public Vector2 EndPos { get; }
    public AI AI { get; }
    public bool willWalkThroughDoor = false;
    public PathFindingQueueItem(Vector2 startPos, Vector2 endPos, AI ai)
    {
        StartPos = startPos;
        EndPos = endPos;
        AI = ai;
    }
}

public class PathingController : MonoBehaviour
{
    public static PathingController Instance;
    public readonly Vector2[] neighbours = { Vector2.left, Vector2.right, Vector2.up, Vector2.down, Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right };
    float[] actionDistance = { 1f, 1f, 1f, 1f, 1.5f, 1.5f, 1.5f, 1.5f };
    NodeType[,] grid;
    Queue<PathFindingQueueItem> waitingQueue = new Queue<PathFindingQueueItem>();
    float delayBetweenPathFindings = 0;
    float maxDelayBetweenPathFindings = 0.012f;

    float delayBetweenDoorQueueChecks = 0f;
    float maxDelayBetweenDoorQueueChecks = 5f;

    Dictionary<Vector2, List<DoorQueueItem>> doors = new Dictionary<Vector2, List<DoorQueueItem>>();
    Dictionary<Vector2, Door> doorsScripts = new Dictionary<Vector2, Door>();

    public class DoorQueueItem
    {
        public AI AI;
        public float Distance;
        public DoorQueueItem(AI ai, float distance)
        {
            AI = ai;
            Distance = distance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        CreateNodeGrid();
        //FindObjectOfType<AI>().Alert(new Vector2(30, 90), AlertType.Investigate);
    }
    int t = 0;
    // Update is called once per frame
    void Update()
    {
        if(t == 0)
        {
            Bank.Instance.start();
            t++;
        }

        if (waitingQueue.Count > 0)
        {
            delayBetweenPathFindings -= Time.deltaTime;
            if(delayBetweenPathFindings <= 0f)
            {
                PathFindingQueueItem pfqi = waitingQueue.Dequeue();
                pfqi.AI.followPath.NodeToPathList(FindPath(pfqi.StartPos, pfqi.EndPos, (NodeType[,])grid.Clone()), "pathing Controller");
                delayBetweenPathFindings = maxDelayBetweenPathFindings;
            }
        }

        delayBetweenDoorQueueChecks += Time.deltaTime;
        if(delayBetweenDoorQueueChecks > maxDelayBetweenDoorQueueChecks)
        {
            delayBetweenDoorQueueChecks = 0f;
        }
    }

    public void CreateNodeGrid()
    {
        grid = new NodeType[(int)MapController.MapSize.x, (int)MapController.MapSize.y];
        for (int y = 0; y < MapController.MapSize.y; y++)
        {
            for (int x = 0; x < MapController.MapSize.x; x++)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(x, y), 0.45f);
                grid[x, y] = (uint)NodeType.Clear;
                foreach (var v in hits)
                {
                    BoxCollider2D bc = v.GetComponent<BoxCollider2D>();
                    if (bc != null && !bc.isTrigger)
                    {
                        if(v.GetComponent<Door>() != null)
                        {
                            grid[x, y] = NodeType.Door;
                            doors.Add(new Vector2(x, y), new List<DoorQueueItem>());
                            doorsScripts.Add(new Vector2(x, y), v.GetComponent<Door>());
                        }
                        else
                            grid[x, y] = NodeType.Blocked;
                        break;
                    }
                }
                if (y <= 0 || x <= 0 || x >= MapController.MapSize.x - 1 || y >= MapController.MapSize.y - 1)
                    grid[x, y] = NodeType.Blocked;
            }
        }


        foreach (var v in doors)
        {
            for(int i = 0; i < 8; i++)
            {
                Vector2 pos = v.Key + neighbours[i];
                if(grid[(int)pos.x, (int)pos.y] != NodeType.Blocked)
                {
                    grid[(int)pos.x, (int)pos.y] = NodeType.DoorNeighbour;
                }
            }
        }
    }

    public Door GetDoor(Vector2 pos)
    {
        if (doorsScripts.ContainsKey(pos))
            return doorsScripts[pos];
        else
            return null;
    }

    public int DoorQueueSize(Vector2 pos)
    {
        if (doors.ContainsKey(pos))
            return doors[pos].Count;
        else
            return 0;
    }

    public NodeType GetNodeType(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y];
    }

    public bool IsDoorNeighbour(Vector2 pos)
    {
        if (grid[(int)pos.x, (int)pos.y] == NodeType.Door || grid[(int)pos.x, (int)pos.y] == NodeType.DoorNeighbour)
            return true;

        return false;
    }

    public float RequestDoorAccess(Vector2 doorPosition, AI ai)
    {
        for(int i = 0; i < 8; i++)
        {
            Vector2 pos = doorPosition + neighbours[i];
            if(grid[(int)pos.x, (int)pos.y] == NodeType.Door)
            {
                doorPosition = pos;
                break;
            }
        }
        if (!doors.ContainsKey(doorPosition))
            return 0f;

        List<DoorQueueItem> queue = doors[doorPosition];
        DoorQueueItem contains = null;
        DoorQueueItem closest = null;
        for (int i = 0; i < queue.Count; i++)
        {
            var v = queue[i];
            v.Distance = Vector2.Distance(doorPosition, v.AI.transform.position);
            if(v.AI.name == ai.name)
            {
                contains = v;
            }
            if(closest == null || v.Distance < closest.Distance)
            {
                closest = v;
            }
        }
        if(contains != null)
        {
            if(closest.Distance == contains.Distance)
            {
                return 0f;
            }
        }
        else
        {
            queue.Add(new DoorQueueItem(ai, Vector2.Distance(doorPosition, ai.transform.position)));
            if (queue.Count == 1)
                return 0f;
        }
        float distance = 3f;
        queue = queue.OrderBy(c => c.Distance).ToList();
        foreach (var v in queue)
        {
            distance++;
            if (v.AI.name == ai.name)
                break;
        }
        return distance;
    }

    public void DoorPassed(Vector2 doorPosition, AI ai)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 pos = doorPosition + neighbours[i];
            if (grid[(int)pos.x, (int)pos.y] == NodeType.Door)
            {
                doorPosition = pos;
                break;
            }
        }

        if (doors.ContainsKey(doorPosition))
        {
            doors[doorPosition].Remove(doors[doorPosition].Find(c => c.AI.name == ai.name));
            if (doors[doorPosition].Count <= 0)
                doorsScripts[doorPosition].Close();
        }
            
    }

    public Node FindPathExcluding(Vector2 s, Vector2 g, List<Vector2> excluding)
    {
        if (grid[(int)Mathf.Round(g.x), (int)Mathf.Round(g.y)] != NodeType.Clear)
            return null;

        NodeType[,] gridCopy = (NodeType[,])grid.Clone();
        foreach(var v in excluding)
        {
            gridCopy[(int)v.x, (int)v.y] = NodeType.Blocked;
        }
        return FindPath(s, g, gridCopy);
    }

    public bool FindPath(Vector2 s, Vector2 g, AI ai, string from)
    {
        if (grid[(int)Mathf.Round(g.x), (int)Mathf.Round(g.y)] != NodeType.Clear)
            return false;

        Debug.Log("FIND PATH FROM: " + from + " goalpos: " + g);

        foreach(var v in waitingQueue)
        {
            if (v.AI.name == ai.name)
                return true;
        }

        waitingQueue.Enqueue(new PathFindingQueueItem(s, g, ai));
        return true;
    }

    Node FindPath(Vector2 s, Vector2 g, NodeType[,] gridCopy)
    {
        Vector2 startPos = new Vector2((int)Mathf.Round(s.x), (int)Mathf.Round(s.y));
        Vector2 goalPos = new Vector2((int)Mathf.Round(g.x), (int)Mathf.Round(g.y));
        Debug.Log("FIND PATH INTERNAL: goalpos: " + g);

        LinkedList<Node> availableActions = new LinkedList<Node>();
        Node current = new Node(startPos, null, 0f, Vector2.zero);
        AddNeighbours(current, availableActions, gridCopy);
        while (availableActions.Count > 0)
        {
            current = FindNextAction(availableActions, goalPos);
            gridCopy[(int)current.Position.x, (int)current.Position.y] = NodeType.Blocked;
            availableActions.Remove(current);

            if (current.Position == goalPos)
            {
                while(current.Parent != null)
                {
                    current.Parent.Child = current;
                    current = current.Parent;
                }
                return current;
            }
            AddNeighbours(current, availableActions, gridCopy);
        }
        return null;
    }

    Node FindNextAction(LinkedList<Node> availableActions, Vector2 goalPos)
    {
        Node best = availableActions.First.Value;
        double bestDist = ManhattanDistance(best.Position, goalPos);
        foreach (var v in availableActions)
        {
            if (v.Distance + ManhattanDistance(v.Position, goalPos) < best.Distance + bestDist || (v.Distance + ManhattanDistance(v.Position, goalPos) == best.Distance + bestDist && v.Dir == v.Parent.Dir))
            {
                // if v distance is shorter, or equal and does not need to change direction
                best = v;
                bestDist = ManhattanDistance(best.Position, goalPos);
            }
        }

        return best;
    }

    void AddNeighbours(Node origin, LinkedList<Node> availableActions, NodeType[,] gridCopy)
    {
        Vector2 current;
        for (int i = 0; i < neighbours.Length; i++)
        {
            current = origin.Position + neighbours[i];
            if (gridCopy[(int)current.x, (int)current.y] != NodeType.Blocked && (i < 4 || (gridCopy[(int)origin.Position.x, (int)current.y] != NodeType.Blocked && gridCopy[(int)current.x, (int)origin.Position.y] != NodeType.Blocked)))
            {
                float distance = actionDistance[i];
                if (gridCopy[(int)current.x, (int)current.y] == NodeType.DoorNeighbour)
                    distance += 1;
                availableActions.AddFirst(new Node(current, origin, origin.Distance + distance, neighbours[i])); 
                gridCopy[(int)current.x, (int)current.y] = NodeType.Blocked;
            }
        }
    }

    float ManhattanDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public bool IsClear(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y] == NodeType.Clear;
    }
}
