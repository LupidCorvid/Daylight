using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyBase : Entity
{

    public float aggroRange = 10;

    public BaseAI ai;
    public BaseMovement movement;

    public SlopeAdjuster slopeChecker;

    public Animator anim;

    public Action killed;

    public override void Start()
    {
        base.Start();
        slopeChecker ??= GetComponent<SlopeAdjuster>();
        movement ??= GetComponent<BaseMovement>();
        if(movement != null)
            movement.slopeChecker ??= slopeChecker;
        if(ai != null)
            ai.movement = movement;

        ai?.Start();

        allies = Team.Enemy;
        enemies = Team.Player;
    }

    public override void Update()
    {
        base.Update();
        ai?.Update();
    }

    public virtual void FixedUpdate()
    {
        ai?.FixedUpdate();
    }

    public virtual void LateUpdate()
    {
        ai?.LateUpdate();
    }

    public void die()
    {
        killed?.Invoke();
        Destroy(gameObject);
    }

    public override void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            die();
        GetComponentInChildren<SimpleFlash>()?.Flash(1f, 3, true);
    }
}
