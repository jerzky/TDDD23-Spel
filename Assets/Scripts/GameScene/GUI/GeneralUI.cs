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

    int health;
    int credits;
    int kills;
    int totalCreditsEarned;

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
    }

    void Update()
    {

    }
}
