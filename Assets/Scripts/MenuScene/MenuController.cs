using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;
    [SerializeField]
    Image background;
    [SerializeField]
    GameObject buttonHolder;
    
    BackgroundController bc;

    Image[] menuButtons;
    bool startGameAnimationActive = false;
    Sprite[] menuButtonSprites;
    Text[] controlInputFields;

    int currentMenuButton = 0;

    bool menuIsOpen = true;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bc = new BackgroundController(background, Resources.LoadAll<Sprite>("Textures/bg"));
        menuButtonSprites = Resources.LoadAll<Sprite>("Textures/MenuSprites");
        menuButtons = buttonHolder.GetComponentsInChildren<Image>();

        for (int i = 0; i < menuButtons.Length; i++)
        {
            Debug.Log("say what?");
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
        Debug.Log(menuButtons.Length - 1 + " " + newButton);
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
