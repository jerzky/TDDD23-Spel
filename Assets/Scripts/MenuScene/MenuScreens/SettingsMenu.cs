using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum SettingType 
{
    ToolTipOn
}

public class Setting
{
    public SettingType SettingType { get; private set; }
    public object Val { get; set; }
    public Type ValType;

    public Setting(SettingType key, object value, Type valueType)
    {
       
    }
}


public class SettingsMenu : MenuScreen
{
    GameObject textPrefab;
    Text[,] settingTexts;
    SortedDictionary<SettingType, string> settingMap;

    public SettingsMenu(GameObject holder, string name, GameObject textPrefab) : base(holder, name)
    {
        this.textPrefab = textPrefab;
        monitoredKeys = new List<KeyCode> { KeyCode.Return, KeyCode.Escape };
    }
    public override void KeyPressed(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.Return:
                break;
            case KeyCode.Escape:
                break;
        }
    }

    void CreateTextObjects()
    {
        /*settingTexts = new Text[3, controlMap.Count];
        controlActions = new ControlAction[controlMap.Count];
        int x = 0;
        int y = 0;
        foreach (var v in controlMap)
        {
            controlActions[y] = v.Key;

            GameObject parentText = GameObject.Instantiate(textPrefab, holder.transform.position + y * textOffset, Quaternion.identity, holder.transform);
            GameObject child1 = GameObject.Instantiate(textPrefab, parentText.transform.position + controlOffset, Quaternion.identity, parentText.transform);
            GameObject child2 = GameObject.Instantiate(textPrefab, child1.transform.position + controlOffset, Quaternion.identity, parentText.transform);

            Text temp = parentText.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = v.Key.ToString();
            temp.color = Color.black;
            settingTexts[x + 1, y] = temp;

            temp = child1.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = ((KeyCode)v.Value.KeyCode1).ToString();
            temp.color = Color.black;
            if (y == 0)
                temp.color = Color.magenta;
            settingTexts[x, y] = temp;
            x = 0;
            y++;
        }*/
    }

    protected override void ActivateMenuObject(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }

    protected override void DeactivateMenuObject(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }
}
