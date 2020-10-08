using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class MenuController : MonoBehaviour
{
    public static MenuController Instance;
    [SerializeField]
    Image background;
    [SerializeField]
    GameObject mainMenuButtonHolder;
    [SerializeField]
    GameObject controlEditHolder;
    [SerializeField]
    GameObject loadSaveHolder;
    [SerializeField]
    GameObject controlEditorTextPrefab;
    [SerializeField]
    GameObject loadGamesTextPrefab;

    public BackgroundController bc;
    public bool startGameAnimationActive = false;
    Dictionary<string, MenuScreen> menuScreens = new Dictionary<string, MenuScreen>();
    string currentMenu = "MainMenu";
    string sceneToBeLoaded = "GameScene";
    

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bc = new BackgroundController(background, Resources.LoadAll<Sprite>("NoSpriteAtlasTextures/bg"));
        menuScreens.Add("MainMenu", new MainMenu(mainMenuButtonHolder, "MainMenu"));
        menuScreens.Add("ControlEditor", new ControlMenu(controlEditHolder, "ControlEditor", controlEditorTextPrefab));
        menuScreens.Add("LoadGamesMenu", new LoadGamesMenu(loadSaveHolder, "LoadGamesMenu", loadGamesTextPrefab));
        menuScreens["MainMenu"].Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (startGameAnimationActive)
            if(bc.Animate())
                SceneManager.LoadScene(sceneToBeLoaded);

        if (menuScreens[currentMenu].SkipFrame)
        {
            menuScreens[currentMenu].SkipFrame = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
            menuScreens[currentMenu].Move(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S))
            menuScreens[currentMenu].Move(Vector2.down);
        else if(Input.GetKeyDown(KeyCode.A))
            menuScreens[currentMenu].Move(Vector2.right);
        else if (Input.GetKeyDown(KeyCode.D))
            menuScreens[currentMenu].Move(Vector2.left);

        foreach (var v in menuScreens[currentMenu].MonitoredKeys)
            if (Input.GetKeyDown(v))
                menuScreens[currentMenu].KeyPressed(v);
    }

    public void ChangeMenuScreen(string name)
    {

        menuScreens[currentMenu].DeActivate();
        currentMenu = name;
        menuScreens[currentMenu].Activate();
    }

    public void StartGameAnimation(string sceneName)
    {
        startGameAnimationActive = true;
        sceneToBeLoaded = sceneName;
    }

    void OnGUI()
    {
        menuScreens[currentMenu].OnGUI();
    }
}
