using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyBase : MonoBehaviour
{
    public int attackDamage = 1;
    public float moveSpeed = 1;
    public float attackSpeed = 1;

    public int maxHealth = 10;
    public int health = 10;

    public float aggroRange = 10;

    public BaseAI ai;
    public BaseMovement movement;

    public SlopeAdjuster slopeChecker;

    public Animator anim;

    public Action killed;

    public virtual void Start()
    {
        slopeChecker ??= GetComponent<SlopeAdjuster>();
        movement ??= GetComponent<BaseMovement>();
        if(movement != null)
            movement.slopeChecker ??= slopeChecker;
        if(ai != null)
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
        killed?.Invoke();
        Destroy(gameObject);
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            die();
        GetComponentInChildren<SimpleFlash>()?.Flash(1f, 3, true);
    }
}
