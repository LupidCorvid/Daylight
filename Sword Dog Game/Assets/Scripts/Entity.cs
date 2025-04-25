using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IParryable, ITeam
{
    public ModifiableValue attackDamage = new ModifiableValue(1);
    public ModifiableValue moveSpeed = new ModifiableValue(1);
    public ModifiableValue attackSpeed = new ModifiableValue(1);

    public Transform emotePosition;

    public int maxHealth = 8;
    public int health = 8;

    public bool invincible = false;

    public Action killed;
    public Action<Entity> damaged;

    private bool _stunned = false;

    public virtual bool stunned
    {
        get
        {
            return _stunned;
        }

        set
        {
            _stunned = value;
        }
    }

    private bool _attacking = false;
    public virtual bool attacking
    {
        get
        {
            return _attacking;
        }
        set
        {
            _attacking = value;
        }
    }

    public bool canBeParried
    {
        get { return attacking; }
        set { attacking = value; }
    }

    //[System.Flags]
    //public enum Team
    //{
    //    Player = 0b_01,
    //    Enemy = 0b_10
    //}

    private ITeam.Team _allies;
    private ITeam.Team _enemies;

    public ITeam.Team allies
    {
        get { return _allies; }
        set { _allies = value; }
    }

    public ITeam.Team enemies
    {
        get { return _enemies; }
        set { _enemies = value; }
    }

    //#region buffs
    //public SporedDebuff spored;
    //public MoveSpeedBuff moveSpeedBuff;
    //#endregion

    public Buffs buffManager;

    private Vector2 _facingDir;

    public virtual Vector2 facingDir
    {
        get { return _facingDir; }
        set { _facingDir = value; }
    }

    public bool freezeAI = false;

    public virtual Inventory getAssociatedInventory()
    {
        return null;
    }

    public Entity()
    {
        buffManager = new Buffs(this);
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        //buffManager = new Buffs(this);
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

    public virtual void removeBuffDisplay(int buffID)
    {
        
    }

    public virtual void TakeDamage(int damage, Entity source)
    {
        damaged?.Invoke(source);
    }

    public virtual void Die()
    {

    }

    public bool GetIfEnemies(Entity otherEntity)
    {
        return ((enemies & otherEntity.allies) > 0 && otherEntity != this);
    }

    public virtual void OnDestroy()
    {
        
    }

    public virtual void Parried(SwordFollow parriedBy)
    {
        

    }

    public virtual void Blocked(SwordFollow parriedBy)
    {

    }

}
