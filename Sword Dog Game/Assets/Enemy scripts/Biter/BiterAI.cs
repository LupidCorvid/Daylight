using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiterAI : BaseAI
{
    public float stopRange = 0.2f;

    public float attackRange = 2;

    public enum AIStates
    {
        idle,
        pursuit,
        attacking
    }

    public AIStates state;

    public BiterAI(EnemyBase baseEnemy) : base(baseEnemy)
    {
        state = AIStates.idle;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        switch(state)
        {
            case AIStates.idle:
                if (target != null && Mathf.Abs(target.position.x - transform.position.x) > stopRange)
                {
                    state = AIStates.pursuit;
                }
                else
                {
                    anim.SetFloat("MoveSpeed", 0);
                    movement.NotMoving();
                }
                break;
            case AIStates.pursuit:
                if (target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange && movement.slopeChecker.isGrounded)
                {
                    Attack();
                }
                else if (target == null || Mathf.Abs(target.position.x - transform.position.x) <= stopRange)
                {
                    state = AIStates.idle;
                }
                //else if (Vector2.Distance(target.transform.position, transform.position) <= attackRange && movement.slopeChecker.isGrounded)
                //{
                //    Attack();
                //}
                else
                {
                    SeekMovement();
                }
                break;
            case AIStates.attacking:
                Attacking();
                break;
        }
    }

    public void SeekMovement()
    {
        if (target.transform.position.x + stopRange < transform.position.x)
        {
            movement.MoveLeft(moveSpeed);
            anim.SetFloat("MoveSpeed", moveSpeed / 3f);
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.transform.position.x - stopRange > transform.position.x)
        {
            movement.MoveRight(moveSpeed);
            anim.SetFloat("MoveSpeed", moveSpeed / 3f);
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            movement.NotMoving();
    }

    public void Attack()
    {
        state = AIStates.attacking;
        anim.SetTrigger("Attack");
    }

    public void Attacking()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon1_bite"))
            state = AIStates.idle;
    }
}
