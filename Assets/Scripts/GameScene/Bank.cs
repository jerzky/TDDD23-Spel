using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Bank : Building
{
    public static Bank Instance;
    private readonly List<NodePath> _nodePaths = new List<NodePath>();
    [SerializeField] 
    private GameObject _nodeHolder;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        var guard = FindObjectOfType<AI>();
        LoadPathingNodes();
        _nodePaths[1].Guard = guard;
        guard.SetRoute(_nodePaths[1]);
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void LoadPathingNodes()
    {

        var rooms = _nodeHolder.GetComponentsInChildren<Transform>();
        foreach (var r in rooms)
        {
            if (r.childCount == 0 || r.name == _nodeHolder.name)
                continue;
            var nodePath = new NodePath(r.name, null, new List<NodePath.RouteNode>());
            var nodes = r.GetComponentsInChildren<Transform>();

            foreach (var node in nodes.OrderBy(c => c.name))
            {
                if (node == r)
                    continue;
                nodePath.Nodes.Add(ParseNodeName(node.name, node.transform.position, r.name));
            }
            _nodePaths.Add(nodePath);
        }

        foreach (var path in _nodePaths)
        {
            Debug.Log($"Name: {path.Name}, Count: {path.Nodes.Count}");
        }
    }

    private static NodePath.RouteNode ParseNodeName(string name, Vector2 pos, string parentName = "")
    {
        var firstDashIndex = name.IndexOf('-') + 1;
        var secondDash = name.IndexOf('-', firstDashIndex);
        if (secondDash == -1)
        {
            Debug.Log(string.IsNullOrEmpty(parentName)
                ? "Added walk node"
                : $"Added walk node to parent: {parentName}");

            return new NodePath.RouteNode(pos, NodePath.RouteNodeType.Walk);
        }

        var type = int.Parse(name.Substring(firstDashIndex, secondDash - firstDashIndex));

        var enumType = (NodePath.RouteNodeType) type;

        var length = name.Substring(secondDash + 1, name.Length - secondDash - 1);
        var intLength = int.Parse(length);

        Debug.Log(string.IsNullOrEmpty(parentName)
            ? $"Added a {enumType} with length: {intLength}"
            : $"Added a {enumType} with length: {intLength} to parent: {parentName}");
      
        return new NodePath.RouteNode(pos, enumType, intLength);
    }



    public override void OnAlert(AlertType alertType, Vector2 pos)
    {
        Debug.Log("CCTV OnAlert");
        switch(alertType)
        {
            case AlertType.Investigate:
                SendGuardToInvestigate(pos);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(alertType), alertType, null);
        }


        base.OnAlert(alertType, pos);
    }

    private void SendGuardToInvestigate(Vector2 pos)
    {

    }

}
