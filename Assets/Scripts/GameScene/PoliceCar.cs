
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    public class PoliceCar : MonoBehaviour
    {
        private const int MAX_SPOTS = 4;
        public Vector2 CarDirection { get; set; }
        public Building Building { get; set; }
        private const float CAR_SPEED = 10f;

        [SerializeField] 
        public GameObject Holder;

        private  readonly List<GameObject> _policeOnCar = new List<GameObject>();

        public bool PoliceHasSpawned { get; private set; }



        public List<Police> MyPolice { get; private set; } = new List<Police>();
        public bool AllPoliceAreDead => MyPolice.Count == 0;


        void Start()
        {
            _policeOnCar.AddRange(Holder.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
            SetPoliceOnTruck(true, MAX_SPOTS);
        }

        void FixedUpdate()
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            if (Vector2.Distance(transform.position, Building.PoliceSpawnPoint) > 0.25f ||
                (AllPoliceAreDead && PoliceHasSpawned))
            {
                GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) +
                                                         CarDirection.normalized * Time.deltaTime * CAR_SPEED);
                

                   

            if (Vector2.Distance(transform.position, Building.PoliceSpawnPoint) > 150 && AllPoliceAreDead)
                {
                    Destroy(gameObject);
                }

                return;
            }

            if (!PoliceHasSpawned)
            {
                SpawnPolice();
                PoliceHasSpawned = true;
            }
        }







        private void SpawnPolice(bool debug = false)
        {

            const float xOff = 1f;
            const float yOff = 2f;
            var origin = (Vector2) (transform.position);
            for (var i = 0; i < MAX_SPOTS; i++)
            {
                Police p;
                if (Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal)
                {
                    p = Police.Generate(
                        origin + new Vector2(xOff * (i % (MAX_SPOTS / 2) - 3), (i >= (MAX_SPOTS / 2) ? -1 : 1) * yOff),
                        State.GotoCoverEntrance,
                        ActionE.GotoCoverEntrance, Building).GetComponent<Police>();


                }
                else
                {
                    p = Police.Generate(
                        origin + new Vector2((i >= (MAX_SPOTS / 2) ? -1 : 1) * yOff, xOff * (i % (MAX_SPOTS / 2) - 3)),
                        State.GotoCoverEntrance,
                        ActionE.GotoCoverEntrance, Building).GetComponent<Police>();
                }

                p.MySpawnPoint = p.transform.position;
                p.Car = this;
                MyPolice.Add(p);

            }

            var outerUpper = Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal ? 6 : 3;
            var outerLower = Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal ? -3 : 0;

            var innerUpper = Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal ? 2 : 5;
            var innerLower = Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal ? -1 : -4;

            for (var i = outerLower; i < outerUpper; i++)
            {
                for (var j = innerLower; j < innerUpper; j++)
                {
                    var origin2 = (Vector2) transform.position + new Vector2(i, j);
                    PathingController.Instance.UpdateGrid(origin2, NodeType.Blocked);

                    if (!debug)
                        continue;

                    var go = new GameObject();
                    go.transform.position =
                        new Vector3(Mathf.FloorToInt(origin2.x), Mathf.FloorToInt(origin2.y), 0);
                    go.AddComponent<SpriteRenderer>().sprite =
                        Resources.LoadAll<Sprite>("Textures/x64spritesheet")[10];
                }
            }




            SetPoliceOnTruck(false, MAX_SPOTS);
 
        }

        private void SetPoliceOnTruck(bool value, int count)
        {
            for (var i = 0; i < _policeOnCar.Count; i++)
            {
                _policeOnCar[i].SetActive(i <= count ? value : !value);
            }
        }

        public void ReportWaiting()
        {
            if (MyPolice.Where(p => p.CurrentAction == ActionE.WaitingForAllPolice).ToList().Count != MyPolice.Count)
                return;

            SetPoliceOnTruck(true, MyPolice.Count);
            foreach (var police in MyPolice.ToList())
            {
                police.OnDeath();
                Destroy(police.gameObject);
            }
            MyPolice.Clear();
        }



    private static GameObject _standardHorPrefab;
        private static GameObject _standardVertPrefab;

        public static GameObject Generate(Building building)
        {
            Debug.Log("Spawning police car");
            if (_standardVertPrefab == null || _standardHorPrefab == null)
            {
                _standardVertPrefab = Resources.Load<GameObject>("Prefabs/PoliceCarVert");
                _standardHorPrefab = Resources.Load<GameObject>("Prefabs/PoliceCarHor");
            }

            var prefab = building.PoliceCarAlignment == PoliceCarAlignment.Horizontal
                ? _standardHorPrefab
                : _standardVertPrefab;

            var car = Instantiate(prefab, building.PoliceCarSpawnPoint, prefab.transform.rotation,
                WeaponController.Instance.bulletHolder.transform);

            car.GetComponent<PoliceCar>().CarDirection = building.PoliceSpawnPoint - building.PoliceCarSpawnPoint;
            car.GetComponent<PoliceCar>().Building = building;
            return car;
        }

    }
