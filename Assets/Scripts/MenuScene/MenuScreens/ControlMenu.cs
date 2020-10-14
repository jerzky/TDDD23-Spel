using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;

public enum ControlAction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    Interact = 4,
    Shoot = 5,
    UseItem = 6,
    TakeDown = 7,
    Sneak = 8,
    Inventory = 9,
    Reload = 10,
    Itembar_1 = 11,
    Itembar_2 = 12,
    Itembar_3 = 13,
    Itembar_4 = 14,
    Itembar_5 = 15,
    Itembar_6 = 16,
    Itembar_7 = 17,
    Itembar_8 = 18
}

public class ControlInfo
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ControlAction Action { get; set; }
    public uint KeyCode1 { get; set; }
    public uint KeyCode2 { get; set; }

}

public class ControlMenu : MenuScreen
{
    Vector3 textOffset = new Vector3(0f, -55f, 0f);
    Vector3 controlOffset = new Vector3(370f, 0f, 0f);
    GameObject textPrefab;
    Text[,] keyTexts;
    SortedDictionary<ControlAction, ControlInfo> controlMap;
    ControlAction[] controlActions;
    bool choosingKey = false;
    Text currentRedText;

    // Start is called before the first frame update
    public ControlMenu(GameObject holder, Screen screen, GameObject textPrefab) : base(holder, screen)
    {
        this.textPrefab = textPrefab;
        controlMap = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
        CreateControlTexts();
        Json.SaveToJson(controlMap, "controldata.json");
        monitoredKeys = new List<KeyCode> { KeyCode.Return, KeyCode.Escape };
        limit.x = 1;
        limit.y = keyTexts.GetUpperBound(1);
    }

    public override void Move(Vector2 dir)
    {
        if (choosingKey)
            return;
        base.Move(dir);
    }

    protected override void ActivateMenuObject(Vector2 pos)
    {
        keyTexts[(int)position.x, (int)position.y].color = Color.magenta;
    }

    protected override void DeactivateMenuObject(Vector2 pos)
    {
        keyTexts[(int)position.x, (int)position.y].color = Color.black;
    }

    public override void KeyPressed(KeyCode key)
    {
        if (choosingKey)
            return;
        switch (key)
        {
            case KeyCode.Return:
                choosingKey = true;
                keyTexts[(int)position.x, (int)position.y].color = Color.green;
                break;
            case KeyCode.Escape:
                MenuController.Instance.ChangeMenuScreen(Screen.MainMenu);
                break;

        }
    }

    public override void OnGUI()
    {

        if (!choosingKey || Event.current.keyCode == KeyCode.Return)
            return;

        KeyCode lastKey = Event.current.keyCode;
        if (lastKey == KeyCode.None)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                lastKey = KeyCode.LeftShift;
            else if (Input.GetKey(KeyCode.Mouse0))
                lastKey = KeyCode.Mouse0;
            else if (Input.GetKey(KeyCode.Mouse1))
                lastKey = KeyCode.Mouse1;
            else if (Input.GetKey(KeyCode.Mouse2))
                lastKey = KeyCode.Mouse2;
            else if (Input.GetKey(KeyCode.Mouse3))
                lastKey = KeyCode.Mouse3;
            else if (Input.GetKey(KeyCode.Mouse4))
                lastKey = KeyCode.Mouse4;
            else
                return;
        }

        if (Event.current.keyCode == KeyCode.Delete)
            lastKey = KeyCode.None;

        if (Event.current.keyCode == KeyCode.Escape)
        {
            StopChoosingKey();
            return;
        }

        int y = 0;
        foreach (var v in controlMap)
        {
            int x = 0;
            if (KeyCode.None != lastKey && v.Value.KeyCode1 == (uint)lastKey)
                x = 1;
            if (KeyCode.None != lastKey && v.Value.KeyCode2 == (uint)lastKey)
                x = 2;

            if (x != 0)
            {
                if (currentRedText != null)
                    currentRedText.color = Color.black;
                currentRedText = keyTexts[x, y];
                keyTexts[x, y].color = Color.red;
                return;
            }
            y++;
        }

        if (position.x == 1)
        {
            controlMap[controlActions[(int)position.y]].KeyCode1 = (uint)lastKey;
            keyTexts[1, (int)position.y].text = lastKey.ToString();
        }
        else if (position.x == 2)
        {
            controlMap[controlActions[(int)position.y]].KeyCode2 = (uint)lastKey;
            keyTexts[2, (int)position.y].text = lastKey.ToString();
        }

        StopChoosingKey();
    }

    void StopChoosingKey()
    {
        if (currentRedText != null)
        {
            currentRedText.color = Color.black;
            currentRedText = null;
        }
        keyTexts[(int)position.x, (int)position.y].color = Color.magenta;
        choosingKey = false;
        SkipFrame = true;
    }


    void CreateControlTexts()
    {
        int i = 0;
        keyTexts = new Text[3, controlMap.Count];
        controlActions = new ControlAction[controlMap.Count];
        int x = 0;
        int y = 0;
        foreach (var v in controlMap)
        {
            controlActions[y] = v.Key;

            GameObject parentText = GameObject.Instantiate(textPrefab, holder.transform.position + i++ * textOffset, Quaternion.identity, holder.transform);
            GameObject child1 = GameObject.Instantiate(textPrefab, parentText.transform.position + controlOffset, Quaternion.identity, parentText.transform);
            GameObject child2 = GameObject.Instantiate(textPrefab, child1.transform.position + controlOffset, Quaternion.identity, parentText.transform);

            Text temp = parentText.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = v.Key.ToString();
            temp.color = Color.black;
            keyTexts[x+2, y] = temp;

            temp = child1.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = ((KeyCode)v.Value.KeyCode1).ToString();
            temp.color = Color.black;
            if (y == 0)
                temp.color = Color.magenta;
            keyTexts[x, y] = temp;
            

            temp = child2.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = ((KeyCode)v.Value.KeyCode2).ToString();
            temp.color = Color.black;
            keyTexts[x+1, y] = temp;
            x = 0;
            y++;
        }
    }

    public override void ResetPosition()
    {
        MoveTo(new Vector2(1, 0));
    }
}
