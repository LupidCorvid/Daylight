using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public PlayerHealth playerHealth;

    Player() : base()
    {
        maxHealth = 8;
        health = 8;
        allies = Team.Player;
        enemies = Team.Enemy;
        //GameSaver.loadedNewData += loadSavedBuffs;
    }

    public void Awake()
    {
        //seems like this is added too late
        
    }

    public override void Start()
    {
        base.Start();
        buffManager.loadBuffs(GameSaver.currData);
    }

    public override Inventory getAssociatedInventory()
    {
        return InventoryManager.currInventory;
    }

    public override void TakeDamage(int damage, Entity source)
    {
        base.TakeDamage(damage, source);
        playerHealth.TakeDamage(damage);
    }

    public override void heal(int amount)
    {
        playerHealth.Heal(amount);
    }

    public override GameObject addBuffDisplay(int buffID, GameObject buffObj)
    {
        return BuffList.main.instantiateAndAddBuffIcon(buffID, buffObj);
    }

    public override void removeBuffDisplay(int buffID)
    {
        BuffList.main.removeBuffIcon(buffID);
    }

    public override void OnDestroy()
    {
        GameSaver.loadedNewData -= loadSavedBuffs;
    }

    public void loadSavedBuffs(GameSaver.SaveData data)
    {
        if (this != null && gameObject != null)
            buffManager.loadBuffs(data);
        else
            GameSaver.loadedNewData -= loadSavedBuffs;
    }
}
