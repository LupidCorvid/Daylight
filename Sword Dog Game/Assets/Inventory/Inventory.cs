using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<ItemSlot> contents = new List<ItemSlot>();

    public System.Action<ItemSlot> itemChanged;
    public System.Action<ItemSlot, int> itemCountChanged;

    public Inventory(int size = 3)
    {
        AddSlots(size);
        //AddItem(new TeardropAloe());
    }

    public void changedItem(ItemSlot slot)
    {
        //temp fix, might not work on non-player inventories
        if (InventoryManager.currInventory == this)
            InventoryManager.main.UpdateItemDisplay(slot);
        else
            itemChanged?.Invoke(slot);
    }

    public void itemQuantityChanged(ItemSlot slot, int amount)
    {
        //temp fix, might not work on non-player inventories
        if (InventoryManager.currInventory == this)
            InventoryManager.main.UpdateItemCount(slot, amount);
        else
            itemCountChanged?.Invoke(slot, amount);
    }

    public void AddSlots(int num = 1)
    {
        for (int i = 0; i < num; i++)
        {
            contents.Add(new ItemSlot());
            contents[i].itemChanged += changedItem;
            contents[i].itemAmountChanged += itemQuantityChanged;
        }
    }

    public void AddFilledSlot(ItemSlot slot)
    {
        contents.Add(slot);
        contents[^1].itemChanged += changedItem;
        contents[^1].itemAmountChanged += itemQuantityChanged;
    }


    /// <returns>returns amount of item not added</returns>
    public int AddItem(Item item)
    {
        List<Item> matches = FindAll(item.itemId);
        while(matches.Count > 0)
        {
            for(int i = matches.Count - 1; i >= 0; i--)
            {
                matches[i].combineStack(item);
                if (item.quantity > 0)
                    matches.RemoveAt(i);
                else
                    return 0;
            }
        }

        foreach(ItemSlot slot in contents)
        {
            if(slot.item == null || slot.item.quantity <= 0)
            {
                slot.item = ItemDatabase.main.getItemFromId(item.itemId);
                slot.item.quantity = 0;
                slot.item.combineStack(item);
            }

            if (item.quantity == 0)
                return 0;
        }
        return item.quantity;
    }

    public void AddItems(List<Item> items)
    {
        foreach (Item item in items)
        {
            if (AddItem(item) > 0)
                Debug.Log("Couldn't fit " + item.name + " in inventory");
        }
    }

    public int RemoveItem(int id, int count)
    {
        foreach (ItemSlot slot in contents)
        {
            if (slot.item.itemId == id)
            {
                count = slot.RemoveAmount(count);

                if (count == 0)
                    return 0;
            }
        }
        return count;
    }

    /// <returns>Returns the number of items that couldnt be removed/didnt exist</returns>
    public int RemoveItem(Item item, int count)
    {
        //foreach(ItemSlot slot in contents)
        //{
        //    if(slot.item.itemId == item.itemId)
        //    {
        //        count = slot.RemoveAmount(count);

        //        if (count == 0)
        //            return 0;
        //    }
        //}
        //return count;
        return RemoveItem(item.itemId, count);
    }

    public int RemoveItem(Item item)
    {
        return RemoveItem(item, item.quantity);
    }

    public int CountItem(int id)
    {
        int total = 0;
        foreach(ItemSlot slot in contents)
        {
            if (slot?.item?.itemId == id)
                total += slot.item.quantity;
        }
        return total;
    }

    public int CountItem(Item item)
    {
        return CountItem(item.itemId);
    }

    public Item FindItem(int id)
    {
        return contents.Find((e) => e.item.itemId == id).item;
    }
    public List<Item> FindAll(int id)
    {
        List<Item> matches = new List<Item>();
        
        foreach(ItemSlot slot in contents)
        {
            if(slot?.item?.itemId == id)
            {
                matches.Add(slot.item);
            }
        }

        return matches;
    }
}
