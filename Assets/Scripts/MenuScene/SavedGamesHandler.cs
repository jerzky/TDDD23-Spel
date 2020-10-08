using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class SavedGamesHandler : MonoBehaviour
{
    public class SavedGame
    {
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Kills { get; set; }
        public List<Tuple<uint, uint>> Items;
        public SavedGame(string name, int credits, int kills, List<Tuple<uint, uint>> items)
        {
            Name = name;
            Credits = credits;
            Kills = kills;
            Items = items;
        }
    }
    [SerializeField]
    GameObject menuObject;
    [SerializeField]
    GameObject textPrefab;
    public static SavedGamesHandler Instance;
    SortedDictionary<uint, SavedGame> savedGamesMap;
    bool skipFrame = true;
    Text currentRedText;
    List<Text> savedGameTexts;
    int currentPointer = 0;
    Vector3 textOffset = new Vector3(0f, -100f, 0f);
    bool choosingName = false;
    bool startGameAnimationActive = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        savedGamesMap = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        CreateSavedGameTexts();
        savedGameTexts[0].color = Color.magenta;
    }

    // Update is called once per frame
    void Update()
    {
        if (startGameAnimationActive)
            if (MenuController.Instance.bc.Animate())
                SceneManager.LoadScene("LoadSaveScene");

        if (!menuObject.activeSelf)
            return;

        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        if (choosingName)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // SetLoadedSave on First position
            var clone = savedGamesMap.ToList().Where(c => c.Key != currentPointer);
            var chosenSave = savedGamesMap[(uint)currentPointer];
            savedGamesMap.Clear();
            savedGamesMap.Add(0, chosenSave);
            uint i = 0;
            foreach (var v in clone)
                savedGamesMap.Add(++i, v.Value);
            Json.SaveToJson(savedGamesMap, "saves.json");
            startGameAnimationActive = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeActivate();
            MenuController.Instance.Activate();
        }
        else if (Input.GetKeyDown(KeyCode.S))
            UpdatePointer(currentPointer + 1);
        else if (Input.GetKeyDown(KeyCode.W))
            UpdatePointer(currentPointer - 1);
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            savedGameTexts[currentPointer].color = Color.green;
            savedGameTexts[currentPointer].text = "";
            choosingName = true;
        }
        else if(Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(savedGameTexts[currentPointer]);
            savedGameTexts.RemoveAt(currentPointer);
            savedGamesMap.Remove((uint)currentPointer);
            for(int i = currentPointer; i < savedGameTexts.Count; i++)
            {
                savedGameTexts[i].transform.position -= textOffset;
            }

            if (savedGameTexts.Count == 0)
            {
                DeActivate();
                MenuController.Instance.Activate();
            }
            if (currentPointer >= savedGameTexts.Count)
                UpdatePointer(savedGameTexts.Count-1);
            else
                UpdatePointer(currentPointer);

        }
    }
    // counter solves weird problem that keys are pressed twice
    int onguicounter = 0;
    private void OnGUI()
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
            savedGamesMap[(uint)currentPointer].Name = savedGameTexts[currentPointer].text;
            choosingName = false;
            skipFrame = true;
            savedGameTexts[currentPointer].color = Color.magenta;
            return;
        }

        if (key == KeyCode.Escape)
        {
            savedGameTexts[currentPointer].text = savedGamesMap[(uint)currentPointer].Name;
            choosingName = false;
            skipFrame = true;
            savedGameTexts[currentPointer].color = Color.magenta;

            return;
        }

        if(Input.GetKeyDown(key) && (uint)key >= (uint)KeyCode.A && (uint)key <= (uint)KeyCode.Z)
            savedGameTexts[currentPointer].text += key.ToString();
        Debug.Log((uint)key);
    }

    void CreateSavedGameTexts()
    {
        int i = 0;
        savedGameTexts = new List<Text>();
        foreach (var v in savedGamesMap)
        {
            GameObject text = Instantiate(textPrefab, menuObject.transform.position + ++i * textOffset - new Vector3(175f, 0f, 0f), Quaternion.identity, menuObject.transform);

            Text temp = text.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = v.Value.Name;
            temp.color = Color.black;
            temp.fontSize = 50;
            savedGameTexts.Add(temp);
        }
    }

    void UpdatePointer(int newPointer)
    {
        if (newPointer < 0 || newPointer >= savedGameTexts.Count) return;
        if(savedGameTexts.Count > currentPointer)
            savedGameTexts[currentPointer].color = Color.black;
        currentPointer = newPointer;
        if (savedGameTexts.Count > currentPointer)
            savedGameTexts[currentPointer].color = Color.magenta;
    }

    public void Activate()
    {
        menuObject.SetActive(true);
        skipFrame = true;
    }

    public void DeActivate()
    {
        Json.SaveToJson(savedGamesMap, "saves.json");
        menuObject.SetActive(false);
    }
}
