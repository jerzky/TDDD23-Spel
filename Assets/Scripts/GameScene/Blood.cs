using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    Sprite[] bloodSprites;
    float timer = 0f;
    SpriteRenderer sr;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        bloodSprites = Resources.LoadAll<Sprite>("Textures/blood");
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = bloodSprites[index++];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.07f)
        {
            sr.sprite = bloodSprites[index++];
            timer = 0f;
            if(index >= bloodSprites.Length)
            {
                Destroy(gameObject);
            }
        }
    }
}
