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
    SortedDictionary<uint, string> ItemToInfo = new SortedDictionary<uint, string>();
    SortedDictionary<string, string> InteractableToInfo = new SortedDictionary<string, string>();

    float timer = 2f;
    float maxTimer = 2f;
    string guardText;

    bool gameHasSave = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        var cams = FindObjectsOfType<Camera>();
        LoadSaveInfo lsi = FindObjectOfType<LoadSaveInfo>();
        if (lsi != null)
        {
            gameHasSave = true;
            GeneralUI.Instance.Credits = lsi.LoadedGame.Credits;
            GeneralUI.Instance.Kills = lsi.LoadedGame.Kills;
            Destroy(cams.Where(c => c.transform.parent == null).ToList()[0].gameObject);
            Debug.Log("FOUND LSI");
            if(lsi.LoadedGame.Items != null)
            {
                Debug.Log("ITEMS NOT NULL");
                foreach (var v in lsi.LoadedGame.Items)
                {
                    Debug.Log("ADD ITEM");
                    Inventory.Instance.AddItem(v.Item1, v.Item2);
                }
            }
        }

        SortedDictionary<ControlAction, ControlInfo> atk = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
        ItemToInfo.Add(ItemList.ITEM_LOCKPICK.UID, "The lockpick can be used on locked interactables, such as doors or cabinets, by pressing the use item key(" + atk[ControlAction.UseItem].KeyCode1.ToString() + ").");
        ItemToInfo.Add(ItemList.ITEM_DRILL.UID, "The drill is used to open locked interactables, similar to the lockpick, however, its main use is to open the vault doors in the bank. Use it by pressing the use item key(" + atk[ControlAction.UseItem].KeyCode1.ToString() + ").");
        ItemToInfo.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID, "The C4 is a highpowered explosive which can break down pretty much anything on the map. Use it by pressing the use item key(" + atk[ControlAction.UseItem].KeyCode1.ToString() + ").");
        ItemToInfo.Add(ItemList.ITEM_SLEDGEHAMMER.UID, "This can be used to break walls, so you can enter a building from a different direction. Unless you want to simply hit the guards with it. It is used as a weapon by pressing the Shoot key(" + atk[ControlAction.Shoot].KeyCode1.ToString() + ").");
        ItemToInfo.Add(ItemList.ITEM_PISTOL.UID, "Weapons can be used to control civilians or fight guards and police, simply point the gun at a civilian that has noticed you to make the civilian freeze.");
        ItemToInfo.Add(ItemList.ITEM_AK47.UID, "Weapons can be used to control civilians or fight guards and police, simply point the gun at a civilian that has noticed you to make the civilian freeze.");
        ItemToInfo.Add(ItemList.ITEM_SILENCED_PISTOL.UID, "Weapons can be used to control civilians or fight guards and police, simply point the gun at a civilian that has noticed you to make the civilian freeze.");
        ItemToInfo.Add(ItemList.ITEM_MAC10.UID, "Weapons can be used to control civilians or fight guards and police, simply point the gun at a civilian that has noticed you to make the civilian freeze.");

        InteractableToInfo.Add("Door", "Doors are interactable, simply press your interactable key(" + atk[ControlAction.Interact].KeyCode1.ToString() + ") to open it.");
        InteractableToInfo.Add("Searchable Container", "Searchable Containers are interactable, they come in different shapes and sizes, for instance a searchable container could be a desk or cabinet, simply press your interactable key(" + atk[ControlAction.Interact].KeyCode1.ToString() + ") to open it. If its locked, maybe you should consider buying a lockpick.");
        InteractableToInfo.Add("Store", "The store can be used to buy the items you need to complete your heists or cause mayhem, whatever, you prefer, simply press your interactable key(" + atk[ControlAction.Interact].KeyCode1.ToString() + ") to open it.");
        InteractableToInfo.Add("Drill", "The drill interactable is spawned when you use your drill item, you can leave it to work on its own, unlike the lockpick, if you want to cancel it and pick it up again, simply use your interact key("+ atk[ControlAction.Interact].KeyCode1.ToString() + ").");
        InteractableToInfo.Add("Cash Register", "The Cash register is a interactable that can be found in a few different locations, such as the bar or gas station, but the most lucrative cashregister is found inside the lobby of the bank. Holding a weapon will trigger the armed robbery mode, if you're not holding a weapon you will try to rob it in stealth mode. Interact key: " + atk[ControlAction.Interact].KeyCode1.ToString());
        InteractableToInfo.Add("CCTV", "The CCTV will alert guards if you walk into its vision, however, you will only be noticed if a guard is guarding the security station.");
        InteractableToInfo.Add("Security Station", "The security station controls the cameras, if there is no guard stationed at it you can freely move through the cameras' vision.");
        guardText = "This is a guard, if you dont have a weapon use your takedown key(" + atk[ControlAction.TakeDown].KeyCode1.ToString() + ") while sneaking up behind him to knock him out.";

    }

    // Update is called once per frame
    void Update()
    {
        if (InteractableToInfo.Count > 0)
            TriggerInteractableText();
    }

    public void TriggerInteractableText()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = maxTimer;
            var colliders = Physics2D.OverlapCircleAll(PlayerController.Instance.transform.position, 10f);
            foreach (var v in colliders)
            {
                var inter = v.GetComponent<Interactable>();
                if (inter != null && InteractableToInfo.ContainsKey(inter.Name()))
                {
                    Sprite sprite = inter.GetComponent<SpriteRenderer>().sprite;
                    if (inter.Name() == "Cash Register")
                        sprite = Resources.LoadAll<Sprite>("Textures/x64spritesheet")[38];
                    GeneralUI.Instance.TriggerInfoText(InteractableToInfo[inter.Name()], sprite);
                    InteractableToInfo.Remove(inter.Name());
                }
                var ai = v.GetComponent<AI>();
                if(ai != null)
                {
                    if(ai.Type() == AI_Type.Guard)
                    {
                        if(guardText != "")
                            GeneralUI.Instance.TriggerInfoText(guardText, ai.GetComponent<SpriteRenderer>().sprite);
                        guardText = "";
                    }
                    else if (ai.Type() == AI_Type.Civilian)
                    {
                        //BLABLA
                    }

                }
            }
        }
    }

    public void TriggerItemText(uint UID)
    {
        if (ItemToInfo.Count <= 0)
            return;

        if (ItemToInfo.ContainsKey(UID))
        {
            GeneralUI.Instance.TriggerInfoText(ItemToInfo[UID], Resources.Load<Sprite>(ItemList.AllItems[UID].IconPath));
            ItemToInfo.Remove(UID);

            // If it is a weapon remove all weapons
            foreach(var v in ItemToInfo.ToList().Where(c => ItemList.AllItems[c.Key].ItemType == ItemType.Weapon))
                ItemToInfo.Remove(v.Key);
        }
    }

    public void SaveGame()
    {
        string name;
        if (!gameHasSave)
            name = CreateNewSave();
        else
            name = SaveOverFirstSlot();

        gameHasSave = true;
        GeneralUI.Instance.TriggerInfoText("Game has been saved with name: " + name + "\n If you would like to change the name of the save, you can do this in the main menu");
    }

    string CreateNewSave()
    {
        
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGamesHandler.SavedGame>>("saves.json");
        var clone = savedGames.ToList();
        savedGames.Clear();
        savedGames.Add(0, new SavedGamesHandler.SavedGame(DateTime.Now.ToString(), GeneralUI.Instance.Credits, GeneralUI.Instance.Kills, Inventory.Instance.InventoryToList()));
        foreach (var v in clone)
            savedGames.Add(v.Key + 1, v.Value);
        Json.SaveToJson(savedGames, "saves.json");
        return savedGames[0].Name;
    }

    string SaveOverFirstSlot()
    {
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGamesHandler.SavedGame>>("saves.json");
        savedGames[0].Kills = GeneralUI.Instance.Kills;
        savedGames[0].Credits = GeneralUI.Instance.Credits;
        savedGames[0].Items = Inventory.Instance.InventoryToList();
        Json.SaveToJson(savedGames, "saves.json");
        return savedGames[0].Name;
    }

    void OnApplicationQuit()
    {
        if (!gameHasSave)
            return;
        SaveOverFirstSlot();
    }
}
