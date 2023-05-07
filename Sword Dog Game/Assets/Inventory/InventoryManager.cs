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
        if (listingHolder == null)
            return;

        for(int i = 0; i < listingHolder.transform.childCount; i++)
        {
            Destroy(listingHolder.transform.GetChild(i).gameObject);
        }
        itemListings.Clear();

        for(int i = 0; i < currInventory.contents.Count; i++)
        {
            itemListings.Add(Instantiate(itemListingPrefab, listingHolder.transform).GetComponent<ItemListing>());
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
        itemListings[currInventory.contents.FindIndex((e) => (e == slot))].UpdateAttachedItem(slot.item);
    }

    public void UpdateItemCount(ItemSlot slot, int amount)
    {
        itemListings[currInventory.contents.FindIndex((e) => (e == slot))].UpdateCount(amount);
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        refreshInventory();
        GameSaver.loadedNewData += ((e) => refreshInventory());

        currInventory.itemChanged += UpdateItemDisplay;
        currInventory.itemCountChanged += UpdateItemCount;
        //AddItem(new TeardropAloe());
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if(currInventory.contents.Count < 3)
    //    {
    //        currInventory.AddSlots(1);
    //    }
    //}
}
