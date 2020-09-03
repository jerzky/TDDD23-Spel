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
        [DefaultValue(1f)]
        public float BoxColliderScaleX { get; set; }
        [DefaultValue(1f)]
        public float BoxColliderScaleY { get; set; }
        public override string ToString()
        {
            return string.Format("R: {0} G: {1}, B: {2}", R, G, B);
        }
    }



    SortedDictionary<string, MapTileData> tileDictLevelOne;
    SortedDictionary<string, MapTileData> tileDictLevelTwo;

    GameObject[] tiles = new GameObject[3];


    // Start is called before the first frame update
    void Start()
    {

        tileDictLevelOne = new SortedDictionary<string, MapTileData>();
        tileDictLevelTwo = new SortedDictionary<string, MapTileData>();

        tiles[0] = new GameObject("BaseTiles");
        tiles[1] = new GameObject("LevelTwoTiles");
        tiles[2] = new GameObject("LevelThreeTiles");

        ReadColorOrder();
        foreach(var v in tileDictLevelOne)
        {
            Debug.Log(v.Value.Name + " " + v.Key);
        }
        ReadImageToMap("Maps/MapLayerOne", tileDictLevelOne, 0);
        ReadImageToMap("Maps/MapLayerTwo", tileDictLevelOne, 1);
        ReadImageToMap("Maps/MapLayerThree", tileDictLevelTwo, 2);
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
            Debug.Log(tile);
            var colorString = new Color((float)tile.R / 255, (float)tile.G / 255, (float)tile.B / 255).ToString();
            Debug.Log(colorString);

            if (tile.UID < 100)
                tileDictLevelOne.Add(colorString, tile);
            else if (tile.UID > 100)
                tileDictLevelTwo.Add(colorString, tile);
                
        }
    }
    void ReadImageToMap(string imagePath, SortedDictionary<string, MapTileData> tileDict, int level)
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
                Debug.Log(string.Format("Found pixel: {0}", pixelColor));

                if (tileDict.TryGetValue(pixelColor.ToString(), out MapTileData tile) && pixelColor.ToString() != new Color(0,0,0).ToString())
                {
                    Debug.Log(tile.Name);
                    Sprite sprite = Resources.LoadAll<Sprite>(tile.Path)[tile.Index];

                    if (sprite == null) Debug.Log("sprite is null");
                    GameObject temp = new GameObject(tile.Name);
                    temp.transform.parent = tiles[level].transform;
                    temp.transform.position = new Vector3(x, y, 0);
                    temp.AddComponent<SpriteRenderer>().sprite = sprite;
                    temp.layer = tile.LayerMask;

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
                                case "searchablecontainer":
                                    temp.AddComponent<SearchableContainer>();
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
        Debug.Log(path);
        File.WriteAllText(path, json);
        
    }







}
