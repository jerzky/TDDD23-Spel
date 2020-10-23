
    using UnityEngine;

    public class BigBank : Bank
    {
        protected void Start()
        {
            BuildingType = BuildingType.BigBank;
            PoliceController.AddBuilding(this);
            LoadPathingNodes();

            var bottomleft = new Vector2(50, 77);
            var topright = new Vector2(129, 109);


            BuildingParts.Add(new BuildingPart(bottomleft, topright - bottomleft));
            PoliceController.AddBuilding(this);
            SetUpCivilianRoutes();
            GenerateEntrances();
        }
    }

