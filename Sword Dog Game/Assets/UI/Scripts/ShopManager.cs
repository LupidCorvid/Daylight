using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ShopManager : BaseManager
{
    public GameObject contentHolder;
    public GameObject ItemPrefab;
    public List<ShopItemListing> CurrentListings = new List<ShopItemListing>();
    public int currentSelectedIndex = 0;

    public TextMeshProUGUI ItemNameDisplay;
    public TextMeshProUGUI ItemDescriptionDisplay;
    public TextMeshProUGUI ItemPriceDisplay;

    public void Awake()
    {
        addNewItem(new ShopItem()
        {
            name = "Tester1",
            description = "This is a tester",
            price = 0

        });
        addNewItem(new ShopItem()
        {
            name = "Tester2",
            description = "This is a tester (again)",
            price = 4
        });
        selectItem(CurrentListings[0].item);
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
        ItemNameDisplay.text = item.name;
        ItemDescriptionDisplay.text = item.description;
        ItemPriceDisplay.text = "" + item.price;
    }

    public void selectItem(int listingID)
    {
        selectItem(CurrentListings[listingID].item);
    }

    public void selectItem(ShopItemListing item)
    {
        selectItem(item.item);
    }

    public void ShopItemClicked(ShopItemListing item)
    {
        selectItem(item);
        currentSelectedIndex = CurrentListings.FindIndex((x) => item == x);
    }


    public override void selectDown()
    {
        currentSelectedIndex = ++currentSelectedIndex % CurrentListings.Count;
        selectItem(currentSelectedIndex);
        
    }

    public override void selectUp()
    {
        currentSelectedIndex = --currentSelectedIndex % CurrentListings.Count;
        if (currentSelectedIndex < 0)
            currentSelectedIndex = CurrentListings.Count - 1;
        selectItem(currentSelectedIndex);
    }

    public void closedButtonClicked()
    {
        CloseMenu();
    }

    public override void CloseMenu()
    {
        //Call cutscene to close it out
        CutsceneController.PlayCutscene("CloseShop");
        Destroy(gameObject);
        MenuManager.inMenu = false;
        //Have shopkeep say bye
    }

    public void purchaseButtonClicked()
    {
        purchaseItem(CurrentListings[currentSelectedIndex].item);
    }

    public void purchaseItem(ShopItem item)
    {
        item.OnPurchase();
        //Remove cost from player currency count
    }
}
