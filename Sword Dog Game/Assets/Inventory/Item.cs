using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{

    public int itemId;
    public string name;
    public int quantity;
    public Sprite sprite;
    public int stackSize;

    /// <returns>The amount remaining in otherItem</returns>
    public int combineStack(Item otherItem)
    {
        quantity += otherItem.quantity;
        int overMaxStack = quantity - stackSize;
        if (overMaxStack > 0)
        {
            quantity = stackSize;
            otherItem.quantity = overMaxStack;
        }
        else
            otherItem.quantity = 0;
        return otherItem.quantity;
    }

}
