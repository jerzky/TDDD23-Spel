
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController
{
    Image image;
    Sprite[] backgroundSprites;
    float timer = 0f;
    uint currentSprite = 0;
    float zoomSpeed = 250f;

    public BackgroundController(Image image, Sprite[] backgroundSprites)
    {
        this.image = image;
        this.backgroundSprites = backgroundSprites;
        this.image.sprite = backgroundSprites[0];
        this.currentSprite = 0;
    }


    public bool Animate()
    {
        timer += Time.deltaTime;
        if(timer > 0.035f)
        {
            if (currentSprite >= backgroundSprites.Length-1)
            {
                if (image.rectTransform.rect.width > 1920*1.5)
                    return true;

                image.rectTransform.sizeDelta = new Vector2(image.rectTransform.rect.width + zoomSpeed * 1920/1080 * Time.deltaTime, image.rectTransform.rect.height + zoomSpeed * Time.deltaTime);
                return false;
            }
            image.sprite = backgroundSprites[++currentSprite];
            timer = 0f;
        }
        return false;
    }
}
