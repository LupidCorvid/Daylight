using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<ItemSlot> contents = new List<ItemSlot>();

    public Inventory(int size = 3)
    {
        AddSlots(size);
        //AddItem(new TeardropAloe());
    }

    public void AddSlots(int num = 1)
    {
        for (int i = 0; i < num; i++)
        {
            contents.Add(new ItemSlot());
        }
    }

    public void AddFilledSlot(ItemSlot slot)
    {
        contents.Add(slot);
    }


    /// <returns>returns amount of item not added</returns>
    public int AddItem(Item item)
    {
        foreach(ItemSlot slot in contents)
        {
            if(slot.item == null)
            {
                slot.item = item;
                return 0;
            }

            if(slot.item.itemId == item.itemId)
            {
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
    /// <returns>Returns the number of items that couldnt be removed/didnt exist</returns>
    public int RemoveItem(Item item, int count)
    {
        foreach(ItemSlot slot in contents)
        {
            if(slot.item.itemId == item.itemId)
            {
                count = slot.RemoveAmount(count);

                if (count == 0)
                    return 0;
            }
        }
        return 0;
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
                total++;
        }
        return total;
    }

    public int CountItem(Item item)
    {
        return CountItem(item.itemId);
    }
}
