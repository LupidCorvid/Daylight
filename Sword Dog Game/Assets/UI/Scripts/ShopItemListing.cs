using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemListing : MonoBehaviour
{
    public ShopManager mainManager;

    [SerializeField]
    Image itemImage;
    [SerializeField]
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
            if(_item.image != null)
                itemImage.sprite = item.image;
            text.text = item.name;
        }
    }

    public void OnClick()
    {
        mainManager.ShopItemClicked(this);
    }



}
