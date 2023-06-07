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

    bool waitingOnChance = false;

    bool turning = false;

    public BiterAI(EnemyBase baseEnemy) : base(baseEnemy)
    {
        state = AIStates.idle;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(turning)
            TurnAnim();

        if (enemyBase.stunned)
            return;

        if(target == null)
        {
            state = AIStates.idle;
        }

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

                    if (Random.Range(0, 1f) <= .15f * Time.deltaTime)
                        state = AIStates.keepDistance;
                }
                break;
            case AIStates.attacking:
                Attacking();
                break;
            case AIStates.keepDistance:
                float facingChanceModifier = Mathf.Lerp(2f, 1f, Vector2.Dot(enemyBase.facingDir, targetEntity.facingDir));
                float randomChargeChance = Random.Range(0f, 1f) / facingChanceModifier;

                if (randomChargeChance < .90f * Time.deltaTime)
                    waitingOnChance = false;

                if (Time.time > lastAttack + attackCooldown && !waitingOnChance)
                {
                    state = AIStates.pursuit;
                    if(Random.Range(0f, 1f) > .25f)
                        waitingOnChance = true;
                }
                KeepDistanceMovement();
                break;
        }
    }

    public void SeekMovement()
    {
        MoveInDirection(target.transform.position);
    }

    public void TurnAnim()
    {
        if(!turning)
        {
            turning = true;
            enemyBase.moveSpeed.multiplier /= 2;
            anim.Play("mon1_turn");
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon1_turn"))
        {
            turning = false;
            //anim.transform.localScale = new Vector3(anim.transform.localScale.x * -1, anim.transform.localScale.y, anim.transform.localScale.z);
            enemyBase.facingDir = Vector2.right * (anim.transform.localScale.z > 0 ? -1 : 1);
            enemyBase.moveSpeed.multiplier *= 2;
        }
    }

    public void MoveInDirection(Vector2 direction , bool controlDirection = true)
    {
        if (direction.x + stopRange < transform.position.x)
        {
            float distance = Mathf.Abs(direction.x + stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveLeft(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            if (anim.transform.localScale.x < 0 && !turning && controlDirection)
                TurnAnim();
            //anim.transform.localScale = new Vector3(1, 1, 1);
            //enemyBase.facingDir = new Vector2(-1, 0);
        }
        else if (direction.x - stopRange > transform.position.x)
        {
            float distance = Mathf.Abs(direction.x - stopRange - transform.position.x);
            float speed = Mathf.Clamp(distance * 5, 0f, moveSpeed);
            movement.MoveRight(speed);
            anim.SetFloat("MoveSpeed", Mathf.Clamp(speed / 3f, .75f, 9999999f));
            if (anim.transform.localScale.x > 0 && !turning && controlDirection)
                TurnAnim();
            //anim.transform.localScale = new Vector3(-1, 1, 1);
            //enemyBase.facingDir = new Vector2(1, 0);
        }
        else
        {
            if (controlDirection)
            {
                if (transform.position.x < direction.x)
                {
                    if (anim.transform.localScale.x < 0 && !turning)
                        TurnAnim();
                    //anim.transform.localScale = new Vector3(1, 1, 1);
                    //enemyBase.facingDir = new Vector2(-1, 0);
                }
                else
                {
                    //anim.transform.localScale = new Vector3(-1, 1, 1);
                    //enemyBase.facingDir = new Vector2(1, 0);
                    if (anim.transform.localScale.x > 0 && !turning)
                        TurnAnim();
                }
            }

            movement.NotMoving();
        }
    }

    public void KeepDistanceMovement()
    {
        if(Mathf.Abs(transform.position.x - target.position.x) - maintainDistance >= stopRange || true)
        {
            if(transform.position.x > target.transform.position.x)
            {
                Vector2 targetPoint = Vector2.right * (target.position.x + maintainDistance);
                MoveInDirection(targetPoint, false);

                if (Mathf.Abs(transform.position.x - targetPoint.x) <= 1.15f)
                {
                    //anim.transform.localScale = new Vector3(1, 1, 1);
                    //enemyBase.facingDir = new Vector2(-1, 0);
                    if (anim.transform.localScale.x < 0 && !turning)
                        TurnAnim();
                }
            }
            else
            {
                Vector2 targetPoint = Vector2.right * (target.position.x - maintainDistance);
                MoveInDirection(targetPoint, false);

                if (Mathf.Abs(transform.position.x - targetPoint.x) <= 1.15f)
                {
                    //anim.transform.localScale = new Vector3(-1, 1, 1);
                    //enemyBase.facingDir = new Vector2(1, 0);
                    if (anim.transform.localScale.x > 0 && !turning)
                        TurnAnim();
                }

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
        enemyBase.cry(); // TODO position this better/more responsively
        //anim.SetFloat("AttackSpeed", attackSpeed);
        anim.SetTrigger("Attack");
        //if(target.transform.position.x < transform.position.x)
        //    anim.transform.localScale = new Vector3(1, 1, 1);
        //else
        //    anim.transform.localScale = new Vector3(-1, 1, 1);
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