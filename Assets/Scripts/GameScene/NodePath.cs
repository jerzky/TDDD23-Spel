﻿using System.Collections.Generic;
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
        public Vector2 Position { get; }
        public RouteNodeType Type { get; }
        public int IdleTime { get; }
        public RouteNode(Vector2 position, RouteNodeType type,  int idleTime = 0)
        {
            Type = type;
            Position = position;
            IdleTime = idleTime;
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
    public RouteNode NextNode => Nodes[CurrentNodeIndex++];


}