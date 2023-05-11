using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ShopManager : BaseManager
{
    public GameObject contentHolder;
    public GameObject ItemPrefab;
    public List<ShopItemListing> CurrentListings = new List<ShopItemListing>();
    public int currentSelectedIndex = 0;

    public TextMeshProUGUI ItemNameDisplay;
    public TextMeshProUGUI ItemDescriptionDisplay;
    public TextMeshProUGUI ItemPriceDisplay;

    public Sprite selectedImage;
    public Sprite deselectedImage;

    public Sprite purchaseCostTooMuch;
    public Sprite purchaseSelected;
    public Sprite purchaseDeselected;

    public Button purchaseButton;
    public Image purchaseImage;

    public Sprite closeSelected;
    public Sprite closeDeselected;
    
    public Button closeButton;
    public Image closeImage;


    public TextMeshProUGUI currencyAmount;

    public void Awake()
    {
        //addNewItem(new ShopItem()
        //{
        //    name = "Tester1",
        //    description = "This is a tester",
        //    price = 0

        //});
        //addNewItem(new ShopItem()
        //{
        //    name = "Tester2",
        //    description = "This is a tester (again)",
        //    price = 4
        //});
        addNewItem(new SpiderTulipBulbShopListing());
        selectItem(CurrentListings[0]);
        CurrentListings[0].backgroundImage.sprite = selectedImage;
        currencyAmount.text = "" + InventoryManager.currInventory.CountItem(1);

        //purchaseButton.OnSelect += purchaseSelected;
        //purchaseButton.OnDeselect += purchaseDeselected;

        //closeButton.OnSelect += closeSelected;
        //closeButton.OnDeselect += closeDeselected;

    }




    public void addNewItem(ShopItem item)
    {
        GameObject addedItem = Instantiate(ItemPrefab, contentHolder.transform);
        ShopItemListing listing = addedItem.GetComponent<ShopItemListing>();
        listing.item = item;
        listing.mainManager = this;
        CurrentListings.Add(listing);
    }

    public void removeItem(ShopItem item)
    {
        for (int i = 0; i < CurrentListings.Count; i++)
        {
            if (CurrentListings[i].item.Equals(item))
            {
                Destroy(CurrentListings[i].gameObject);
                CurrentListings.RemoveAt(i);
            }
        }
    }

    public void selectItem(ShopItem item)
    {
        currentSelectedIndex = CurrentListings.FindIndex((x) => (x.item == item));

        ItemNameDisplay.text = item.name;
        ItemDescriptionDisplay.text = item.description;
        ItemPriceDisplay.text = "" + item.price;
        CheckIfAffordable();
    }

    public void selectItem(int listingID)
    {
        selectItem(CurrentListings[listingID]);
    }

    public void selectItem(ShopItemListing item)
    {

        //deselectClose();
        //deselectPurchase();

        selectItem(item.item);
    }

    public void ShopItemClicked(ShopItemListing item)
    {
        if (item == CurrentListings[currentSelectedIndex])
            return;

        item.backgroundImage.sprite = selectedImage;
        CurrentListings[currentSelectedIndex].backgroundImage.sprite = deselectedImage;


        selectItem(item);
        currentSelectedIndex = CurrentListings.FindIndex((x) => item == x);
    }


    public override void selectDown()
    {
        
        CurrentListings[currentSelectedIndex].backgroundImage.sprite = deselectedImage;

        currentSelectedIndex = ++currentSelectedIndex % CurrentListings.Count;
        
        CurrentListings[currentSelectedIndex].backgroundImage.sprite = selectedImage;
        selectItem(currentSelectedIndex);
        
    }

    public override void selectUp()
    {
        CurrentListings[currentSelectedIndex].backgroundImage.sprite = deselectedImage;
        
        currentSelectedIndex = --currentSelectedIndex % CurrentListings.Count;
        if (currentSelectedIndex < 0)
            currentSelectedIndex = CurrentListings.Count - 1;

        CurrentListings[currentSelectedIndex].backgroundImage.sprite = selectedImage;
        selectItem(currentSelectedIndex);
    }

    public void closedButtonClicked()
    {
        base.CloseMenu();
        manager.closeMenu();
    }

    public override void CloseMenu()
    {
        //Call cutscene to close it out
        CutsceneController.StopAllCutscenes?.Invoke();
        CutsceneController.PlayCutscene("CloseShop");
        Destroy(gameObject);
    }

    public void purchaseButtonClicked()
    {
        purchaseItem(CurrentListings[currentSelectedIndex].item);
        currencyAmount.text = "" + InventoryManager.currInventory.CountItem(1);
    }

    public void CheckIfAffordable()
    {
        if(CurrentListings[currentSelectedIndex].item.price <= InventoryManager.currInventory.CountItem(1))//Get player currency
        {
            //temp until better method of telling what is selected is found
            purchaseImage.sprite = purchaseSelected;
        }
        else
        {
            purchaseImage.sprite = purchaseCostTooMuch;
        }
    }

    public void purchaseItem(ShopItem item)
    {
        if (item.price <= InventoryManager.currInventory.CountItem(1))//Get player currency
        {
            InventoryManager.currInventory.RemoveItem(1, item.price);
            item.OnPurchase(PlayerMenuManager.main.player);
            //Remove cost from player currency count
        }
        CheckIfAffordable();
    }

    public void selectClose()
    {
        closeImage.sprite = closeSelected;
    }

    public void deselectClose()
    {
        closeImage.sprite = closeDeselected;
    }

    public void selectPurchase()
    {
        if(CurrentListings[currentSelectedIndex].item.price <= 0)
        {
            purchaseImage.sprite = purchaseSelected;
        }
    }

    public void deselectPurchase()
    {
        if (CurrentListings[currentSelectedIndex].item.price <= 0)
        {
            purchaseImage.sprite = purchaseDeselected;
        }
    }
}
