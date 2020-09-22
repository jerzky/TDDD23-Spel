using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Node
{ 
    public enum NodeType { Regular, Door };
    public uint UID;
    public GameObject GameObject;
    public List<Node> Neighbours { get; }
    public NodeType Type;
    public Vector3 Position { get { return GameObject.transform.position; } }
    public Node(GameObject gameObject, NodeType type)
    {
        Type = type;
        this.GameObject = gameObject;
        Neighbours = new List<Node>();
    }


    public void AddNeighbours(List<Node> nodes)
    {
        int indexOfDash = GameObject.name.IndexOf('-');
        int length = indexOfDash;
        if (indexOfDash == -1)
            length = GameObject.name.Length;
        this.UID = uint.Parse(GameObject.name.Substring(0, length));


        if (indexOfDash != -1)
        {
            for (int i = indexOfDash + 1; i < GameObject.name.Length; i++)
            {
                Neighbours.Add(nodes[int.Parse(GameObject.name[i].ToString())]);
            }
        }
        else
        {
            for (int i = 0; i < nodes.Count; i++)
                if (i != UID)
                    Neighbours.Add(nodes[i]);
        }
    }
}


public class PathingController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> lobbyNodes;
    [SerializeField]
    List<GameObject> officeNodes;
    List<NodeSystem> nodeSystems;
    public static PathingController Instance;
// Start is called before the first frame update
void Start()
    {
        Instance = this;
        nodeSystems = new List<NodeSystem>();

        List<SortedDictionary<uint, Node>> doorDicts = new List<SortedDictionary<uint, Node>> { new SortedDictionary<uint, Node>(), new SortedDictionary<uint, Node>() };
        AddDoorsToDicts(doorDicts);
        nodeSystems.Add(new NodeSystem(0, CreateNodesFromGO(lobbyNodes), doorDicts[0], 1));
        nodeSystems.Add(new NodeSystem(1, CreateNodesFromGO(officeNodes), doorDicts[1], 0));

    }

    void AddDoorsToDicts(List<SortedDictionary<uint, Node>> doorDicts)
    {
        foreach(var v in FindObjectsOfType<Door>())
        {
            Node temp = new Node(v.gameObject, Node.NodeType.Door);
            if (!int.TryParse(v.gameObject.name[0].ToString(), out int unused))
                continue;
            uint first = uint.Parse(v.gameObject.name.Substring(0, v.gameObject.name.IndexOf('-')));
            uint second = uint.Parse(v.gameObject.name.Substring(v.gameObject.name.IndexOf('-')+1));
            doorDicts[(int)first].Add(second, temp);
            doorDicts[(int)second].Add(first, temp);
        }
    }

    List<Node> CreateNodesFromGO(List<GameObject> g)
    {
        List<Node> result = new List<Node>();
        foreach(var v in g)
        {
            result.Add(new Node(v, Node.NodeType.Regular));
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Node> GetPath(Vector3 from, Vector3 to)
    {
        List<Node> result = new List<Node>();
        List<uint> sectionPath = GetNodeSystemPath(from, to);

        result.Add(nodeSystems[(int)sectionPath[0]].FindClosestNode(from));
        for (int i = 0; i < sectionPath.Count; i++)
        {
            var currentNodeSystem = nodeSystems[(int)sectionPath[i]];
            Node currentFrom = currentNodeSystem.FindClosestNode(from);
            Vector3 currentTo = to;
            if (i+1 < sectionPath.Count)
                currentTo = currentNodeSystem.FindClosestNode(currentNodeSystem.doorNodes[sectionPath[i+1]].Position).Position;

            
            List<Node> temp = currentNodeSystem.GetPathToIdle(currentFrom, currentTo);
            result.AddRange(temp);
        }
        return result;
    }

    List<uint> GetNodeSystemPath(Vector3 from, Vector3 to)
    {
        List<uint> result = new List<uint>();
        uint[] visited = new uint[nodeSystems.Count];
        Queue<Tuple<uint, uint>> frontier = new Queue<Tuple<uint, uint>>();
        var start = new Tuple<uint, uint>(GetSection(from), 404);
        frontier.Enqueue(start);
        visited[frontier.Peek().Item1] = 404;

        uint destination = GetSection(to);

        while(frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            foreach(var v in nodeSystems[(int)current.Item1].SectionNeighbours.FindAll(c => visited[c] == 0))
            {
                if(v == destination)
                {
                    uint temp = v;
                    while(temp != 404)
                    {
                        result.Add(temp);
                        temp = visited[temp];
                    }
                    result.Reverse();
                    return result;
                }
                else
                {
                    visited[v] = current.Item1;
                    frontier.Enqueue(new Tuple<uint, uint>(v, current.Item1));
                }
            }
        }
        result.Add(start.Item1);
        return result;
    }

    uint GetSection(Vector3 position)
    {
        //TODO: write a better version of this
        uint system = 0;
        for(int i = 0; i < nodeSystems.Count; i++)
        {
            var v = nodeSystems[i];
            if (Vector2.Distance(v.FindClosestNode(position).Position, position) < Vector2.Distance(nodeSystems[(int)system].FindClosestNode(position).Position, position))
                system = (uint)i;
        }

        return system;
    }
}
