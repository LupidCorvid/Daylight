using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    Item _item;
    public Item item
    {
        get
        {
            return _item;
        }
        set
        {
            itemChanged?.Invoke(this);
            if(_item != null)
                _item.amountChanged -= quantityChanged;
            _item = value;
            if(_item != null)
                _item.amountChanged += quantityChanged;
            itemChanged?.Invoke(this);
        }
    }

    public System.Action<ItemSlot> itemChanged;
    public System.Action<ItemSlot, int> itemAmountChanged;

    public void quantityChanged(int amount)
    {
        itemAmountChanged?.Invoke(this, amount);
    }

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
