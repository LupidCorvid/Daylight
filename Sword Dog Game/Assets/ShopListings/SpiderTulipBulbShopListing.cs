using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTulipBulbShopListing : ShopItem
{
    public SpiderTulipBulbShopListing()
    {
        price = 2;
        name = "Spider tulip bulb";
        description = "Increases move speed temporarily on use";
        image = TempObjectsHolder.main.FindSprite("Items.SpiderTulipBud");
    }

    public override void OnPurchase(Entity purchaser)
    {
        int numDropped = purchaser.getAssociatedInventory().AddItem(new SpiderTulipBud());
        if (numDropped > 0)
            Utilities.main.SpawnLooseItem(new SpiderTulipBud(), purchaser.transform.position);
    }
}
