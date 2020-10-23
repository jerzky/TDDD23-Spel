using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bar : Building
{
    [SerializeField]
    GameObject _civilianNodeHolder;
    private readonly List<NodePath> _civilianNodePaths = new List<NodePath>();
    private int _civilianNodePathIndex = 0;

    // Start is called before the first frame update
    protected void Start()
    {
        var sizeX = 34 - 5;
        var sizeY = 59 - 49;
        var posX = 5 + sizeX / 2;
        var posY = 49 + sizeY / 2;
        BuildingType = BuildingType.Bar;
        BuildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));

        BuildingType = BuildingType.Bar;
        SetUpCivilianRoutes();
        GenerateEntrances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetUpCivilianRoutes()
    {
        var rooms = _civilianNodeHolder.GetComponentsInChildren<Transform>();
        foreach (var r in rooms)
        {
            if (r.childCount == 0 || r.gameObject.GetInstanceID() == _civilianNodeHolder.GetInstanceID())
                continue;

            _civilianNodePaths.Add(NodePath.LoadPathNodesFromHolder(r.gameObject));
        }
       
    }

    public override NodePath GetCivilianPath(AI ai)
    {
        if (_civilianNodePathIndex >= _civilianNodePaths.Count) 
            _civilianNodePathIndex = _civilianNodePathIndex % _civilianNodePaths.Count;

        return _civilianNodePaths[_civilianNodePathIndex++];
    }
}
