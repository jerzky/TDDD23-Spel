using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Items;
using System.Linq;

public class GeneralUI : MonoBehaviour
{
    public static GeneralUI Instance;
    [SerializeField]
    Slider healthSlider;
    [SerializeField]
    Text creditText;
    [SerializeField]
    Text killsText;
    [SerializeField]
    GameObject infoTextGameObject;
    [SerializeField]
    Text infoText;
    [SerializeField]
    Image infoPic;
    [SerializeField]
    bool infoTextOn = true;

    int health;
    int credits;
    int kills;

    private class InfoText
    {
        public string text;
        public bool isShowing = false;
        public Sprite sprite;
        public InfoText(string text, Sprite sprite)
        {
            this.text = text;
            this.sprite = sprite;
        }
    }
    Queue<InfoText> infoTextQueue = new Queue<InfoText>();

    public int Credits { get => credits; set { credits = value; creditText.text = "$" + credits;  } }
    public int Health { get => health; set { health = value; healthSlider.value = health; } }
    public int Kills { get => kills; set { kills = value; killsText.text = "Kills: " + kills.ToString(); } }

    private Sprite _pic;
    private readonly SortedDictionary<uint, string> _itemToInfo = new SortedDictionary<uint, string>();
    public SortedDictionary<uint, string> ItemToInfo { get => _itemToInfo; }
    private readonly SortedDictionary<string, string> _nearbyToInfo = new SortedDictionary<string, string>();
    public SortedDictionary<string, string> NearbyToInfo { get => _nearbyToInfo; }
    private string _guardText;
    // Start is called before the first frame update
    void Start()
    {
        SetUpBottomRightUI();
        SetUpInfoTexts();
    }

    void Update()
    {
        if (infoTextQueue.Count > 0)
        {
            var temp = infoTextQueue.Peek();
            if (!temp.isShowing)
            {
                infoTextGameObject.SetActive(true);
                infoText.text = temp.text;
                infoPic.sprite = temp.sprite;
            }
        }
    }

    void SetUpInfoTexts()
    {
        // MESSAGE IS CREATED IN SCENE ALREADY
        if (infoTextOn)
        {
            infoTextQueue.Enqueue(new InfoText("", null));
            infoTextQueue.Peek().isShowing = true;
            infoTextGameObject.SetActive(true);
        }
        LoadSaveInfo lsi = FindObjectOfType<LoadSaveInfo>();
        if (lsi != null)
        {
            if (lsi.LoadedGame.ItemToInfo != null)
                foreach (var v in lsi.LoadedGame.ItemToInfo)
                    _itemToInfo.Add(v.Key, v.Value);
            if (lsi.LoadedGame.NearbyToInfo != null)
                foreach (var v in lsi.LoadedGame.NearbyToInfo)
                    _nearbyToInfo.Add(v.Key, v.Value);
        }
        else
        {
            var keys = Json.JsonToContainer<SortedDictionary<ControlAction, ControlInfo>>("controldata.json");
            _itemToInfo.Add(ItemList.ITEM_LOCKPICK.UID, string.Format(ItemList.ITEM_LOCKPICK.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_DRILL.UID, string.Format(ItemList.ITEM_DRILL.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_EXPLOSIVE_TIMED.UID, string.Format(ItemList.ITEM_EXPLOSIVE_TIMED.Tooltip, (KeyCode)keys[ControlAction.UseItem].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_SLEDGEHAMMER.UID, string.Format(ItemList.ITEM_SLEDGEHAMMER.Tooltip, (KeyCode)keys[ControlAction.Shoot].KeyCode1));
            _itemToInfo.Add(ItemList.ITEM_PISTOL.UID, ItemList.ITEM_PISTOL.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_AK47.UID, ItemList.ITEM_AK47.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_SILENCED_PISTOL.UID, ItemList.ITEM_SILENCED_PISTOL.Tooltip);
            _itemToInfo.Add(ItemList.ITEM_MAC10.UID, ItemList.ITEM_MAC10.Tooltip);

            _nearbyToInfo.Add("Door", string.Format(Tooltips.DOOR_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Searchable Container", string.Format(Tooltips.SEARCHABLE_CONTAINER_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Store", string.Format(Tooltips.STORE_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Drill", string.Format(Tooltips.DRILL_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("Cash Register", string.Format(Tooltips.CASH_REGISTER_TOOLTIP, (KeyCode)keys[ControlAction.Interact].KeyCode1));
            _nearbyToInfo.Add("CCTV", Tooltips.CCTV_TOOLTIP);
            _nearbyToInfo.Add("Security Station", Tooltips.SECURITY_STATION_TOOLTIP);
            _guardText = string.Format(Tooltips.GUARD_TEXT, (KeyCode)keys[ControlAction.TakeDown].KeyCode1);
        }
    }

    void SetUpBottomRightUI()
    {
        Instance = this;
        creditText.color = Color.white;
        creditText.font = Resources.Load<Font>("DigitaldreamSkew");
        killsText.color = Color.white;
        killsText.font = Resources.Load<Font>("DigitaldreamSkew");

        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        Health = 100;
        Credits = 10000;
        Kills = 0;

        _pic = Resources.Load<Sprite>("Textures/Invisible");
    }

    public void TriggerInfoText(string text) { TriggerInfoText(text, _pic); }
    private void TriggerInfoText(string text, Sprite pic)
    {
        if(infoTextOn)
            infoTextQueue.Enqueue(new InfoText(text, pic));
    }

    // CloseInfoBox is called from a button on each infobox
    public void CloseInfoBox()
    {
        infoTextGameObject.SetActive(false);
        if(infoTextOn)
            infoTextQueue.Dequeue();
    }

    public void TriggerItemText(uint UID)
    {
        if (_itemToInfo.Count <= 0)
            return;

        if (!_itemToInfo.ContainsKey(UID))
            return;

        GeneralUI.Instance.TriggerInfoText(_itemToInfo[UID], ItemList.AllItems[UID].Sprite);
        _itemToInfo.Remove(UID);

        // If it is a weapon remove all weapons
        foreach (var v in _itemToInfo.ToList().Where(c => ItemList.AllItems[c.Key].ItemType == ItemType.Weapon))
            _itemToInfo.Remove(v.Key);
    }

    public void TriggerNearby(Collider2D v)
    {
        var inter = v.GetComponent<Interactable>();
        if (inter != null && _nearbyToInfo.ContainsKey(inter.Name()))
        {
            Sprite sprite = inter.GetComponent<SpriteRenderer>().sprite;
            if (inter.Name() == "Cash Register")
                sprite = Resources.LoadAll<Sprite>("Textures/x64spritesheet")[38];
            GeneralUI.Instance.TriggerInfoText(_nearbyToInfo[inter.Name()], sprite);
            _nearbyToInfo.Remove(inter.Name());
        }
        var ai = v.GetComponent<AI>();
        if (ai == null)
            return;

        if (ai.Type() == AI_Type.Guard)
        {
            if (_guardText != "")
                GeneralUI.Instance.TriggerInfoText(_guardText, ai.GetComponent<SpriteRenderer>().sprite);
            _guardText = "";
        }
        else if (ai.Type() == AI_Type.Civilian)
        {
            //BLABLA
        }
    }
}
