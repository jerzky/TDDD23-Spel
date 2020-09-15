using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEditor.U2D.Path.GUIFramework;
using Newtonsoft.Json.Converters;


public enum ControlAction { Up, Down, Left, Right, Interact, Shoot, UseItem, Sneak, Inventory, Itembar_1, Itembar_2, Itembar_3, Itembar_4, Itembar_5, Itembar_6, Itembar_7, Itembar_8 }

public class ControlInfo
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ControlAction Action { get; set; }
    public uint KeyCode1 { get; set; }
    public uint KeyCode2 { get; set; }

}

public class ControlEditor : MonoBehaviour
{
    public static ControlEditor Instance;

    [SerializeField]
    GameObject holder;
    [SerializeField]
    GameObject textPrefab;

    Vector3 textOffset = new Vector3(0f, -55f, 0f);
    Vector3 controlOffset = new Vector3(370f, 0f, 0f);
    Vector3 controlOffset2 = new Vector3(270f, 0f, 0f);

    Vector2 currentPointer = new Vector2(1,0);

    Text[,] keyTexts;
    SortedDictionary<ControlAction, ControlInfo> controlMap;
    ControlAction[] controlActions;


    bool controlEditorOpen = false;
    bool choosingKey = false;
    bool skipFrame = true;

    Text currentRedText;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //controlMap = new SortedDictionary<ControlAction, ControlInfo>();
        //controlMap.Add(ControlAction.Up, new ControlInfo { Action = ControlAction.Up, KeyCode1 = 1, KeyCode2 = 2 });
        controlMap = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
        CreateControlTexts();
        Json.SaveToJson<SortedDictionary<ControlAction, ControlInfo>>(controlMap, "controldata.json");
  
    }
    
    // Update is called once per frame
    void Update()
    {

        if (!controlEditorOpen)
            return;
        if(skipFrame)
        {
            skipFrame = false;
            return;
        }
        if (choosingKey)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            choosingKey = true;
            keyTexts[(int)currentPointer.x, (int)currentPointer.y].color = Color.green;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeActivate();
            MenuController.Instance.Activate();
        }
        else if (Input.GetKeyDown(KeyCode.S))
            UpdatePointer(currentPointer + Vector2.up);
        else if (Input.GetKeyDown(KeyCode.W))
            UpdatePointer(currentPointer + Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A))
            UpdatePointer(currentPointer + Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D))
            UpdatePointer(currentPointer + Vector2.right);
    }
    
    private void OnGUI()
    {

        if (choosingKey && Event.current.keyCode != KeyCode.Return)
        {
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

            if (Event.current.keyCode == KeyCode.Escape)
                lastKey = KeyCode.None;

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

            if (currentPointer.x == 1)
            {
                controlMap[controlActions[(int)currentPointer.y]].KeyCode1 = (uint)lastKey;
                keyTexts[1, (int)currentPointer.y].text = lastKey.ToString();
            }
            else if (currentPointer.x == 2)
            {
                controlMap[controlActions[(int)currentPointer.y]].KeyCode2 = (uint)lastKey;
                keyTexts[2, (int)currentPointer.y].text = lastKey.ToString();
            }


            if (currentRedText != null)
            {
                currentRedText.color = Color.black;
                currentRedText = null;
            }

            keyTexts[(int)currentPointer.x, (int)currentPointer.y].color = Color.magenta;
            choosingKey = false;
            skipFrame = true;
        }
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

            GameObject parentText = Instantiate(textPrefab, holder.transform.position + i++ * textOffset, Quaternion.identity, holder.transform);
            GameObject child1 = Instantiate(textPrefab, parentText.transform.position + controlOffset, Quaternion.identity, parentText.transform);
            GameObject child2 = Instantiate(textPrefab, child1.transform.position + controlOffset, Quaternion.identity, parentText.transform);

            Text temp = parentText.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = v.Key.ToString();
            temp.color = Color.black;
            keyTexts[x++, y] = temp;

            temp = child1.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = ((KeyCode)v.Value.KeyCode1).ToString();
            temp.color = Color.black;
            if (y == 0)
                temp.color = Color.magenta;
            keyTexts[x++, y] = temp;
            

            temp = child2.GetComponent<Text>();
            temp.font = Resources.Load<Font>("DigitaldreamSkew");
            temp.text = ((KeyCode)v.Value.KeyCode2).ToString();
            temp.color = Color.black;
            keyTexts[x, y] = temp;
            x = 0;
            y++;
        }
    }

    void UpdatePointer(Vector2 newPointer)
    {
        if (newPointer.x < 1 || newPointer.x > 2) return;
        if (newPointer.y < 0 || newPointer.y > 15) return;
        keyTexts[(int)currentPointer.x, (int)currentPointer.y].color = Color.black;
        currentPointer = newPointer;
        keyTexts[(int)currentPointer.x, (int)currentPointer.y].color = Color.magenta;
    }

    public void Activate()
    {
        controlEditorOpen = true;
        holder.SetActive(true);
        skipFrame = true;
    }

    public void DeActivate()
    {
        Json.SaveToJson<SortedDictionary<ControlAction, ControlInfo>>(controlMap, "controldata.json");
        controlEditorOpen = false;
        holder.SetActive(false);
    }
}
