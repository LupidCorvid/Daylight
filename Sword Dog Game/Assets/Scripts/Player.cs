using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public PlayerHealth playerHealth;

    Player()
    {
        maxHealth = 8;
        health = 8;
        allies = Team.Player;
        enemies = Team.Enemy;
    }

    public override void TakeDamage(int damage)
    {
        playerHealth.TakeDamage(damage);
    }

    public override GameObject addBuffDisplay(int buffID, GameObject buffObj)
    {
        return BuffList.main.instantiateAndAddBuffIcon(buffID, buffObj);
    }

    public override void removeBufffDisplay(int buffID)
    {
        BuffList.main.removeBuffIcon(buffID);
    }
}
