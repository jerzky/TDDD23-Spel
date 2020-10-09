using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        creditText.color = Color.white;
        creditText.font = Resources.Load<Font>("DigitaldreamSkew");
        killsText.color = Color.white;
        killsText.font = Resources.Load<Font>("DigitaldreamSkew");

        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;
        Health = 100;
        Credits = 100000;
        Kills = 0;

        _pic = Resources.Load<Sprite>("Textures/Invisible");


        // MESSAGE IS CREATED IN SCENE ALREADY
        if (infoTextOn)
        {
            infoTextQueue.Enqueue(new InfoText("", null));
            infoTextQueue.Peek().isShowing = true;
            infoTextGameObject.SetActive(true);
        }
        
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
    public void TriggerInfoText(string text) { TriggerInfoText(text, _pic); }
    public void TriggerInfoText(string text, Sprite pic)
    {
        if(infoTextOn)
            infoTextQueue.Enqueue(new InfoText(text, pic));
    }

    public void CloseInfoBox()
    {
        infoTextGameObject.SetActive(false);
        if(infoTextOn)
            infoTextQueue.Dequeue();
    }
}
