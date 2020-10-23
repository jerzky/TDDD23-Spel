using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Interactable
{
    const int AVERAGE_CASH = 5000;
    const int TIMERCOUNT_RESETVALUE = 3; // only 3 sprites
    [SerializeField]
    float cashMultiplier = 1f;
    int cash;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    bool timerActive = false;
    SimpleTimer timer = new SimpleTimer(5f);
    int timerCount = TIMERCOUNT_RESETVALUE;
    Sprite[] sprites;
    int spriteIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Textures/money");
        Reset();
    }

    void Update()
    {
        if(timerActive)
            if(timer.TickAndReset())
            {
                if(--timerCount <= 0)
                {
                    boxCollider.enabled = false;
                    spriteRenderer.enabled = false;
                    cash = 0;
                    LoadingCircle.Instance.StopLoading();
                }
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/ChaChing1"), PlayerController.Instance.transform.position);
                Debug.Log(cash / TIMERCOUNT_RESETVALUE);
                GeneralUI.Instance.Credits += cash / TIMERCOUNT_RESETVALUE;
                spriteIndex = (spriteIndex - 1) < 0 ? 2 : (spriteIndex - 1);
                spriteRenderer.sprite = sprites[spriteIndex];
            }

    }
    public override void Cancel()
    {
        timerActive = false;
        LoadingCircle.Instance.StopLoading();
    }

    public override bool Interact(uint itemIndex)
    {
        if (itemIndex != 0)
            return false;
        timer.Reset();
        timerActive = true;
        LoadingCircle.Instance.StartLoading();
        return true;
    }

    public override string Name()
    {
        return "Money";
    }

    public void Reset()
    {
        boxCollider.enabled = true;
        spriteIndex = 2;
        spriteRenderer.sprite = sprites[2];
        spriteRenderer.enabled = true;
        cash = (int)(AVERAGE_CASH * cashMultiplier * Random.Range(0.5f, 1.5f));
        timerCount = TIMERCOUNT_RESETVALUE;
        timer.Reset();
    }
}
