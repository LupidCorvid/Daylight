using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public Item item;

    public void combineStack(ItemSlot otherItem)
    {
        item.combineStack(otherItem.item);
        if (item.quantity <= 0)
            item = null;
        if (otherItem.item.quantity <= 0)
            otherItem.item = null;
    }
}
