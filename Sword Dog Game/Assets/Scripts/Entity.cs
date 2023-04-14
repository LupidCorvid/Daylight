using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int attackDamage = 1;
    public float moveSpeed = 1;
    public float attackSpeed = 1;


    public int maxHealth = 8;
    public int health = 8;

    public bool invincible = false;

    [System.Flags]
    public enum Team
    {
        Player = 0b_01,
        Enemy = 0b_10
    }

    public Team allies;
    public Team enemies;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void heal()
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
        return (enemies & otherEntity.allies) > 0;
    }
}
