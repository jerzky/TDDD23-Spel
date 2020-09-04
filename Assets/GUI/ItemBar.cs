using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemBar : MonoBehaviour
{
    [SerializeField]
    GameObject item;
    private GameObject[] items = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            items[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }
   
    public void AddItem(uint index, Inventory.ItemInfo info)
    {
        Debug.Log("Test");
        var sprite = Resources.Load<Sprite>(info.IconPath);
        if (sprite == null)
            Debug.Log(info.IconPath);
        items[index].GetComponent<Image>().sprite = sprite;
    }
    public void UpdateCount(uint index, int newCount )
    {
        items[index].transform.GetChild(1).GetComponent<Text>().text = newCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
