using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
//[JsonDerivedType(typeof(Item), typeDiscriminator: "item")]
//[JsonDerivedType(typeof(TeardropAloe), typeDiscriminator: "TeardropAloe")]
public class Item
{

    public int itemId = -1;
    public string name;
    public string description;
    public int _quantity;
    public int quantity
    {
        get { return _quantity; }
        set
        {
            amountChanged?.Invoke(value);
            _quantity = value;
        }
    }
    public string sprite;
    public int stackSize;
    public int sellValue;

    public Action<int> amountChanged;

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

    public virtual void OnUse(Entity user)
    {

    }

}
