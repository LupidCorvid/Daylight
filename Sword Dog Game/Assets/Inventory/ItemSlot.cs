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

    /// <returns>Returns number of what couldn't be removed (!0 if removed more than there is)</returns>
    public int RemoveAmount(int amount)
    {
        item.quantity -= amount;
        int removedAmount = 0;
        if (item.quantity < 0)
        {
            removedAmount = item.quantity * -1;
            item.quantity = 0;
        }

        return removedAmount;
    }

    public ItemSlot()
    {

    }

    public ItemSlot(Item item)
    {
        this.item = item;
    }
}
