using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    List<ItemSlot> _contents = new List<ItemSlot>();

    public List<ItemSlot> contents
    {
        get
        {
            return _contents;
        }
    }

    public Inventory(int size = 3)
    {
        AddSlots(size);
    }
    public void AddSlots(int num = 1)
    {
        for (int i = 0; i < num; i++)
        {
            _contents.Add(new ItemSlot());
        }
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
        }
        return item.quantity;
    }
}
