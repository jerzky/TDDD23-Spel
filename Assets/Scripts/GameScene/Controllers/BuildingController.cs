﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance;
    [SerializeField]
    List<Building> _buildings;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Building GetBuilding(Vector2 position)
    {
        foreach (var v in _buildings)
            if (v.IsWithin(position))
                return v;
        return null;
    }

    public NodePath GetCivilianNodePath(BuildingType type, AI ai)
    {
        foreach(var v in _buildings)
        {
            if (v.BuildingType == type)
                return v.GetCivilianPath(ai);
        }
        return null;
    }
}
