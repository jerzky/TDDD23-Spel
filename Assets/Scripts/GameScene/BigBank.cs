
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

            if (allGuards.Length > 1)
            {
                foreach (var guard in allGuards)
                {
                    _guards.Add(guard);
                }

                _nodePaths[0].Guard = _guards[0];

                _guards[0].SetRoute(_nodePaths[0]);
            }
        }
    }

