using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListing : MonoBehaviour
{
    public TMPro.TextMeshProUGUI countDisplay;
    public Image itemSprite;
    public Image bgSprite;
    //Should be a pointer to an item in inventory
    public Item attachedItem;

    public Sprite backgroundSprite;
    public Sprite backgroundCountSprite;


    public void UpdateAttachedItem(Item newItem)
    {
        attachedItem = newItem;
        if (newItem == null)
        {
            itemSprite.color = Color.clear;
            UpdateCount(0);
            return;
        }
        else
            itemSprite.color = Color.white;

        if(newItem?.sprite != "")
            itemSprite.sprite = TempObjectsHolder.main.FindSprite(newItem.sprite);
        
        UpdateCount(newItem.quantity);
    }

    public void UpdateCount(int count)
    {
        if (count <= 0)
            itemSprite.color = Color.clear;
        else
            itemSprite.color = Color.white;
        if (count <= 1)
        {
            countDisplay.text = "";
            bgSprite.sprite = backgroundSprite;
        }
        else
        {
            countDisplay.text = "" + count;
            bgSprite.sprite = backgroundCountSprite;
        }

    }
}
