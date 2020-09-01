using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapController : MonoBehaviour
{

    SortedDictionary<string, int> ColorOrder;

    // Start is called before the first frame update
    void Start()
    {
        ColorOrder = new SortedDictionary<string, int>();
        ReadColorOrder();
        ReadImageToMap("Textures/maptest");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadColorOrder()
    {
        Texture2D bitMap = Resources.Load<Texture2D>("Textures/colororder");
        StreamReader file = new StreamReader("Assets/Resources/Documents/colormaping.txt");
        if(bitMap == null)
        {
            Debug.LogError("Could not read the colororder texture! (MapController/ReadColorOrder)");
            return;
        }
        if(file == null)
        {
            Debug.LogError("Could not read the colormaping.txt! (MapController/ReadColorOrder)");
            return;
        }
        file.ReadLine(); // remove header
        for (int y = 0; true; y++)
        {
            Color pixelColor = bitMap.GetPixel(0, bitMap.height-1-y);
            string str = file.ReadLine();

            if (str == "END")
                break;

            int spaceIndex = str.IndexOf(" ");
            str = str.Substring(spaceIndex+1);
            int spriteIndex = -1;
            int.TryParse(str, out spriteIndex);
            if(spriteIndex >= 0)
                ColorOrder.Add(pixelColor.ToString(), spriteIndex);

            if (pixelColor.Equals(new Color(1.0f, 1.0f, 1.0f))) 
                break;
        }
    }

    void ReadImageToMap(string imagePath)
    {
        Texture2D bitMap = Resources.Load(imagePath) as Texture2D;
        if (bitMap == null)
        {
            Debug.LogError("Could not read the file at " + imagePath + "! (MapController/ReadImageToMap)");
            return;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/BuildingSpriteSheet");

        for (int x = 0; x < bitMap.width; x++)
        {
            for (int y = 0; y < bitMap.height; y++)
            {
                Color pixelColor = bitMap.GetPixel(x, y);
                Debug.Log(pixelColor);
                
                if (ColorOrder.TryGetValue(pixelColor.ToString(), out int spriteIndex))
                {
                    Sprite sprite = sprites[spriteIndex];
                    if (sprite == null) Debug.Log("sprite is null");
                    GameObject temp = new GameObject("mapObject(" + x + ", " + y + ")");
                    temp.transform.position = new Vector3(x, y, 0);
                    temp.AddComponent<SpriteRenderer>().sprite = sprite;
                    if(spriteIndex != 258 && spriteIndex != 89)
                        temp.AddComponent<BoxCollider2D>();
                }
            }
        }
    }
}
