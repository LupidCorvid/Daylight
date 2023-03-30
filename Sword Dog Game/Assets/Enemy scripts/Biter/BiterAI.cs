using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiterAI : BaseAI
{
    public float stopRange = 0.4f;

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
                    anim.SetFloat("MoveSpeed", .75f);
                    movement.NotMoving();
                }
                break;
            case AIStates.pursuit:
                if (target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange && movement.slopeChecker.isGrounded)
                {
                    Attack();
                }
                else if (target == null || Mathf.Abs(target.position.x - transform.position.x) <= stopRange || target.position.y > transform.position.y + 7)
                {
                    state = AIStates.idle;
                }
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
            float distance = Mathf.Abs(target.transform.position.x + stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveLeft(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.transform.position.x - stopRange > transform.position.x)
        {
            float distance = Mathf.Abs(target.transform.position.x - stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveRight(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            movement.NotMoving();
    }

    public void Attack()
    {
        if (attackSpeed == 0)
            return;
        state = AIStates.attacking;
        anim.SetFloat("AttackSpeed", attackSpeed);
        anim.SetTrigger("Attack");
        if(target.transform.position.x < transform.position.x)
            anim.transform.localScale = new Vector3(1, 1, 1);
        else
            anim.transform.localScale = new Vector3(-1, 1, 1);
        //applyAttackDamage();
    }

    public override void applyAttackDamage()
    {
        Vector2 location = transform.position + (anim.transform.localScale.x * Vector3.left * 1f);
        Vector2 range = new Vector2(1, .5f) + Vector2.right * (.25f);
        DamageBox(location, range);
    }

    public void Attacking()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon1_bite"))
            state = AIStates.idle;
    }
}
