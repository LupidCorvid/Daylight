using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Inventory currInventory = new Inventory();
    public GameObject itemListingPrefab;
    public GameObject listingHolder;
    public List<ItemListing> itemListings = new List<ItemListing>();
    public static InventoryManager main;

    public void refreshInventory()
    {
        if (listingHolder == null && main == this)
        {
            Debug.LogError("Something is weird");
            return;
        }

        for(int i = listingHolder.transform.childCount - 1; i >= 0 ; i--)
        {
            Destroy(listingHolder.transform.GetChild(i).gameObject);
        }
        itemListings.Clear();

        for(int i = 0; i < currInventory.contents.Count; i++)
        {
            GameObject addedObject = Instantiate(itemListingPrefab, listingHolder.transform);
            itemListings.Add(addedObject.GetComponent<ItemListing>());
        }

        UpdateItemDisplays();
    }

    public int AddItem(Item item)
    {
        return currInventory.AddItem(item);
        //Update all item counts and sprites
    }

    public void AddItems(List<Item> items)
    {
        currInventory.AddItems(items);
    }

    public void UpdateItemDisplays()
    {
        for(int i = 0; i < currInventory.contents.Count; i++)
        {
            itemListings[i].UpdateAttachedItem(currInventory.contents[i].item);
        }
    }
    public void UpdateItemDisplay(ItemSlot slot)
    {
        itemListings[currInventory.contents.IndexOf(slot)].UpdateAttachedItem(slot.item);
    }

    public void UpdateItemCount(ItemSlot slot, int amount)
    {
        itemListings[currInventory.contents.IndexOf(slot)].UpdateCount(amount);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (main == null)
            main = this;

        if (main != this)
            return;
        refreshInventory();
        GameSaver.loadedNewData += ((e) => refreshInventory());

        currInventory.itemChanged += UpdateItemDisplay;
        currInventory.itemCountChanged += UpdateItemCount;
        //AddItem(new TeardropAloe());
    }
}
