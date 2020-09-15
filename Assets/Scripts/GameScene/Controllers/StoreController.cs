using Assets.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    public static StoreController Instance;
    [SerializeField]
    GameObject header;
    [SerializeField]
    GameObject scrollbar;
    [SerializeField]
    GameObject storeTextHolder;
    [SerializeField]
    GameObject dot;
    float dotDistance;

    Text[,] storeTexts;
    ItemInfo[] allItems;
    int currentTopIndex = 0;

    bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        allItems = new ItemInfo[ItemList.AllItems.Count];
        int i = 0;
        foreach(var v in ItemList.AllItems)
        {
            allItems[i++] = v.Value;
        }
        Button[] scrollButtons = scrollbar.GetComponentsInChildren<Button>();
        float upDownDistance = scrollButtons[0].transform.position.y - scrollButtons[1].transform.position.y;
        float offSetDistance = scrollButtons[0].transform.position.y - dot.transform.position.y;
        dotDistance = upDownDistance - offSetDistance*2;

        Image[] itemTextImages = storeTextHolder.GetComponentsInChildren<Image>();
        storeTexts = new Text[3, 6];
        int x = 0;
        int y = 0;
        foreach(var v in itemTextImages)
        {
            if (y >= allItems.Length || y > 5)
                break;
            storeTexts[x, y] = v.GetComponentInChildren<Text>();
            if (x == 0)
                storeTexts[x, y].text = allItems[y].Name;
            else if(x == 1)
                storeTexts[x, y].text = "$" + allItems[y].BuyPrice;
            else if (x == 2)
                storeTexts[x, y].text = "$" + allItems[y].SellPrice;
            x++;
            if (x > 2)
            {
                x = 0;
                y++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Scroll(int dir)
    {
        if ((currentTopIndex <= 0 && dir < 0) || (currentTopIndex >= allItems.Length-6 && dir > 0))
            return;
        
        float changeDistance = dotDistance / (allItems.Length-6);
        dot.transform.position -= new Vector3(0f, changeDistance * dir, 0f);
        currentTopIndex += dir;

        for (int i = 0; i < storeTexts.GetLength(1); i++)
        {
            storeTexts[0, i].text = allItems[i + currentTopIndex].Name;
            storeTexts[1, i].text = "$" + allItems[i + currentTopIndex].BuyPrice;
            storeTexts[2, i].text = "$" + allItems[i + currentTopIndex].SellPrice;
        }
    }

    public void Buy()
    {
        int clicked = 0;
        Vector3 mouse = Input.mousePosition;
        for (int i = 0; i < storeTexts.GetLength(1); i++)
        {
            if (i >= allItems.Length)
                break;

            if (Vector2.Distance(storeTexts[1, i].transform.position, mouse) < Vector2.Distance(storeTexts[1, clicked].transform.position, mouse))
            {
                clicked = i;
            }
        }
        clicked += currentTopIndex;
        ItemInfo boughtItem = allItems[clicked];
        if (GeneralUI.Instance.Credits >= boughtItem.BuyPrice)
        {
            GeneralUI.Instance.Credits -= (int)boughtItem.BuyPrice;
            Inventory.Instance.AddItem(boughtItem.UID, boughtItem.PurchaseAmount);
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/ChaChing2"), PlayerController.Instance.transform.position);
        }
    }

    public void Sell()
    {
        int clicked = 0;
        Vector3 mouse = Input.mousePosition;
        for (int i = 0; i < storeTexts.GetLength(1); i++)
        {
            if (i >= allItems.Length)
                break;
            
            if (Vector2.Distance(storeTexts[2, i].transform.position, mouse) < Vector2.Distance(storeTexts[2, clicked].transform.position, mouse))
            {
                clicked = i;
            }
        }
        clicked += currentTopIndex;
        ItemInfo soldItem = allItems[clicked];
        if(Inventory.Instance.GetItemCount(soldItem.UID) >= soldItem.PurchaseAmount)
        {
            Inventory.Instance.RemoveItem(soldItem.UID, soldItem.PurchaseAmount);
            GeneralUI.Instance.Credits += (int)soldItem.SellPrice;
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/ChaChing1"), PlayerController.Instance.transform.position);
        }
    }

    public void Toggle(bool b)
    {
        isOpen = b;
        header.SetActive(b);
        scrollbar.SetActive(b);
        storeTextHolder.SetActive(b);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
