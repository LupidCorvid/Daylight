using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemListing : MonoBehaviour
{
    public ShopManager mainManager;

    Image itemImage;
    TextMeshProUGUI text;
    private ShopItem _item;

    public ShopItem item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
                return;
            itemImage.sprite = item.image;
            text.text = item.name;
        }
    }

    public void OnMouseDown()
    {
        mainManager.ShopItemClicked(this);
    }



}
