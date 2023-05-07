using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeardropAloe : Item
{

    public TeardropAloe()
    {
        itemId = 0;
        name = "Teardrop Aloe";
        description = "Has healing properties";
        sellValue = 2;
        quantity = 1;
        sprite = "Items.TeardropAloe";
        stackSize = 10;
    }

    public override void OnUse(Entity User)
    {
        quantity--;
        //Debug.Log("Used Aloe!");
        User.heal(1);
    }
}
