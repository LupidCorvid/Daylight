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
    }

    public override void OnUse()
    {
        quantity--;
        
    }
}
