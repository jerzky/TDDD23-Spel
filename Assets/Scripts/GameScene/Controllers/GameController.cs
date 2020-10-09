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
    private readonly SortedDictionary<uint, string> _itemToInfo = new SortedDictionary<uint, string>();
    private readonly SortedDictionary<string, string> _nearbyToInfo = new SortedDictionary<string, string>();

    private readonly SimpleTimer _timer = new SimpleTimer(2f);
    private string _guardText;

    private bool _gameHasSave;
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
            if (lsi.LoadedGame.ItemToInfo != null)
                foreach (var v in lsi.LoadedGame.ItemToInfo)
                    _itemToInfo.Add(v.Key, v.Value);
            if (lsi.LoadedGame.NearbyToInfo != null)
                foreach (var v in lsi.LoadedGame.NearbyToInfo)
                    _nearbyToInfo.Add(v.Key, v.Value);
        }
        else
        {
            var keys = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
            _itemToInfo.Add(ItemList.ITEM_LOCKPICK.UID, string.Format(ItemList.ITEM_LOCKPICK.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_DRILL.UID, string.Format(ItemList.ITEM_DRILL.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID,  string.Format(ItemList.ITEM_EXPLOSIVE_TIMED.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_SLEDGEHAMMER.UID, string.Format(ItemList.ITEM_SLEDGEHAMMER.Tooltip, (KeyCode)keys[ControlAction.Shoot].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_PISTOL.UID, ItemList.ITEM_PISTOL.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_AK47.UID, ItemList.ITEM_AK47.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_SILENCED_PISTOL.UID, ItemList.ITEM_SILENCED_PISTOL.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_MAC10.UID, ItemList.ITEM_MAC10.Tooltip);

            _nearbyToInfo.Add("Door", string.Format(Tooltips.DOOR_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Searchable Container", string.Format(Tooltips.SEARCHABLE_CONTAINER_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Store", string.Format(Tooltips.STORE_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Drill", string.Format(Tooltips.DRILL_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Cash Register", string.Format(Tooltips.CASH_REGISTER_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("CCTV", Tooltips.CCTV_TOOLTIP);
            _nearbyToInfo.Add("Security Station", Tooltips.SECURITY_STATION_TOOLTIP);
            _guardText = string.Format(Tooltips.GUARD_TEXT, (KeyCode)keys[ControlAction.TakeDown].KeyCode1);
        }
    }



    // Update is called once per frame
    private void Update()
    {
        if (_nearbyToInfo.Count > 0)
            TriggerInteractableText();
    }

    public void TriggerInteractableText()
    {

        if (!_timer.TickAndReset())
            return;
        var colliders = Physics2D.OverlapCircleAll(PlayerController.Instance.transform.position, 10f);
        foreach (var v in colliders)
        {
            var inter = v.GetComponent<Interactable>();
            if (inter != null && _nearbyToInfo.ContainsKey(inter.Name()))
            {
                Sprite sprite = inter.GetComponent<SpriteRenderer>().sprite;
                if (inter.Name() == "Cash Register")
                    sprite = Resources.LoadAll<Sprite>("Textures/x64spritesheet")[38];
                GeneralUI.Instance.TriggerInfoText(_nearbyToInfo[inter.Name()], sprite);
                _nearbyToInfo.Remove(inter.Name());
            }
            var ai = v.GetComponent<AI>();
            if (ai == null) 
                continue;
            
            if(ai.Type() == AI_Type.Guard)
            {
                if(_guardText != "")
                    GeneralUI.Instance.TriggerInfoText(_guardText, ai.GetComponent<SpriteRenderer>().sprite);
                _guardText = "";
            }
            else if (ai.Type() == AI_Type.Civilian)
            {
                //BLABLA
            }
        }
    }

    public void TriggerItemText(uint UID)
    {
        if (_itemToInfo.Count <= 0)
            return;

        if (!_itemToInfo.ContainsKey(UID)) 
            return;

        GeneralUI.Instance.TriggerInfoText(_itemToInfo[UID], Resources.Load<Sprite>(ItemList.AllItems[UID].IconPath));
        _itemToInfo.Remove(UID);

        // If it is a weapon remove all weapons
        foreach(var v in _itemToInfo.ToList().Where(c => ItemList.AllItems[c.Key].ItemType == ItemType.Weapon))
            _itemToInfo.Remove(v.Key);
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
        savedGames.Add(0, new SavedGame(DateTime.Now.ToString(), GeneralUI.Instance.Credits, GeneralUI.Instance.Kills, Inventory.Instance.InventoryToList(), _itemToInfo, _nearbyToInfo));
        foreach (var v in clone)
            savedGames.Add(v.Key + 1, v.Value);
        Json.SaveToJson(savedGames, "saves.json");
        return savedGames[0].Name;
    }

    private string SaveOverFirstSlot()
    {
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        savedGames[0] = new SavedGame(DateTime.Now.ToString(), GeneralUI.Instance.Credits, GeneralUI.Instance.Kills, Inventory.Instance.InventoryToList(), _itemToInfo, _nearbyToInfo);
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
