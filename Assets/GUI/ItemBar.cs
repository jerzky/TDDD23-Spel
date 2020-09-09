using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemBar : MonoBehaviour
{
    [SerializeField]
    GameObject item;
    [SerializeField]
    GameObject inventory;
    [SerializeField]
    GameObject bar;
    private GameObject[] barItems = new GameObject[8];
    private GameObject[,] inventoryItems = new GameObject[8,8];

    Vector3 firstItemPositionBar;
    Vector3 firstItemPositionInventory;

    // Start is called before the first frame update
    void Start()
    {
        firstItemPositionBar = new Vector3(32+5, 32+5, 0);
        firstItemPositionInventory = inventory.GetComponent<RectTransform>().position + new Vector3(-inventory.GetComponent<RectTransform>().rect.width/2, inventory.GetComponent<RectTransform>().rect.height/2, 0);
        firstItemPositionInventory += new Vector3(32 + 5, -(32 + 5), 0);

        for (int i = 0; i < 8; i++)
        {
            barItems[i] = Instantiate(item, firstItemPositionBar + new Vector3((64+5)*i, 0, 0), Quaternion.identity, bar.transform);
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                inventoryItems[x,y] = Instantiate(item, firstItemPositionInventory + new Vector3((64 + 5) * x, -(64 + 5) * y, 0), Quaternion.identity, inventory.transform);
            }
        }
    }
   
    public void AddItem(uint index, ItemInfo info)
    {
        Debug.Log("Test");
        var sprite = Resources.Load<Sprite>(info.IconPath);
        if (sprite == null)
            Debug.Log(info.IconPath);
        barItems[index].GetComponent<Image>().sprite = sprite;
    }
    public void UpdateCount(uint index, int newCount )
    {
        barItems[index].transform.GetChild(1).GetComponent<Text>().text = newCount.ToString();
    }

    public void OpenInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }
}
