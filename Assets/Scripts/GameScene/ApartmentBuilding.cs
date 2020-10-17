using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Apartment
{
    public AI Resident = null;
    public NodePath ApartmentRotation;
    public Vector2 Position;
    private Vector2 size = new Vector2(15f, 13f);
    public Apartment(NodePath nodePath, Vector2 position)
    {
        ApartmentRotation = nodePath;
        Position = position;
    }

    public bool IsWithin(Vector2 pos)
    {
        if (pos.x > Position.x + size.x / 2 || pos.x < Position.x - size.x / 2)
            return false;
        if (pos.y > Position.y + size.y / 2 || pos.y < Position.y - size.y / 2)
            return false;
        return true;
    }
}

public class ApartmentBuilding : Building
{
    private int numberOfApartments = 10;
    [SerializeField]
    GameObject nodeHolder;
    List<Apartment> apartments = new List<Apartment>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        var sizeX = 124 - 45;
        var sizeY = 67 - 33;
        var posX = 45 + sizeX / 2;
        var posY = 33 + sizeY / 2;

        _buildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));
        NodePath nodePath = NodePath.LoadPathNodesFromHolder(nodeHolder);
        apartments.Add(new Apartment(nodePath, new Vector2(52, 60)));

        float xInc = 15;
        float xIncOver3 = 4;
        float yIncOver4 = -17;

        for(int i = 0; i < numberOfApartments; i++)
        {
            Vector2 offset = Vector2.zero;
            offset.x = ((i > 3 && i < 5) || i > 7) ? xInc * i + xIncOver3 : xInc * i;
            offset.y = i > 5 ? yIncOver4 : 0f;
            NodePath newPath = new NodePath("Apartment" + i, null);
            for(int j = 0; j < nodePath.Nodes.Count; j++)
            {
                var v = nodePath.Nodes[j];
                newPath.Nodes.Add(new NodePath.RouteNode(v.Position + offset, v.Type, v.IdleTime));
            }
            apartments.Add(new Apartment(newPath, apartments[0].Position + offset));
        }

        BuildingType = BuildingType.Appartment;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override NodePath GetCivilianPath(AI ai)
    {
        foreach (var v in apartments)
        {
            if (v.Resident == null)
            {
                v.Resident = ai;
                return v.ApartmentRotation;
            }
        }
        return null;
    }

    public Apartment GetApartment(Vector2 pos)
    {
        foreach (var v in apartments)
            if (v.IsWithin(pos))
                return v;
        return apartments[(int)Random.Range(0, apartments.Count-1)];
    }
}
