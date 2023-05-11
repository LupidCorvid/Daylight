using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem
{
    public string name;
    public string description;
    public Sprite image;
    public int price;

    public virtual void OnPurchase(Entity purchaser)
    {

    }
}
