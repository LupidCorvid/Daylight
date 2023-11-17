using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItemListing : MonoBehaviour, ISelectHandler
{
    public ShopManager mainManager;

    [SerializeField]
    Image itemImage;
    [SerializeField]
    TextMeshProUGUI text;
    private ShopItem _item;

    public Button button;
    public Image backgroundImage;

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

    private void Awake()
    {
        ChangeFont();
    }
    public void ChangeFont()
    {
        text.font = FontManager.main.currFont;
    }

    public void OnClick()
    {
        mainManager.ShopItemSelected(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        mainManager.ShopItemSelected(this);
    }

    //public void OnSelect()
    //{
    //    backgroundImage.sprite = selectedImage;
    //}

    //public void OnDeselect()
    //{
    //    backgroundImage.sprite = unselectedImage;
    //}



}
