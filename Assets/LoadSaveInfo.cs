using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSaveInfo : MonoBehaviour
{
    public SavedGame LoadedGame { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        var savedGames = Json.JsonToContainer<SortedDictionary<uint, SavedGame>>("saves.json");
        LoadedGame = savedGames[0];
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
}
