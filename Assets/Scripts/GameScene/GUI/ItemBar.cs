using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemBar : MonoBehaviour
{
    [SerializeField]
    GameObject item;
    [SerializeField]
    GameObject inventory;
    [SerializeField]
    GameObject bar;
    [SerializeField]
    GameObject arrow;
    private GameObject[,] inventoryItems = new GameObject[8,9];

    Vector3 firstItemPositionBar;
    Vector3 firstItemPositionInventory;
    Vector3 arrowOriginalPosition;
    Sprite originalItemSprite;
    GameObject SelectedItem;
    
    // Start is called before the first frame update
    void Start()
    {
        firstItemPositionBar = new Vector3(32+5, 32+5, 0);
        firstItemPositionInventory = inventory.GetComponent<RectTransform>().position + new Vector3(-inventory.GetComponent<RectTransform>().rect.width/2, inventory.GetComponent<RectTransform>().rect.height/2, 0);
        firstItemPositionInventory += new Vector3(32 + 5, -(32 + 5), 0);

        for (int i = 0; i < 8; i++)
        {
            inventoryItems[i, 0] = Instantiate(item, firstItemPositionBar + new Vector3((64+5)*i, 0, 0), Quaternion.identity, bar.transform);
            inventoryItems[i, 0].name = i + " " + 0;
        }

        arrow = Instantiate(arrow, inventoryItems[0, 0].transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity, bar.transform);
        arrowOriginalPosition = arrow.transform.position;
        arrow.name = "SelectedItemBorder";

        for (int x = 0; x < 8; x++)
        {
            for (int y = 1; y < 9; y++)
            {
                inventoryItems[x,y] = Instantiate(item, firstItemPositionInventory + new Vector3((64 + 5) * x, -(64 + 5) * (y-1), 0), Quaternion.identity, inventory.transform);
                inventoryItems[x,y].name = x + " " + (y);

            }
        }

        originalItemSprite = inventoryItems[7, 7].GetComponent<Image>().sprite;
    }
   
    public void AddItem(Vector2 position, Inventory.InventoryItem item)
    {
        var sprite = Resources.Load<Sprite>(item.ItemInfo.IconPath);
        if (sprite == null)
            Debug.LogError(item.ItemInfo.IconPath);
        inventoryItems[(int)position.x, (int)position.y].GetComponent<Image>().sprite = sprite;
        if(item.ItemInfo.InventoryStackSize > 1)
            inventoryItems[(int)position.x, (int)position.y].transform.GetChild(0).GetComponent<Text>().text = item.Count.ToString();
    }
    public void UpdateCount(Vector2 position, uint newCount)
    {
        inventoryItems[(int)position.x, (int)position.y].transform.GetChild(0).GetComponent<Text>().text = newCount.ToString();
    }

    public void RemoveItem(Vector2 position)
    {
        inventoryItems[(int)position.x, (int)position.y].GetComponent<Image>().sprite = originalItemSprite;
        inventoryItems[(int)position.x, (int)position.y].transform.GetChild(0).GetComponent<Text>().text = "";
    }

    public void OpenInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }

    public void UpdateCurrentItem(uint index)
    {
        arrow.transform.position = arrowOriginalPosition + new Vector3((64+5) * index, 0f, 0f);
    }
}
