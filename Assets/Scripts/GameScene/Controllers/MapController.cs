using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.ComponentModel;
using UnityEngine.Tilemaps;
using Assets.Items;

public class MapController : MonoBehaviour
{
    public class MapTileData
    { 
        public int Index { get; set; }
        public string Name { get; set; }
        [JsonProperty("Texture")]
        public string Path { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int LayerMask { get; set; }
        [DefaultValue("Untagged")]
        public string Tag { get; set; }
        [DefaultValue(Mathf.Infinity)]
        public int Durability { get; set; }
        public List<string> Components { get; set; }
        [DefaultValue(1f)]
        public float BoxColliderScaleX { get; set; }
        [DefaultValue(1f)]
        public float BoxColliderScaleY { get; set; }
        [DefaultValue(0f)]
        public float BoxColliderOffsetX { get; set; }
        [DefaultValue(0f)]
        public float BoxColliderOffsetY { get; set; }
        public override string ToString()
        {
            return string.Format("R: {0} G: {1}, B: {2}", R, G, B);
        }
    }

    SortedDictionary<string, MapTileData> tileDictLevelOne;
    SortedDictionary<string, MapTileData> tileDictLevelTwo;
    SortedDictionary<string, MapTileData> tileDictLevelThree;
    public static SortedDictionary<string, MapTileData> AllTiles;

    GameObject[] tiles = new GameObject[3];


    // Start is called before the first frame update
    void Start()
    {

        tileDictLevelOne = new SortedDictionary<string, MapTileData>();
        tileDictLevelTwo = new SortedDictionary<string, MapTileData>();
        tileDictLevelThree = new SortedDictionary<string, MapTileData>();
        AllTiles = new SortedDictionary<string, MapTileData>();
        tiles[0] = new GameObject("LevelOneTiles");
        tiles[1] = new GameObject("LevelTwoTiles");
        tiles[2] = new GameObject("LevelThreeTiles");

        ReadTileData();
        


        // create grass background
        Texture2D bitMap = Resources.Load("Maps/MapLayerOne") as Texture2D;
        GameObject background = new GameObject("background");
        background.transform.position = new Vector3(bitMap.width / 2 - background.transform.position.x, bitMap.height / 2 - background.transform.position.y, 100);
        SpriteRenderer sr = background.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.LoadAll<Sprite>("Textures/BuildingSpriteSheet")[120];
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(bitMap.width, bitMap.height);
        ReadImageToMap("Maps/MapLayerOne", tileDictLevelOne, 0);
        ReadImageToMap("Maps/MapLayerTwo", tileDictLevelTwo, 1);
        //ReadImageToMap("Maps/MapLayerThree", tileDictLevelThree, 2);

    }

    // Update is called once per frame
    void Update()
    {

    }


    void ReadTileData()
    {
        tileDictLevelOne = Json.JsonToContainer<SortedDictionary<string, MapTileData>>("tiledatalevelone.json");
        tileDictLevelTwo = Json.JsonToContainer<SortedDictionary<string, MapTileData>>("tiledataleveltwo.json");
        tileDictLevelThree = Json.JsonToContainer<SortedDictionary<string, MapTileData>>("tiledatalevelthree.json");

        foreach (var v in tileDictLevelOne)
            AllTiles.Add(v.Value.Name, v.Value);
        foreach (var v in tileDictLevelTwo)
            AllTiles.Add(v.Value.Name, v.Value);
        foreach (var v in tileDictLevelThree)
            Debug.Log(v.Key + v.Value.Name);
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
                if (level == 2)
                    Debug.Log(pixelColor);

                if (tileDict.TryGetValue(pixelColor.ToString(), out MapTileData tile) && pixelColor.ToString() != new Color(0, 0, 0).ToString())
                {
                    Sprite sprite = Resources.LoadAll<Sprite>(tile.Path)[tile.Index];
                    if (level == 2)
                        Debug.Log("CREATING NODE");
                    GameObject temp = new GameObject(tile.Name);
                    temp.transform.parent = tiles[level].transform;
                    temp.transform.position = new Vector3(x, y, 99 - level);
                    temp.AddComponent<SpriteRenderer>().sprite = sprite;
                    temp.layer = tile.LayerMask;
                    if (!string.IsNullOrEmpty(tile.Tag))
                        temp.tag = tile.Tag;

                    if (tile.Components != null)
                    {
                        foreach (var str in tile.Components)
                        {
                            switch (str.ToLower())
                            {
                                case "boxcollider":
                                    BoxCollider2D bc = temp.AddComponent<BoxCollider2D>();
                                    if (tile.BoxColliderScaleX > 0 && tile.BoxColliderScaleY > 0)
                                    {
                                        bc.size = new Vector2(tile.BoxColliderScaleX, tile.BoxColliderScaleY);
                                        bc.offset = new Vector2(tile.BoxColliderOffsetX, tile.BoxColliderOffsetY);
                                    }  
                                    break;
                                case "door":
                                    temp.AddComponent<Door>().AssignUnlockItems(new HashSet<uint> { ItemList.ITEM_LOCKPICK.UID });
                                    break;
                                case "searchablecontainer":
                                    temp.AddComponent<SearchableContainer>().AssignUnlockItems(new HashSet<uint> { ItemList.ITEM_LOCKPICK.UID });
                                    break;
                                case "vault":
                                    Door door = temp.AddComponent<Door>();
                                    door.timerMultiplier = 10f;
                                    break;
                                case "cashregister":
                                    temp.AddComponent<CashRegister>();
                                    break;
                                case "store":
                                    temp.AddComponent<Store>();
                                    break;
                                case "node":

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