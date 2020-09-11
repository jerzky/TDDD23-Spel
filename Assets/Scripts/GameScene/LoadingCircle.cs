using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour
{
    public static LoadingCircle Instance;
    private Transform rectComponent;
    private SpriteRenderer sr;


    private float lastUpdate;
    private void Start()
    {
        Instance = this;
        rectComponent = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void StartLoading()
    {
        sr.enabled = true;
    }

    public void StopLoading()
    {
        rectComponent.rotation = rectComponent.parent.rotation;
        lastUpdate = 0;
        sr.enabled = false;
    }

    private void Update()
    {
        if (!sr.enabled)
            return;

        lastUpdate += Time.deltaTime;
        if (lastUpdate < 1)
            return;
        rectComponent.Rotate(0f, 0f, -72);
        lastUpdate = 0;
    }

 
}
