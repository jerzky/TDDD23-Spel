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
            public int InputInt { get; set; }
            public RouteNode Self { get; set; }
        }
        public Vector2 Position { get; }
        public RouteNodeType Type { get; }
        public int IdleTime { get; }

        public System.Action<NodeFunctionInputValue> ActionCall { get; set; }
        public NodeFunctionInputValue Input { get; }
        public int SetInput { set { Input.InputInt = value; } }
        public RouteNode(Vector2 position, RouteNodeType type,  int idleTime = 0, Action<NodeFunctionInputValue> act = null)
        {
            Type = type;
            Position = position;
            IdleTime = idleTime;
            ActionCall = act;
            Input = new NodeFunctionInputValue();
            Input.Self = this;
        }

        public void CallOnAchieved()
        {
            ActionCall?.Invoke(Input);
        }
    }
    public List<RouteNode> Nodes { get; }

    private int _currentNodeIndex;

    public string Name { get; }
    public Building Building { get; set; }

    public NodePath(string name, params RouteNode[] nodes)
    {
        _currentNodeIndex = 0;
        Name = name;
        Nodes = new List<RouteNode>(nodes);
    }
    public NodePath(string name, List<RouteNode> nodes)
    {
        _currentNodeIndex = 0;
        Name = name;
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
        var nodePath = new NodePath(holder.name, new List<NodePath.RouteNode>());
        var nodes = holder.GetComponentsInChildren<Transform>().ToList();
        foreach (var node in nodes)
        {
            if (node.gameObject.GetInstanceID() == holder.GetInstanceID())
                continue;
            nodePath.Nodes.Add(ParseNodeName(node.name, node.transform.position, holder.name));
            GameObject.Destroy(node.GetComponent<SpriteRenderer>());
        }
        return nodePath;
    }

    private static NodePath.RouteNode ParseNodeName(string name, Vector2 pos, string parentName = "")
    {
        var firstDashIndex = name.IndexOf('-') + 1;
        var secondDash = name.IndexOf('-', firstDashIndex);
        
        
        Action<NodePath.RouteNode.NodeFunctionInputValue> act = null;
        if(firstDashIndex -1 > 0)
        {
            var actionStr = name.Substring(0, firstDashIndex - 1);
            if (int.TryParse(actionStr, out int actionInt))
                act = GetAction(actionInt);
        }
        
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
        return new NodePath.RouteNode(pos, enumType, intLength, act);
    }

    static Action<NodePath.RouteNode.NodeFunctionInputValue> GetAction(int actionInt)
    {
        switch(actionInt)
        {
            case 10:
                return PlayPoolSound;
            case 11:
                return StartCameras;
            case 12:
                return TurnOffCameras;
        }
        return null;
    }

    static void PlayPoolSound(RouteNode.NodeFunctionInputValue value)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/poolshot"), value.Self.Position);
    }

    static void StartCameras(RouteNode.NodeFunctionInputValue value)
    {
        SecurityStation closest = null;
        foreach (var v in GameObject.FindObjectsOfType<SecurityStation>())
            if (closest == null || Vector2.Distance(v.transform.position, value.Self.Position) < Vector2.Distance(closest.transform.position, value.Self.Position))
                closest = v;

        if (closest == null)
            throw new SystemException("NO SECURITYSTATIONS???");

        closest.IsMonitored = true;
    }

    static void TurnOffCameras(RouteNode.NodeFunctionInputValue value)
    {
        SecurityStation closest = null;
        foreach (var v in GameObject.FindObjectsOfType<SecurityStation>())
            if (closest == null || Vector2.Distance(v.transform.position, value.Self.Position) < Vector2.Distance(closest.transform.position, value.Self.Position))
                closest = v;

        if (closest == null)
            throw new SystemException("NO SECURITYSTATIONS???");

        closest.IsMonitored = false;
    }
}