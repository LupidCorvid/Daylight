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

    public TMPro.TextMeshProUGUI NameText;
    public Image nameBacking;

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
        {
            //itemSprite.color = Color.white;
            //nameBacking.color = Color.white;
            //NameText.text = newItem.name;
        }

        if(newItem?.sprite != "")
            itemSprite.sprite = TempObjectsHolder.main.FindSprite(newItem.sprite);
        NameText.font = FontManager.main.currFont;

        UpdateCount(newItem.quantity);
    }


    public void UpdateCount(int count)
    {
        if (count <= 0)
        {
            itemSprite.color = Color.clear;
            nameBacking.color = Color.clear;
            NameText.text = "";
        }
        else
        {
            itemSprite.color = Color.white;
            nameBacking.color = Color.white;
            NameText.text = attachedItem.name;
        }

        if (count <= 1)
        {
            countDisplay.text = "";
            bgSprite.sprite = backgroundSprite;
        }
        else
        {
            countDisplay.text = "" + count;
            //Debug.Log(countDisplay.gameObject);
            bgSprite.sprite = backgroundCountSprite;

            countDisplay.font = FontManager.main.currFont;
        }

    }

    public void useItem()
    {
        //if (attachedItem != null && attachedItem.quantity > 0)
        //    attachedItem.OnUse(PlayerMenuManager.main.player);
        InventoryManager.main.itemInfoPopup.close();
        if (attachedItem == null || attachedItem.quantity <= 0)
        {
            return;
        }
        RectTransform rect = GetComponent<RectTransform>();
        InventoryManager.main.itemInfoPopup.fromListing = this;
        InventoryManager.main.itemInfoPopup.selectItem(attachedItem, transform.position + Vector3.right * rect.sizeDelta.x);
    }
}
