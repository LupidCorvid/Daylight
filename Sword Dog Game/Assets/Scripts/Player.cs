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
}
