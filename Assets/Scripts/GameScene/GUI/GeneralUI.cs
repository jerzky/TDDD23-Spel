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

    int health;
    int credits;

    public int Credits { get => credits; set { credits = value; creditText.text = "$" + credits;  } }
    public int Health { get => health; set { health = value; healthSlider.value = health; } }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        creditText.color = Color.white;
        creditText.font = Resources.Load<Font>("DigitaldreamSkew");
        healthSlider.maxValue = 100;
        healthSlider.minValue = 0;

        Health = 100;
        Credits = 1000;
    }
}
