using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : Item
{
    public Bone(int amount)
    {
        itemId = 1;
        name = "Bone";
        description = "Can be exchanged for goods and services.";
        sellValue = -1;//Can't be sold
        sprite = "Items.Bone";
        quantity = amount;
        stackSize = 99;
    }
    public Bone()
    {
        itemId = 1;
        name = "Bone";
        description = "Can be exchanged for goods and services.";
        sellValue = -1;//Can't be sold
        sprite = "Items.Bone";
        quantity = 1;
        stackSize = 99;
    }
}
