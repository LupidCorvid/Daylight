using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiterAI : BaseAI
{
    public float stopRange = 0.4f;

    public float attackRange = 1;

    public float lastAttack;

    public float maintainDistance = 6;

    public float attackCooldown
    {
        get
        {
            if (attackSpeed == 0)
                return 999999f;
            return 2f / attackSpeed;
        }
    }

    public enum AIStates
    {
        idle,
        pursuit,
        attacking,
        keepDistance
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
                if (target != null && Vector2.Distance(target.transform.position, (Vector2)transform.position + rb.velocity * .5f) <= attackRange && movement.slopeChecker.isGrounded && lastAttack + attackCooldown < Time.time)
                {
                    Attack();
                }
                else if (target == null || /*Mathf.Abs(target.position.x - transform.position.x) <= stopRange ||*/ target.position.y > transform.position.y + 7)
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
            case AIStates.keepDistance:
                if(Time.time > lastAttack + attackCooldown)
                {
                    state = AIStates.pursuit;
                }
                KeepDistanceMovement();
                break;
        }
    }

    public void SeekMovement()
    {
        MoveInDirection(target.transform.position);
    }

    public void MoveInDirection(Vector2 direction)
    {
        if (direction.x + stopRange < transform.position.x)
        {
            float distance = Mathf.Abs(direction.x + stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveLeft(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x - stopRange > transform.position.x)
        {
            float distance = Mathf.Abs(direction.x - stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveRight(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            anim.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            movement.NotMoving();
    }

    public void KeepDistanceMovement()
    {
        if(Mathf.Abs(transform.position.x - target.position.x) - maintainDistance >= stopRange || true)
        {
            if(transform.position.x > target.transform.position.x)
            {
                Vector2 targetPoint = Vector2.right * (target.position.x + maintainDistance);
                MoveInDirection(targetPoint);
            }
            else
            {
                Vector2 targetPoint = Vector2.right * (target.position.x - maintainDistance);
                MoveInDirection(targetPoint);
            }
        }
    }

    public void Attack()
    {
        if (attackSpeed == 0)
            return;
        lastAttack = Time.time;
        state = AIStates.attacking;
        attacking = true;
        //anim.SetFloat("AttackSpeed", attackSpeed);
        anim.SetTrigger("Attack");
        if(target.transform.position.x < transform.position.x)
            anim.transform.localScale = new Vector3(1, 1, 1);
        else
            anim.transform.localScale = new Vector3(-1, 1, 1);
        //applyAttackDamage();
    }

    public override void applyAttackDamage()
    {
        Vector2 location = transform.position + (anim.transform.localScale.x * Vector3.left * 1f) + Vector3.down * .5f;
        Vector2 range = new Vector2(1, .5f) + Vector2.right * (.25f);
        DamageBox(location, range);
    }

    public void Attacking()
    {
        if (!attacking)
            state = AIStates.keepDistance;
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
}