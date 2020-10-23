using System;
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
    protected void Start()
    {
        var sizeX = 129 - 50;
        var sizeY = 67 - 33;
        var posX = 50 + sizeX / 2;
        var posY = 33 + sizeY / 2;

        BuildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));
        NodePath nodePath = NodePath.LoadPathNodesFromHolder(nodeHolder);
        apartments.Add(new Apartment(nodePath, new Vector2(57, 60)));

        float xInc = 15;
        float xIncOver2 = 4;
        float yIncOver4 = -17;

        for(int i = 1; i < numberOfApartments; i++)
        {
            Vector2 offset = Vector2.zero;
            offset.x = ((i > 2 && i < 5) || i > 7) ? xInc * (i % 5) + xIncOver2 : xInc * (i % 5);
            offset.y = i > 4 ? yIncOver4 : 0f;
            NodePath newPath = new NodePath("Apartment" + i, null);
            for(int j = 0; j < nodePath.Nodes.Count; j++)
            {
                var v = nodePath.Nodes[j];
                newPath.Nodes.Add(new NodePath.RouteNode(v.Position + offset, v.Type, v.IdleTime));
            }
            apartments.Add(new Apartment(newPath, apartments[0].Position + offset));
        }

        BuildingType = BuildingType.Apartment;
        Transform civParent = new GameObject("CivParent").transform;
        GenerateEntrances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override NodePath GetCivilianPath(AI ai)
    {
        Apartment a = apartments.Find(c => c.Resident != null && c.Resident.gameObject.GetInstanceID() == ai.gameObject.GetInstanceID());
        if (a == default(Apartment))
        {
            a = apartments.Find(c => c.Resident == null);
            a.Resident = ai;
        }

        return a.ApartmentRotation;

        /*foreach (var v in apartments)
        {
            if (v.Resident == null)
            {
                v.Resident = ai;
                return v.ApartmentRotation;
            }
        }
        return null;*/
    }

    public Apartment GetApartment(Vector2 pos)
    {
        foreach (var v in apartments)
            if (v.IsWithin(pos))
                return v;
        return null;
    }
}
