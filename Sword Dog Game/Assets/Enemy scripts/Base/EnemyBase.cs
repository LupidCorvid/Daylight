using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int attackDamage = 1;
    public float moveSpeed = 1;
    public float attackSpeed = 1;

    public int maxHealth = 10;
    public int health = 10;

    public BaseAI ai;
    public BaseMovement movement;

    public SlopeAdjuster slopeChecker;

    public virtual void Start()
    {
        slopeChecker ??= GetComponent<SlopeAdjuster>();
        movement ??= GetComponent<BaseMovement>();
        if(movement != null)
            movement.slopeChecker ??= slopeChecker;
        ai.movement = movement;

        ai?.Start();
    }

    public virtual void Update()
    {
        ai?.Update();
    }

    public virtual void FixedUpdate()
    {
        ai?.FixedUpdate();
    }

    public void die()
    {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            die();
        GetComponent<SimpleFlash>()?.Flash(1f, 3, true);
    }
}
