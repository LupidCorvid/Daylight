using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player instance;
    public static PlayerMovement controller;

    public PlayerHealth playerHealth;
    public PlayerAttack pAttack;

    public bool hasLantern = false;
    public GameObject mouthLantern;

    public override bool attacking
    {
        get
        {
            if (controller == null)
                return false;

            return controller.attacking;
        }

        set
        {
            //Cannot be set
        }
    }


    public override Vector2 facingDir
    {
        get
        {
            return new Vector2(controller.facingRight ? 1 : -1, 0);
        }
        set
        {
            // Can't be set
        }
    }


    Player() : base()
    {
        maxHealth = 8;
        health = 8;
        allies = ITeam.Team.Player;
        enemies = ITeam.Team.Enemy;
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

        // Singleton design pattern
        if (instance != null && instance != this)
        {
            // Destroy(gameObject);
        }
        else
        {
            controller = GetComponent<PlayerMovement>();
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        
    }

    public override void Update()
    {
        instance = this;
        controller = GetComponent<PlayerMovement>();
        base.Update();
        

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
