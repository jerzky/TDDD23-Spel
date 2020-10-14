using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Apartment
{
    public AI Resident = null;
    public NodePath ApartmentRotation;
    public Vector2 position;
    public Apartment(NodePath nodePath, Vector2 position)
    {

    }
}

public class ApartmentBuilding : Building
{
    private const int numberOfApartments = 10;
    [SerializeField]
    GameObject nodeHolder;
    List<Apartment> apartments = new List<Apartment>();
    // Start is called before the first frame update
    void Start()
    {
        var sizeX = 124 - 45;
        var sizeY = 57 - 23;
        var posX = 45 + sizeX / 2;
        var posY = 23 + sizeY / 2;
        _buildingParts.Add(new BuildingPart(new Vector2(posX, posY), new Vector2(sizeX, sizeY)));

        NodePath nodePath = NodePath.LoadPathNodesFromHolder(nodeHolder);
        apartments.Add(new Apartment(nodePath, new Vector2(52, 48)));
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
            apartments.Add(new Apartment(newPath, apartments[0].position + offset));
        }

        BuildingType = BuildingType.Appartment;
        Debug.Log("CIVIVIVIVIVIVIVI APAPAPPAPAPAP PAATATHS " + apartments.Count);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override NodePath GetCivilianPath()
    {
        foreach (var v in apartments)
        {
            if (v.Resident == null) return v.ApartmentRotation;
        }

        return null;
    }
}
