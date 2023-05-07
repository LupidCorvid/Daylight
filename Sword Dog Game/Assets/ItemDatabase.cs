using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase main;

    public List<System.Type> itemTypes = new List<System.Type>();

    private void Awake()
    {
        if (main != null && main != this)
            return;
        main = this;
        buildDatabase();
    }

    public void buildDatabase()
    {
        itemTypes.Clear();

        itemTypes.Add(typeof(TeardropAloe));
        itemTypes.Add(typeof(Bone));
    }

    public Item getItemFromId(int id)
    {
        return (Item)Activator.CreateInstance(itemTypes[id]);
    }

    public Item unpackItem(PackedItem item)
    {
        if (item.itemId == -1)
            return null;
        Item unpackedItem = getItemFromId(item.itemId);
        unpackedItem.quantity = item.itemCount;
        return unpackedItem;
    }

    public List<Item> unpackItems(List<PackedItem> packedItems)
    {
        List<Item> unpackedItems = new List<Item>();

        foreach(PackedItem item in packedItems)
        {
            Item unpackedItem = getItemFromId(item.itemId);
            unpackedItem.quantity = item.itemCount;
            unpackedItems.Add(unpackedItem);
        }
        return unpackedItems;
    }

    public Inventory unpackInventory(PackedInventory inventory)
    {
        Inventory newInventory = new Inventory(0);

        foreach(PackedSlot slot in inventory.contents)
        {
            ItemSlot slotToAdd;
            if (slot.item == null)
                slotToAdd = new ItemSlot(null);
            else
                slotToAdd = new ItemSlot(unpackItem(slot.item));
            newInventory.AddFilledSlot(slotToAdd);
        }
        return newInventory;
        //return unpackItems(inventory.contents);
    }

    public List<PackedItem> packItems(List<Item> items)
    {
        List<PackedItem> packedItems = new List<PackedItem>();

        foreach (Item item in items)
        {
            
            packedItems.Add(new PackedItem(item.itemId, item.quantity));
        }
        return packedItems;
    }

    public PackedInventory packInventory(Inventory items)
    {
        List<PackedSlot> packedItems = new List<PackedSlot>();

        foreach (ItemSlot item in items.contents)
        {
            if (item?.item != null)
                packedItems.Add(new PackedSlot(new PackedItem(item.item.itemId, item.item.quantity)));
            else
                packedItems.Add(new PackedSlot(null));
        }
        return new PackedInventory(packedItems);
    }

    [Serializable]
    public class PackedItem
    {
        public int itemId = -1;
        public int itemCount;

        public PackedItem(int itemId, int itemCount)
        {
            this.itemId = itemId;
            this.itemCount = itemCount;
        }

        public PackedItem()
        {
            itemId = -1;
            itemCount = 0;
        }

    }
    [Serializable]
    public class PackedSlot
    {
        public PackedItem item;

        public PackedSlot(PackedItem item)
        {
            this.item = item;
        }
    }

    [Serializable]
    public class PackedInventory
    {
        public List<PackedSlot> contents = new List<PackedSlot>();

        public PackedInventory(List<PackedSlot> items)
        {
            contents = items;
        }

        public PackedInventory()
        {
            contents = new List<PackedSlot>(3);
        }
    }
}
