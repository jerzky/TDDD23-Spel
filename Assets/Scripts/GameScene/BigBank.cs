
    using UnityEngine;

    public class BigBank : Bank
    {
        protected new void Start()
        {
            BuildingType = BuildingType.BigBank;
            PoliceController.AddBuilding(this);
            LoadPathingNodes();

            var bottomleft = new Vector2(50, 77);
            var topright = new Vector2(129, 109);


            BuildingParts.Add(new BuildingPart(topright - bottomleft / 2, topright - bottomleft));
            SetUpCivilianRoutes();
            GenerateEntrances();


            var allGuards = GetComponentsInChildren<AI>();
            var i = 0;
            foreach (var guard in allGuards)
            {
                _guards.Add(guard);
                _guards.Add(guard);
                _nodePaths[i].Building = this;
                guard.SetRoute(_nodePaths[i++]);
                Debug.Log("Gave a guard a path");
            }


        }
    }

