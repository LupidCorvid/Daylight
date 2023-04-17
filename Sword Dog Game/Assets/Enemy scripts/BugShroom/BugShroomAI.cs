using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugShroomAI : BaseAI
{
    public float lastAttack = -100;

    //public List<PlayerHealth> hitThisFrame = new List<PlayerHealth>();
    //public List<hitTarget> hitTargets = new List<hitTarget>();


    public float attackCooldown
    {
        get
        {
            if (enemyBase.attackSpeed == 0)
                return 999999;
            return 1 / enemyBase.attackSpeed;
        }
    }

    public float stopRange = 6;

    public enum AIState
    {
        idle,
        attacking,
        seeking
    }

    public AIState state;


    public BugShroomAI(EnemyBase baseEnemy) : base(baseEnemy)
    {
        state = AIState.idle;
    }

    public override void FixedUpdate()
    {
        base.Update();

        switch(state)
        {
            case AIState.idle:
                anim.SetFloat("WalkingSpeed", .75f);
                movement.NotMoving();
                if (target != null && Vector2.Distance(transform.position, target.position) > stopRange)
                {
                    state = AIState.seeking;
                }
                else if (target != null && lastAttack + attackCooldown < Time.time)
                {
                    Attack();
                }
                break;
            case AIState.seeking:
                SeekMovement();
                anim.ResetTrigger("Attacking");
                if (Vector2.Distance(transform.position, target.position) <= stopRange && lastAttack + attackCooldown < Time.time)
                {
                    Attack();
                }
                    
                break;
            case AIState.attacking:
                movement.NotMoving();
                if(transform.position.x < target.position.x )
                    anim.transform.localScale = new Vector3(-1, 1, 1);
                else
                    anim.transform.localScale = new Vector3(1, 1, 1);

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon2_attack"))
                    state = AIState.idle;
                break;
        }
    }

    public void Attack()
    {
        state = AIState.attacking;
        anim.SetTrigger("Attacking");
        lastAttack = Time.time;

    }

    //public override void LateUpdate()
    //{
    //    foreach(PlayerHealth hit in hitThisFrame)
    //    {
    //        int foundIndex = hitTargets.FindIndex((x) => x.target == hit);
    //        if (foundIndex == -1)
    //        {
    //            foundIndex = hitTargets.Count;
    //            hitTargets.Add(new hitTarget(hit, 0));
    //        }

    //        hitTargets[foundIndex].damage += attackDamage * Time.fixedDeltaTime;
    //        hitTargets[foundIndex].hitThisFrame = true;
    //    }

    //    for(int i = hitTargets.Count - 1; i >= 0; i--)
    //    {
    //        if (!hitTargets[i].hitThisFrame)
    //            hitTargets[i].damage -= attackDamage * Time.fixedDeltaTime;
    //        hitTargets[i].hitThisFrame = false;

    //        if (hitTargets[i].damage >= 1)
    //        {
    //            hitTargets[i].target.TakeDamage((int)hitTargets[i].damage / 1);
    //            hitTargets[i].damage %= 1;
    //        }


    //        if (hitTargets[i].damage <= 0)
    //            hitTargets.RemoveAt(i);

    //    }

    //    hitThisFrame.Clear();
    //}

    public void SeekMovement()
    {
        if (target.transform.position.x + stopRange < transform.position.x)
        {
            float distance = Mathf.Abs(target.transform.position.x + stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveLeft(speed);
            anim.SetFloat("WalkingSpeed", Mathf.Clamp(speed/3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.transform.position.x - stopRange > transform.position.x)
        {
            float distance = Mathf.Abs(target.transform.position.x - stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveRight(speed);
            //movement.MoveRight(moveSpeed);
            //anim.SetFloat("WalkingSpeed", moveSpeed/3f);
            anim.SetFloat("WalkingSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            movement.NotMoving();
    }

    //public class hitTarget
    //{
    //    public PlayerHealth target;
    //    public float damage;
    //    public bool hitThisFrame = true;

    //    public static implicit operator PlayerHealth(hitTarget target ) => target;

    //    public hitTarget(PlayerHealth target, float damage)
    //    {
    //        this.target = target;
    //        this.damage = damage;
    //    }

    //    //public static bool operator ==(PlayerHealth target, hitTarget group) => (group.target == target);
    //    //public static bool operator !=(PlayerHealth target, hitTarget group) => (group.target != target);
    //}
}
