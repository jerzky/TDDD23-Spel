using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ANode
{
    public Node Node;
    public ANode Parent;
    public float Distance;
    public ANode(Node node, ANode parent)
    {
        Node = node;
        Parent = parent;
    }

    public ANode(Node node, ANode parent, float distance)
    {
        Node = node;
        Parent = parent;
        Distance = distance;
    }
}

public class NodeSystem
{
    uint id;
    public List<Node> nodes;
    public List<uint> SectionNeighbours;
    public SortedDictionary<uint, Node> doorNodes;

    public NodeSystem(uint id, List<Node> nodes, SortedDictionary<uint, Node> doorNodes, params uint[] neighbours) 
    {
        this.doorNodes = doorNodes;
        this.id = id;
        this.nodes = nodes;
        SectionNeighbours = new List<uint>();
        SectionNeighbours.AddRange(neighbours);

        foreach(var v in nodes)
        {
            // Add all neighbours to regular nodes
            v.AddNeighbours(nodes);
        }
        foreach(var v in doorNodes)
        {
            // Add closest node as neighbour to all doorNodes
            Node closest = nodes[0];
            foreach (var n in nodes)
                if (Vector2.Distance(n.Position, v.Value.Position) < Vector2.Distance(closest.Position, v.Value.Position))
                    closest = n;

            v.Value.Neighbours.Add(closest);
            closest.Neighbours.Add(v.Value);
        }
    }

    public List<Node> GetPathToUrgent(Vector3 currentPosition, Vector3 destination)
    {
        Node startingNode = nodes[0];
        foreach (var v in nodes)
        {
            UpdateIfCloser(currentPosition, destination, startingNode, v);
        }

        foreach(var v in doorNodes)
        {
            UpdateIfCloser(currentPosition, destination, startingNode, v.Value);
        }

        List<Node> result = GetPathTo(startingNode, destination);
        result.Add(startingNode);
        result.Reverse();
        return result;
    }

    void UpdateIfCloser(Vector3 currentPosition, Vector3 destination, Node startingNode, Node current)
    {
        float disTot1 = Vector2.Distance(currentPosition, current.Position) + Vector2.Distance(current.Position, destination);
        float disTot2 = Vector2.Distance(currentPosition, startingNode.Position) + Vector2.Distance(startingNode.Position, destination);
        if (disTot1 < disTot2)
            startingNode = current;
        else if (disTot1 == disTot2)
        {
            if (Vector2.Distance(currentPosition, current.Position) < Vector2.Distance(currentPosition, startingNode.Position))
                startingNode = current;
        }
    }

    public List<Node> GetPathToIdle(Node startingNode, Vector3 destination)
    {
        List<Node> result = GetPathTo(startingNode, destination);
        
        result.Reverse();
        return result;
    }

    public List<Node> GetPathTo(Node startingNode, Vector3 destination)
    {
        Node destinationNode = nodes[0];
        foreach(var v in nodes)
        {
            if (Vector2.Distance(v.Position, destination) < Vector2.Distance(destinationNode.Position, destination))
                destinationNode = v;
        }
        foreach(var v in doorNodes)
        {
            if (Vector2.Distance(v.Value.Position, destination) < Vector2.Distance(destinationNode.Position, destination))
                destinationNode = v.Value;
        }

        Debug.Log("dest: " + destinationNode.GameObject.name);

        Queue<ANode> frontier = new Queue<ANode>();
        HashSet<Node> visited = new HashSet<Node>();
        List<Node> result = new List<Node>();
        List<ANode> availableActions = new List<ANode>();
        AddAvailableActions(new ANode(startingNode, null, 0f), availableActions, visited, destination);
        /*
        frontier.Enqueue(new ANode(startingNode, null));
        visited.Add(startingNode);

        while(frontier.Count > 0)
        {
            ANode current = frontier.Dequeue();


            var next = current.Node.Neighbours.FindAll(c => !visited.Contains(c)).OrderBy(c => Vector2.Distance(current.Node.Position, c.Position) + Vector2.Distance(destinationNode.Position, c.Position)).ElementAt(0);
            if (next == destinationNode)
            {
                ANode temp = new ANode(next, current);
                while (temp.Parent != null)
                {
                    result.Add(temp.Node);
                    temp = temp.Parent;
                }
                return result;
            }
            else if(next != null)
            {
                frontier.Enqueue(new ANode(next, current));
                visited.Add(next);
            }
        }
        */

        while(availableActions.Count > 0)
        {
            ANode current = NextNode(availableActions, visited, destination);
            if(current.Node == destinationNode)
            {
                while (current.Parent != null)
                {
                    result.Add(current.Node);
                    current = current.Parent;
                }
                return result;
            }
            else
            {
                AddAvailableActions(current, availableActions, visited, destination);
            }
        }
        return result;
    }

    ANode NextNode(List<ANode> availableActions, HashSet<Node> visited, Vector3 destination)
    {
        ANode current = availableActions[0];
        foreach(var v in availableActions)
        {
            if (v.Distance + Vector2.Distance(v.Node.Position, destination) < current.Distance + Vector2.Distance(current.Node.Position, destination))
                current = v;
        }
        availableActions.Remove(current);
        visited.Add(current.Node);
        return current;
    }

    void AddAvailableActions(ANode node, List<ANode> availableActions, HashSet<Node> visited, Vector3 destination)
    {
        foreach(var v in node.Node.Neighbours)
        {
            if(!visited.Contains(v))
                availableActions.Add(new ANode(v, node, node.Distance + Vector2.Distance(v.Position, destination)));
        }
    }

    public Node FindClosestNode(Vector2 position)
    {
        Node current = nodes[0];
        foreach(var v in nodes)
        {
            if (Vector2.Distance(v.Position, position) < Vector2.Distance(current.Position, position))
                current = v;
        }
        foreach (var v in doorNodes)
        {
            if (Vector2.Distance(v.Value.Position, position) < Vector2.Distance(current.Position, position))
                current = v.Value;
        }
        return current;
    }
}
