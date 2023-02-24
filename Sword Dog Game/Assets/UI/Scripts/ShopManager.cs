using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManager : BaseManager
{
    public GameObject contentHolder;
    public GameObject ItemPrefab;
    public List<ShopItemListing> CurrentListings = new List<ShopItemListing>();

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
    }


    public void addNewItem(ShopItem item)
    {
        GameObject addedItem = Instantiate(ItemPrefab, contentHolder.transform);
        ShopItemListing listing = addedItem.GetComponent<ShopItemListing>();
        listing.item = item;
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

    }

    public void ShopItemClicked(ShopItemListing item)
    {
        selectItem(item.item);
    }
}
