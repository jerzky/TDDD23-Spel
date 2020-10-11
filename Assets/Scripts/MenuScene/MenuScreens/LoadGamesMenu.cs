using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class SavedGame
{
    public string Name { get; set; }
    public int Credits { get; set; }
    public int Kills { get; set; }
    public List<Tuple<uint, uint>> Items;
    public SortedDictionary<uint, string> ItemToInfo = new SortedDictionary<uint, string>();
    public SortedDictionary<string, string> NearbyToInfo = new SortedDictionary<string, string>();
    public SavedGame(string name, int credits, int kills, List<Tuple<uint, uint>> items, SortedDictionary<uint, string>  itemToInfo, SortedDictionary<string, string> nearbyToInfo)
    {
        Name = name;
        Credits = credits;
        Kills = kills;
        Items = items;
        NearbyToInfo = nearbyToInfo;
        ItemToInfo = itemToInfo;
    }
}

public class LoadGamesMenu : MenuScreen
{
    GameObject textPrefab;
    SortedDictionary<uint, SavedGame> savedGamesMap;
    List<Text> savedGameTexts;
    Vector3 textOffset = new Vector3(0f, -100f, 0f);
    bool choosingName = false;
    public LoadGamesMenu (GameObject holder, Screen screen, GameObject textPrefab) : base(holder, screen)
    {
        this.textPrefab = textPrefab;
        savedGamesMap = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        CreateSavedGameTexts();
        savedGameTexts[0].color = Color.magenta;
        limit.x = 0;
        limit.y = savedGameTexts.Count-1;

        monitoredKeys = new List<KeyCode> { KeyCode.Return, KeyCode.LeftControl, KeyCode.Escape, KeyCode.Delete };
    }

    // counter solves weird problem that keys are pressed twice
    int onguicounter = 0;
    public override void OnGUI()
    {
        if (onguicounter++ % 2 == 0)
            return;
        if (!choosingName)
            return;

        KeyCode key = Event.current.keyCode;
        // input getkeydown reduces the times keys are recorded to 2
        if (key == KeyCode.None || !Input.GetKeyDown(key))
            return;

        if(key == KeyCode.Return)
        {
            savedGamesMap[(uint)position.y].Name = savedGameTexts[(int)position.y].text;
            choosingName = false;
            SkipFrame = true;
            savedGameTexts[(int)position.y].color = Color.magenta;
            return;
        }

        if (key == KeyCode.Escape)
        {
            savedGameTexts[(int)position.y].text = savedGamesMap[(uint)position.y].Name;
            choosingName = false;
            SkipFrame = true;
            savedGameTexts[(int)position.y].color = Color.magenta;

            return;
        }

        if(Input.GetKeyDown(key) && (uint)key >= (uint)KeyCode.A && (uint)key <= (uint)KeyCode.Z)
            savedGameTexts[(int)position.y].text += key.ToString();
    }

    void CreateSavedGameTexts()
    {
        int i = 0;
        savedGameTexts = new List<Text>();
        foreach (var v in savedGamesMap)
        {
            GameObject text = GameObject.Instantiate(textPrefab, holder.transform.position + ++i * textOffset - new Vector3(175f, 0f, 0f), Quaternion.identity, holder.transform);

            Text temp = text.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = v.Value.Name;
            temp.color = Color.black;
            temp.fontSize = 50;
            savedGameTexts.Add(temp);
        }
    }

    public override void DeActivate()
    {
        Json.SaveToJson(savedGamesMap, "saves.json");
        base.DeActivate();
    }

    public override void Move(Vector2 dir)
    {
        if (choosingName)
            return;
        base.Move(dir);
    }

    protected override void ActivateMenuObject(Vector2 pos)
    {
        savedGameTexts[(int)pos.y].color = Color.magenta;
    }

    protected override void DeactivateMenuObject(Vector2 pos)
    {
        savedGameTexts[(int)pos.y].color = Color.black;
    }

    public override void KeyPressed(KeyCode key)
    {
        if (choosingName)
            return;
        switch (key)
        {
            case KeyCode.Return:
                // SetLoadedSave on First position
                var clone = savedGamesMap.ToList().Where(c => c.Key != (uint)position.y);
                var chosenSave = savedGamesMap[(uint)position.y];
                savedGamesMap.Clear();
                savedGamesMap.Add(0, chosenSave);
                uint i = 0;
                foreach (var v in clone)
                    savedGamesMap.Add(++i, v.Value);
                Json.SaveToJson(savedGamesMap, "saves.json");
                MenuController.Instance.StartGameAnimation("LoadSaveScene");
                break;

            case KeyCode.LeftControl:
                savedGameTexts[(int)position.y].color = Color.green;
                savedGameTexts[(int)position.y].text = "";
                choosingName = true;
                break;

            case KeyCode.Escape:
                MenuController.Instance.ChangeMenuScreen(Screen.MainMenu);
                break;
            case KeyCode.Delete:
                GameObject.Destroy(savedGameTexts[(int)position.y].gameObject);
                savedGameTexts.RemoveAt((int)position.y);
                savedGamesMap.Remove((uint)position.y);
                for (i = (uint)position.y; i < savedGameTexts.Count; i++)
                {
                    savedGameTexts[(int)i].transform.position -= textOffset;
                }

                if (savedGameTexts.Count == 0)
                    MenuController.Instance.ChangeMenuScreen(Screen.MainMenu);

                Vector2 lastPos = position;
                if ((int)position.y >= savedGameTexts.Count)
                    position.y--;

                if(savedGameTexts.Count > 0)
                    ActivateMenuObject(position);
                limit.y--;
                break;
        }
    }
}
