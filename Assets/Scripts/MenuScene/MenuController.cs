using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class MenuScreen
{
    protected Vector2 limit;
    public MenuScreen()
    {

    }

    protected bool WithinLimit(Vector2 pos)
    {
        return Mathf.Clamp(pos.x, 0f, limit.x) == pos.x && Mathf.Clamp(pos.y, 0f, limit.y) == pos.y;
    }

    public abstract void ActivateMenuObject(Vector2 pos);
    public abstract void DeactivateMenuObject(Vector2 pos);
}

public class MainMenu : MenuScreen
{
    Image[] menuButtons;
    public MainMenu(GameController buttonHolder)
    {
        limit.x = 0f;
        limit.y = 3f;

        menuButtons = buttonHolder.GetComponentsInChildren<Image>();
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].color = Color.blue;
        }
        menuButtons[0].color = Color.magenta;
    }
    public override void ActivateMenuObject(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }

    public override void DeactivateMenuObject(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }
}

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;
    [SerializeField]
    Image background;
    [SerializeField]
    GameObject buttonHolder;
    
    public BackgroundController bc;

    Image[] menuButtons;
    bool startGameAnimationActive = false;

    int currentMenuButton = 0;

    bool menuIsOpen = true;

    

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bc = new BackgroundController(background, Resources.LoadAll<Sprite>("NoSpriteAtlasTextures/bg"));
        menuButtons = buttonHolder.GetComponentsInChildren<Image>();

        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].color = Color.blue;
        }
        menuButtons[0].color = Color.magenta;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startGameAnimationActive)
            if(bc.Animate())
                SceneManager.LoadScene("GameScene");

        if (!menuIsOpen)
            return;

        if (Input.GetKeyDown(KeyCode.S))
            UpdateCurrentMenuButton(currentMenuButton + 1);
        else if (Input.GetKeyDown(KeyCode.W))
            UpdateCurrentMenuButton(currentMenuButton - 1);

        if (Input.GetKeyDown(KeyCode.Return))
            ClickButton(currentMenuButton);
    }

    void UpdateCurrentMenuButton(int newButton)
    {
        if (newButton < 0 || newButton >= menuButtons.Length) return;
        menuButtons[currentMenuButton].color = Color.blue;
        currentMenuButton = newButton;
        menuButtons[currentMenuButton].color = Color.magenta;
    }

    void ClickButton(int buttonIndex)
    {
        switch(buttonIndex)
        {
            case 0:
                buttonHolder.SetActive(false);
                startGameAnimationActive = true;
                break;
            case 1:
                DeActivate();
                ControlEditor.Instance.Activate();
                break;
            case 2:
                DeActivate();
                SavedGamesHandler.Instance.Activate();
                break;
            case 3:
                #if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                #endif
                break;
            
        }
    }

    public void Activate()
    {
        menuIsOpen = true;
        buttonHolder.SetActive(true);
    }

    public void DeActivate()
    {
        menuIsOpen = false;
        buttonHolder.SetActive(false);
    }


}
