using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.ComponentModel;

public class MapController : MonoBehaviour
{
    public class MapTileData 
    {
        public uint UID { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        [JsonProperty("Texture")]
        public string Path { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int LayerMask { get; set; }
        public List<string> Components { get; set; }
        [DefaultValue(false)]
        public bool Moveable { get; set; }

        public override string ToString()
        {
            return string.Format("R: {0} G: {1}, B: {2}", R, G, B);
        }

    }



    SortedDictionary<string, MapTileData> ColorOrder;

    // Start is called before the first frame update
    void Start()
    {
 
        ColorOrder = new SortedDictionary<string, MapTileData>();
        ReadColorOrder();
<<<<<<< Updated upstream
        foreach(var v in ColorOrder)
        {
            Debug.Log(v.Value.Name + " " + v.Key);
        }
        ReadImageToMap("Textures/maptest");
=======
        // create grass background
        Texture2D bitMap = Resources.Load("Maps/MapLayerOne") as Texture2D;
        GameObject background = new GameObject("background");
        background.transform.position = new Vector3(bitMap.width/2 - background.transform.position.x, bitMap.height/2 - background.transform.position.y, 100);
        SpriteRenderer sr = background.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.LoadAll<Sprite>("Textures/BuildingSpriteSheet")[120];
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(bitMap.width, bitMap.height);
        ReadImageToMap("Maps/MapLayerOne", tileDictLevelOne, 0);
        ReadImageToMap("Maps/MapLayerTwo", tileDictLevelTwo, 1);
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ReadColorOrder()
    {
        var path = Path.Combine(Application.dataPath, "tiledata.json");
        var fileContent = File.ReadAllText(path);
        var tiles = JsonConvert.DeserializeObject<List<MapTileData>>(fileContent);

        foreach(var tile in tiles)
        {
            var colorString = new Color((float)tile.R / 255, (float)tile.G / 255, (float)tile.B / 255).ToString();
<<<<<<< Updated upstream
            Debug.Log(colorString);
            ColorOrder.Add(colorString,  tile);
=======
     

            if (tile.UID < 100)
                tileDictLevelOne.Add(colorString, tile);
            else if (tile.UID > 100)
                tileDictLevelTwo.Add(colorString, tile);
                
>>>>>>> Stashed changes
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

        for (int x = 0; x < bitMap.width; x++)
        {
            for (int y = 0; y < bitMap.height; y++)
            {
                Color pixelColor = bitMap.GetPixel(x, y);
   

                if (ColorOrder.TryGetValue(pixelColor.ToString(), out MapTileData tile))
                {
                    Sprite sprite = Resources.LoadAll<Sprite>(tile.Path)[tile.Index];

                    if (sprite == null) 
                        Debug.LogError("sprite is null");
                    GameObject temp = new GameObject(tile.Name);
                    temp.transform.position = new Vector3(x, y, 0);
                    temp.AddComponent<SpriteRenderer>().sprite = sprite;
                    temp.layer = tile.LayerMask;
                    if(tile.Moveable)
                    {
                        GameObject under = new GameObject("under");
                        under.transform.position = new Vector3(x, y, 1);
                        under.AddComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/BuildingSpriteSheet")[363];
                        under.layer = 2;
                    }
                    if (tile.Components != null)
                    {
                        foreach (var str in tile.Components)
                        {
                            switch(str.ToLower())
                            {
                                case "boxcollider":
                                    temp.AddComponent<BoxCollider2D>();
                                    break;
                                case "door":
                                    temp.AddComponent<Door>();
                                    break;
                            }
                        }
                    }
                    
                }

            }
        }
    }

    
    void Test()
    {
        var maptile = new MapTileData
        {
            Index = 0,
            Path = "Grass",
            R = 255,
            G = 255,
            B = 112,
            LayerMask = 0,
            Components = new List<string>
            {
                "Script",
                "BoxCollider"
            }
        };

        var list = new List<MapTileData> { maptile };
        
        var setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        var json = JsonConvert.SerializeObject(list, setting);
        var path = Path.Combine(Application.dataPath, "tiledata.json");
        File.WriteAllText(path, json);
        
    }







}
