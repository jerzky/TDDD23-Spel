using System.Collections.Generic;
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
    Dictionary<MenuScreen.Screen, MenuScreen> menuScreens = new Dictionary<MenuScreen.Screen, MenuScreen>();
    MenuScreen.Screen currentMenu = MenuScreen.Screen.MainMenu;
    string sceneToBeLoaded = "GameScene";
    

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bc = new BackgroundController(background, Resources.LoadAll<Sprite>("NoSpriteAtlasTextures/bg"));
        menuScreens.Add(MenuScreen.Screen.MainMenu, new MainMenu(mainMenuButtonHolder, MenuScreen.Screen.MainMenu));
        menuScreens.Add(MenuScreen.Screen.ControlEditor, new ControlMenu(controlEditHolder, MenuScreen.Screen.ControlEditor, controlEditorTextPrefab));
        menuScreens.Add(MenuScreen.Screen.LoadGame, new LoadGamesMenu(loadSaveHolder, MenuScreen.Screen.LoadGame, loadGamesTextPrefab));
        menuScreens[MenuScreen.Screen.MainMenu].Activate();
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

    public void ChangeMenuScreen(MenuScreen.Screen screen)
    {

        menuScreens[currentMenu].DeActivate();
        currentMenu = screen;
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
