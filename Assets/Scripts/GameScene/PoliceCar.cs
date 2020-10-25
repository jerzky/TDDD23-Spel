    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

public class PoliceCar : MonoBehaviour
{
    public int PoliceCount { get; set; }
    public Vector2 CarDirection { get; set; }
    public Building Building { get; set; }
    private const float CAR_SPEED = 10f;

    [SerializeField] public GameObject Holder;

    private readonly List<GameObject> _policeOnCar = new List<GameObject>();
    private readonly List<Vector2> _blockedPositions = new List<Vector2>();
    private readonly List<GameObject> _blockedGameObjects = new List<GameObject>();
    public bool PoliceHasSpawned { get; private set; }

    private bool _dontRemoveGrid;

    public List<Police> MyPolice { get; private set; } = new List<Police>();
    public bool AllPoliceAreDead => MyPolice.Count == 0;


    void Start()
    {
        _policeOnCar.AddRange(Holder.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
        SetPoliceOnTruck(true, PoliceCount);
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if (AllPoliceAreDead && PoliceHasSpawned)
        {
            PoliceController.Instance.NotifyPolice(Building);
        }

        if (Vector2.Distance(transform.position, Building.PoliceSpawnPoint) > 0.25f ||
            (AllPoliceAreDead && PoliceHasSpawned))
        {
            GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y) +
                                                     CarDirection.normalized * Time.deltaTime * CAR_SPEED);




            if (Vector2.Distance(transform.position, Building.PoliceSpawnPoint) > 150 && AllPoliceAreDead)
            {
                Destroy(gameObject);

                foreach (var o in _blockedGameObjects)
                {
                    Destroy(o);
                }

                if (_dontRemoveGrid)
                    return;

                foreach (var position in _blockedPositions)
                {
                    PathingController.Instance.UpdateGrid(position, NodeType.Clear);
                }
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

        var origin = (Vector2)(transform.position);
        for (var i = 0; i < PoliceCount; i++)
        {
            Police p;
            if (Building.PoliceCarAlignment == PoliceCarAlignment.Horizontal)
            {
                p = Police.Generate(
                    origin + new Vector2(3f * (i % (PoliceCount / 2) - 1.5f),
                        (i >= (PoliceCount / 2) ? -1 : 1) * 2),
                    State.GotoCoverEntrance,
                    ActionE.GotoCoverEntrance, Building).GetComponent<Police>();


            }
            else
            {
                p = Police.Generate(
                    origin + new Vector2((i >= (PoliceCount / 2) ? -1 : 1) * 3f,
                        2f * (i % (PoliceCount / 2) - 1.5f)),
                    State.GotoCoverEntrance,
                    ActionE.GotoCoverEntrance, Building).GetComponent<Police>();
            }

            p.MySpawnPoint = p.transform.position;
            p.Car = this;
            MyPolice.Add(p);

        }

            var bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        for (var y = (int)Mathf.Floor(bounds.min.y); y < (int)Mathf.Ceil(bounds.max.y); y++)
            for (var x = (int) Mathf.Floor(bounds.min.x); x < (int) Mathf.Ceil(bounds.max.x); x++)
            {
                var pos = new Vector2(x, y);
                PathingController.Instance.UpdateGrid(pos, NodeType.Blocked);
                _blockedPositions.Add(pos);

                if (!debug)
                    continue;
                var go = new GameObject();
                go.transform.position = new Vector3(pos.x, pos.y, 0);
                go.AddComponent<SpriteRenderer>().sprite =
                    Resources.LoadAll<Sprite>("Textures/x64spritesheet")[20];
                _blockedGameObjects.Add(go);
            }
        SetPoliceOnTruck(false, PoliceCount);
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


    public void Transfer(PoliceCar car)
    {
        _dontRemoveGrid = true;
        foreach (var police in MyPolice)
        {
            police.Car = car;
        }
        car.MyPolice.AddRange(MyPolice);
        MyPolice.Clear();
    }


    private static GameObject _standardHorPrefab;
    private static GameObject _standardVertPrefab;

    public static GameObject Generate(Building building, int count = 4)
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
        car.GetComponent<PoliceCar>().PoliceCount = count;
        return car;
    }

}
