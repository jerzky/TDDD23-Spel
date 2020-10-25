using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MenuScreen
{
    Image[] menuButtons;
    public MainMenu(GameObject holder, Screen screen) : base(holder, screen)
    {
        menuButtons = holder.GetComponentsInChildren<Image>();
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].color = Color.blue;
        }
        menuButtons[0].color = Color.magenta;

        limit.x = 0f;
        limit.y = menuButtons.Length - 1;
        monitoredKeys = new List<KeyCode> { KeyCode.Return };
    }

    public override void KeyPressed(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Return:
                Select();
                break;
        }
    }

    private void Select()
    {
        switch ((int)position.y)
        {
            case 0:
                holder.SetActive(false);
                MenuController.Instance.startGameAnimationActive = true;
                break;
            case 1:
                MenuController.Instance.ChangeMenuScreen(Screen.ControlEditor);
                break;
            case 2:
                MenuController.Instance.ChangeMenuScreen(Screen.LoadGame);
                break;
            case 3:
                #if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                #endif
                Application.Quit();
                break;

        }
    }

    protected override void ActivateMenuObject(Vector2 pos)
    {
        menuButtons[(int)pos.y].color = Color.magenta;
    }

    protected override void DeactivateMenuObject(Vector2 pos)
    {
        menuButtons[(int)pos.y].color = Color.blue;
    }
}
