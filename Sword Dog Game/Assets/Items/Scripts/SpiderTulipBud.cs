using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTulipBud : Item
{
    public SpiderTulipBud()
    {
        itemId = 2;
        name = "Spider Tulip Bud";
        quantity = 1;
        sellValue = 3;
        stackSize = 10;
        sprite = "Items.SpiderTulipBud";
    }

    public override void OnUse(Entity user)
    {
        user.buffManager.moveSpeedBuff.Inflict(60, 0.25f);
        quantity--;
    }
}
