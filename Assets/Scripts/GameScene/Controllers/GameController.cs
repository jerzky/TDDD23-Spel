using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public class RespawnInfo
    {
        public AI_Type AiType;
        public NodePath NodePath;
        public List<GameObject> bodyOrObject;
        public RespawnInfo(AI_Type type, NodePath node, List<GameObject> boo = null)
        {
            AiType = type;
            NodePath = node;
            bodyOrObject = boo;
        }
    }
    public static GameController Instance;
    

    private readonly SimpleTimer _timer = new SimpleTimer(2f);
    private Dictionary<RespawnInfo, SimpleTimer> _respawnQueue = new Dictionary<RespawnInfo, SimpleTimer>();
    public bool _gameHasSave = false;

    public List<GameObject> StackedUpbodies { get; set; } = new List<GameObject>();
    public List<GameObject> StackedUpBreakables { get; set; } = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        LoadSaveInit();

        
    }

    public void AddBody(GameObject g)
    {
        StackedUpbodies.Add(g);
    }

    public void AddBreakable(GameObject g)
    {
        StackedUpBreakables.Add(g);
    }

    public void AddToRespawnQueue(RespawnInfo info)
    {
        float time = 30f;
        if (info.AiType == AI_Type.Guard)
            time = 90f;
        SimpleTimer t = new SimpleTimer(time);
        t.Reset();
        _respawnQueue.Add(info, t);
    }

    void RespawnAI(RespawnInfo info)
    {
        switch(info.AiType)
        {
            case AI_Type.Guard:
                Guard guard = Instantiate(Resources.Load<GameObject>("Prefabs/Guard"), new Vector2(10, 10), Quaternion.identity, null).GetComponent<Guard>();
                (info.NodePath.Building as Bank).AddNewGuard(guard);
                guard.SetRoute(info.NodePath);
                guard.SetPathToPosition(info.NodePath.CurrentNode.Position);
                break;
            case AI_Type.Civilian:
                Apartment apartment = (BuildingController.Instance.Buildings.Find(b => b.BuildingType == BuildingType.Apartment) as ApartmentBuilding).GetEmptyApartment();
                Civilian civ = Instantiate(Resources.Load<GameObject>("Prefabs/Civilian"), apartment.Position, Quaternion.identity, null).GetComponent<Civilian>();
                apartment.Resident = civ;
                break;
            case AI_Type.Medical_Worker:
                Apartment apartment2 = (BuildingController.Instance.Buildings.Find(b => b.BuildingType == BuildingType.Apartment) as ApartmentBuilding).GetEmptyApartment();
                MedicalWorker med = Instantiate(Resources.Load<GameObject>("Prefabs/MedicalWorker"), apartment2.Position, Quaternion.identity, null).GetComponent<MedicalWorker>();
                apartment2.Resident = med;
                break;
            case AI_Type.Construction_Worker:
                Apartment apartment3 = (BuildingController.Instance.Buildings.Find(b => b.BuildingType == BuildingType.Apartment) as ApartmentBuilding).GetEmptyApartment();
                ConstructionWorker con = Instantiate(Resources.Load<GameObject>("Prefabs/ConstructionWorker"), apartment3.Position, Quaternion.identity, null).GetComponent<ConstructionWorker>();
                apartment3.Resident = con;
                break;
        }    
    }

    void LoadSaveInit()
    {
        var cams = FindObjectsOfType<Camera>();
        LoadSaveInfo lsi = FindObjectOfType<LoadSaveInfo>();
        if (lsi != null)
        {
            _gameHasSave = true;
            GeneralUI.Instance.Credits = lsi.LoadedGame.Credits;
            GeneralUI.Instance.Kills = lsi.LoadedGame.Kills;
            Destroy(cams.Where(c => c.transform.parent == null).ToList()[0].gameObject);
            if (lsi.LoadedGame.Items != null)
                foreach (var v in lsi.LoadedGame.Items)
                    Inventory.Instance.AddItem(v.Item1, v.Item2);
        }
    }



    // Update is called once per frame
    private void Update()
    {
        if (GeneralUI.Instance.NearbyToInfo.Count > 0)
            TriggerInteractableText();

        for (int i = _respawnQueue.Count - 1; i > 0; i--)
            if(_respawnQueue.ToList()[i].Value.Tick())
            {
                RespawnAI(_respawnQueue.ToList()[i].Key);
                _respawnQueue.Remove(_respawnQueue.ToList()[i].Key);
            }
    }

    public void TriggerInteractableText()
    {
        if (!_timer.TickAndReset())
            return;
        var colliders = Physics2D.OverlapCircleAll(PlayerController.Instance.transform.position, 10f);
        foreach (var v in colliders)
        {
            GeneralUI.Instance.TriggerNearby(v);
        }
    }

    public void SaveGame()
    {
        var saveName = !_gameHasSave ? CreateNewSave() : SaveOverFirstSlot();
        _gameHasSave = true;
        GeneralUI.Instance.TriggerInfoText(
            $"Game has been saved with name: {saveName}\n If you would like to change the name of the save, you can do this in the main menu");
    }

    private string CreateNewSave()
    {
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        var clone = savedGames.ToList();
        savedGames.Clear();
        savedGames.Add(0, new SavedGame(DateTime.Now.ToString(), GeneralUI.Instance.Credits, GeneralUI.Instance.Kills, Inventory.Instance.InventoryToList(), GeneralUI.Instance.ItemToInfo, GeneralUI.Instance.NearbyToInfo));
        foreach (var v in clone)
            savedGames.Add(v.Key + 1, v.Value);
        Json.SaveToJson(savedGames, "saves.json");
        return savedGames[0].Name;
    }

    private string SaveOverFirstSlot()
    {
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        savedGames[0] = new SavedGame(DateTime.Now.ToString(), GeneralUI.Instance.Credits, GeneralUI.Instance.Kills, Inventory.Instance.InventoryToList(), GeneralUI.Instance.ItemToInfo, GeneralUI.Instance.NearbyToInfo);
        Json.SaveToJson(savedGames, "saves.json");
        return savedGames[0].Name;
    }

    private void OnApplicationQuit()
    {
        if (!_gameHasSave)
            return;
        SaveOverFirstSlot();
    }
}
