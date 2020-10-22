using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class NodePath
{
    public enum RouteNodeType : uint
    {
        Walk = 0,
        Idle = 1,
        Interact = 2
    }
    public class RouteNode
    {
        public class NodeFunctionInputValue
        {
            int inputInt;
        }
        public Vector2 Position { get; }
        public RouteNodeType Type { get; }
        public int IdleTime { get; }

        public System.Action CallOnAchieved { get; set; }

        public RouteNode(Vector2 position, RouteNodeType type,  int idleTime = 0)
        {
            Type = type;
            Position = position;
            IdleTime = idleTime;
            CallOnAchieved = null;
        }
    }
    public List<RouteNode> Nodes { get; }

    private int _currentNodeIndex;

    public string Name { get; }
    public AI Guard { get;  set; }

    public NodePath(string name, AI guard, params RouteNode[] nodes)
    {
        _currentNodeIndex = 0;
        Name = name;
        Guard = guard;
        Nodes = new List<RouteNode>(nodes);
    }
    public NodePath(string name, AI guard, List<RouteNode> nodes)
    {
        _currentNodeIndex = 0;
        Name = name;
        Guard = guard;
        Nodes = nodes;
    }

    public int CurrentNodeIndex
    {
        get => _currentNodeIndex;
        private set => _currentNodeIndex = value >= Nodes.Count ? 0 : value;
    }

    public RouteNode CurrentNode => Nodes[CurrentNodeIndex];
    public RouteNode NextNode
    {
        get
        {
            ++CurrentNodeIndex;
            return Nodes[CurrentNodeIndex];
        }
    }

    public static NodePath LoadPathNodesFromHolder(GameObject holder)
    {
        var nodePath = new NodePath(holder.name, null, new List<NodePath.RouteNode>());
        var nodes = holder.GetComponentsInChildren<Transform>().ToList();
        foreach (var node in nodes.OrderBy(c => c.name))
        {
            if (node.gameObject.GetInstanceID() == holder.GetInstanceID())
                continue;
            nodePath.Nodes.Add(ParseNodeName(node.name, node.transform.position, holder.name));
        }
        return nodePath;
    }

    private static NodePath.RouteNode ParseNodeName(string name, Vector2 pos, string parentName = "")
    {
        var firstDashIndex = name.IndexOf('-') + 1;
        var secondDash = name.IndexOf('-', firstDashIndex);
        if (secondDash == -1)
        {
           /* Debug.Log(string.IsNullOrEmpty(parentName)
                ? "Added walk node"
                : $"Added walk node to parent: {parentName}");*/

            return new NodePath.RouteNode(pos, NodePath.RouteNodeType.Walk);
        }

        var type = int.Parse(name.Substring(firstDashIndex, secondDash - firstDashIndex));

        var enumType = (NodePath.RouteNodeType)type;
        var length = name.Substring(secondDash + 1, name.Length - secondDash - 1);
        var intLength = int.Parse(length);

        /*Debug.Log(string.IsNullOrEmpty(parentName)
            ? $"Added a {enumType} with length: {intLength} at position {pos}"
            : $"Added a {enumType} with length: {intLength} at position {pos} to parent: {parentName}");*/
        return new NodePath.RouteNode(pos, enumType, intLength);
    }
}