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
        arrowOriginalPosition = arrow.transform.position;
        firstItemPositionBar = new Vector3(32+5, 32+5, 0);
        firstItemPositionInventory = inventory.GetComponent<RectTransform>().position + new Vector3(-inventory.GetComponent<RectTransform>().rect.width/2, inventory.GetComponent<RectTransform>().rect.height/2, 0);
        firstItemPositionInventory += new Vector3(32 + 5, -(32 + 5), 0);

        for (int i = 0; i < 8; i++)
        {
            inventoryItems[i, 0] = Instantiate(item, firstItemPositionBar + new Vector3((64+5)*i, 0, 0), Quaternion.identity, bar.transform);
            inventoryItems[i, 0].name = i + " " + 0;
        }

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
   
    public void AddItem(Vector2 position, ItemInfo info)
    {
        Debug.Log("Test");
        var sprite = Resources.Load<Sprite>(info.IconPath);
        if (sprite == null)
            Debug.Log(info.IconPath);
        inventoryItems[(int)position.x, (int)position.y].GetComponent<Image>().sprite = sprite;


        Debug.Log("Item Added: " + info.UID);
    }
    public void UpdateCount(Vector2 position, uint newCount)
    {
        inventoryItems[(int)position.x, (int)position.y].transform.GetChild(0).GetComponent<Text>().text = newCount.ToString();

        Debug.Log("Item Edited at position: " + position + " NewCount: " + newCount);
    }

    public void RemoveItem(Vector2 position)
    {

        inventoryItems[(int)position.x, (int)position.y].GetComponent<Image>().sprite = originalItemSprite;
        inventoryItems[(int)position.x, (int)position.y].transform.GetChild(0).GetComponent<Text>().text = "";

        Debug.Log("Item fully removed at position: " + position);
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
