using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum NodeType { Clear, Blocked, Door, AvailableAction, Visited };
public class Node 
{
    public Vector2 Position;
    public Node Parent;
    public float Distance;
    public Node Child;
    public Node(Vector2 position, Node parent, float distance)
    {
        Position = position;
        Parent = parent;
        Distance = distance;
    }
}

public class PathFindingQueueItem
{
    public Vector2 StartPos { get; }
    public Vector2 EndPos { get; }
    public AI AI { get; }
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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingQueue.Count > 0)
        {
            delayBetweenPathFindings -= Time.deltaTime;
            if(delayBetweenPathFindings <= 0f)
            {
                PathFindingQueueItem pfqi = waitingQueue.Dequeue();
                pfqi.AI.nextNode = FindPath(pfqi.StartPos, pfqi.EndPos, (NodeType[,])grid.Clone());
                delayBetweenPathFindings = maxDelayBetweenPathFindings;
            }
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
                Debug.Log("Ray");
                foreach (var v in hits)
                {
                    BoxCollider2D bc = v.GetComponent<BoxCollider2D>();
                    if (bc != null && !bc.isTrigger)
                    {
                        if(v.GetComponent<Door>() != null)
                            grid[x, y] = NodeType.Door;
                        else
                            grid[x, y] = NodeType.Blocked;
                        Debug.Log("BoxCollider");
                        break;
                    }
                }
                if (y <= 0 || x <= 0 || x >= MapController.MapSize.x - 1 || y >= MapController.MapSize.y - 1)
                    grid[x, y] = NodeType.Blocked;
            }
        }
    }

    public Node FindPathExcluding(Vector2 s, Vector2 g, List<Vector2> excluding)
    {
        if (grid[(int)g.x, (int)g.y] != NodeType.Clear)
            return null;

        NodeType[,] gridCopy = (NodeType[,])grid.Clone();
        foreach(var v in excluding)
        {
            gridCopy[(int)v.x, (int)v.y] = NodeType.Blocked;
        }
        return FindPath(s, g, gridCopy);
    }

    public bool FindPath(Vector2 s, Vector2 g, AI ai)
    {
        if (grid[(int)g.x, (int)g.y] != NodeType.Clear)
            return false;

        waitingQueue.Enqueue(new PathFindingQueueItem(s, g, ai));
        return true;
    }

    Node FindPath(Vector2 s, Vector2 g, NodeType[,] gridCopy)
    {
        Vector2 startPos = new Vector2((int)s.x, (int)s.y);
        Vector2 goalPos = new Vector2((int)g.x, (int)g.y);

        LinkedList<Node> availableActions = new LinkedList<Node>();
        Node current = new Node(startPos, null, 0f);
        AddNeighbours(current, availableActions, gridCopy);
        while (availableActions.Count > 0)
        {
            current = FindNextAction(availableActions, goalPos);
            gridCopy[(int)current.Position.x, (int)current.Position.y] = NodeType.Visited;
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
            if (v.Distance + ManhattanDistance(v.Position, goalPos) < best.Distance + bestDist)
            {
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
            if (gridCopy[(int)current.x, (int)current.y] == NodeType.Clear && (i < 4 || (gridCopy[(int)origin.Position.x, (int)current.y] == NodeType.Clear && gridCopy[(int)current.x, (int)origin.Position.y] == NodeType.Clear)))
            {
                availableActions.AddFirst(new Node(current, origin, origin.Distance + actionDistance[i]));
                gridCopy[(int)current.x, (int)current.y] = NodeType.AvailableAction;
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
