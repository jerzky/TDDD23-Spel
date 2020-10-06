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
    Text totalCreditsEarnedText;
    [SerializeField]
    Text killsText;
    [SerializeField]
    GameObject infoTextGameObject;
    [SerializeField]
    Text infoText;
    [SerializeField]
    Image infoPic;

    int health;
    int credits;
    int kills;
    int totalCreditsEarned;

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
    float infoTextTimer = 0f;

    public int Credits { get => credits; set { if (value > 0) TotalCreditsEarned += value; credits = value; creditText.text = "Current $" + credits;  } }
    public int Health { get => health; set { health = value; healthSlider.value = health; } }
    public int Kills { get => kills; set { kills = value; killsText.text = "Kills: " + kills.ToString(); } }

    private int TotalCreditsEarned { get => totalCreditsEarned; set { totalCreditsEarned = value; totalCreditsEarnedText.text = "Total Earned $" + totalCreditsEarned.ToString(); } }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        creditText.color = Color.white;
        creditText.font = Resources.Load<Font>("DigitaldreamSkew");
        totalCreditsEarnedText.color = Color.white;
        totalCreditsEarnedText.font = Resources.Load<Font>("DigitaldreamSkew");
        killsText.color = Color.white;
        killsText.font = Resources.Load<Font>("DigitaldreamSkew");
        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;

        Health = 100;
        Credits = 1000;
        kills = 0;
        totalCreditsEarned = 0;

        // MESSAGE IS CREATED IN SCENE ALREADY
        infoTextQueue.Enqueue(new InfoText("", null));
        infoTextQueue.Peek().isShowing = true;
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

    public void TriggerInfoText(string text, Sprite pic)
    {
        infoTextQueue.Enqueue(new InfoText(text, pic));
    }

    public void CloseInfoBox()
    {
        infoTextGameObject.SetActive(false);
        infoTextQueue.Dequeue();
    }
}
