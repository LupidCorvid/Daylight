using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    public ModifiableValue attackDamage = new ModifiableValue(1);
    public ModifiableValue moveSpeed = new ModifiableValue(1);
    public ModifiableValue attackSpeed = new ModifiableValue(1);


    public int maxHealth = 8;
    public int health = 8;

    public bool invincible = false;

    public Action killed;

    [System.Flags]
    public enum Team
    {
        Player = 0b_01,
        Enemy = 0b_10
    }

    public Team allies;
    public Team enemies;

    //#region buffs
    //public SporedDebuff spored;
    //public MoveSpeedBuff moveSpeedBuff;
    //#endregion

    public Buffs buffManager;

    public virtual Inventory getAssociatedInventory()
    {
        return null;
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        buffManager = new Buffs(this);
        //#region buffs
        //spored = new SporedDebuff(this);
        //moveSpeedBuff = new MoveSpeedBuff(this);
        //#endregion
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //#region buffs
        //spored?.Update();
        //#endregion
        buffManager.Update();
        
    }

    public virtual void heal()
    {

    }

    public virtual void heal(int amount)
    {
        health += amount;

        if (health <= 0)
            Die();

        if (health > maxHealth)
            health = maxHealth;
    }

    public virtual GameObject addBuffDisplay(int buffID, GameObject buffObj)
    {
        return null;
    }

    public virtual void removeBufffDisplay(int buffID)
    {
        
    }

    public virtual void TakeDamage(int damage)
    {

    }

    public virtual void Die()
    {

    }

    public bool GetIfEnemies(Entity otherEntity)
    {
        return ((enemies & otherEntity.allies) > 0 && otherEntity != this);
    }
}
