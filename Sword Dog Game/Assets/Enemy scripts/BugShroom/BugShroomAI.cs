using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugShroomAI : BaseAI
{
    public float lastAttack = -100;

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

    public override void Update()
    {
        base.Update();

        switch(state)
        {
            case AIState.idle:
                anim.SetFloat("WalkingSpeed", 0);
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


    public void SeekMovement()
    {
        if (target.transform.position.x + stopRange < transform.position.x)
        {
            movement.MoveLeft(moveSpeed);
            anim.SetFloat("WalkingSpeed", moveSpeed/3f);
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.transform.position.x - stopRange > transform.position.x)
        {
            movement.MoveRight(moveSpeed);
            anim.SetFloat("WalkingSpeed", moveSpeed/3f);
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            movement.NotMoving();
    }
}
