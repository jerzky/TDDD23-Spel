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
    public static GameController Instance;
    

    private readonly SimpleTimer _timer = new SimpleTimer(2f);

    public bool _gameHasSave = false;
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;

        var cams = FindObjectsOfType<Camera>();
        LoadSaveInfo lsi = FindObjectOfType<LoadSaveInfo>();
        if (lsi != null)
        {
            _gameHasSave = true;
            GeneralUI.Instance.Credits = lsi.LoadedGame.Credits;
            GeneralUI.Instance.Kills = lsi.LoadedGame.Kills;
            Destroy(cams.Where(c => c.transform.parent == null).ToList()[0].gameObject);
            if(lsi.LoadedGame.Items != null)
                foreach (var v in lsi.LoadedGame.Items)
                    Inventory.Instance.AddItem(v.Item1, v.Item2);
        }
        
    }



    // Update is called once per frame
    private void Update()
    {
        if (GeneralUI.Instance.NearbyToInfo.Count > 0)
            TriggerInteractableText();
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
