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

    public int killValue = 5;
    public SoundPlayer soundPlayer;
    public SoundClip crySound;
    

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
        Utilities.main.SpawnLooseItem(new Bone(killValue), transform.position, new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(1f, 3f)));

    }

    public override void TakeDamage(int amount, Entity source)
    {
        base.TakeDamage(amount, source);
        health -= amount;
        if (health <= 0)
            die();
        GetComponentInChildren<SimpleFlash>()?.Flash(1f, 3, true);
    }

    public void cry()
    {
        soundPlayer?.PlaySound(crySound, 0.5f);
    }

    public override void Parried(SwordFollow by)
    {
        base.Parried(by);
        buffManager.stunned.Inflict(2, 1);
    }
}
