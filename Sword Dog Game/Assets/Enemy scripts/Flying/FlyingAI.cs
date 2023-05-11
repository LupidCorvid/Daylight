using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Concept for dot product design is from Game Endeavor
public class FlyingAI : BaseAI
{
    public Vector2 prefferedOffset = new Vector2(0, 4);
    public Vector2 acceptableRange = new Vector2(5, 1);

    public Vector2 perchedPoint;

    public enum states
    {
        sitting,
        gettingUp,
        lunging,
        pursuit,
        telegraphing,
        lungeReturn,
        lungeFlee,
        turning
    }

    public states state;

    float lastAttack;
    float attackCooldown = 5;

    float attackDistance = 5.5f;

    bool facingLeft = true;

    public float windupSpeedScalar
    {
        get
        {
            if (enemyBase == null)
                return 1;
            return ((FlyingEnemy)enemyBase).windupSpeedScalar;
        }
    }

    public Vector2 targetPosition
    {
        get
        {
            return (Vector2)target.transform.position + (prefferedOffset);
        }
    }

    public Vector2 windUpTarget;

    public override void Start()
    {
        base.Start();
        state = states.sitting;
        if (transform.localScale.x < 0)
            facingLeft = false;
    }

    public override void FoundTarget(Entity newTarget)
    {
        base.FoundTarget(newTarget);
        lastAttack = Time.time;
    }

    public FlyingAI(EnemyBase enemy) : base(enemy)
    {
        prefferedOffset += new Vector2(Random.Range(-.75f, .75f), Random.Range(-.5f, .5f));
        acceptableRange *= Random.Range(.9f, 1.1f);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (target == null)
            return;//Need idle for this case
        switch(state)
        {
            case states.sitting:
                perchedPoint = transform.position;
                if (target != null)
                {
                    anim.SetBool("Sitting", false);

                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon4_sit"))
                    {
                        state = states.gettingUp;
                    }
                }
                break;

            case states.gettingUp:
                updateFacingDirection();
                if(Vector2.Distance(target.transform.position, transform.position) <= 3)
                {
                    moveToPoint((perchedPoint - (Vector2)target.transform.position ).normalized * 3);
                }
                else
                {
                    moveToPoint(perchedPoint + Vector2.up * 2);
                }
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon4_sitToFly"))
                    state = states.pursuit;
                break;

            case states.pursuit:

                if (Mathf.Abs(targetPosition.x - transform.position.x) > acceptableRange.x || Mathf.Abs(targetPosition.y - transform.position.y) > acceptableRange.y)
                    Pursuit();
                if (transform.rotation.eulerAngles.z > 0)
                {
                    if (facingLeft)
                        rotateToDirection(Vector2.left, moveSpeed);
                    else
                        rotateToDirection(Vector2.right, moveSpeed);
                    break;
                }
                if (attackSpeed != 0 && lastAttack + (attackCooldown / attackSpeed) <= Time.time && Vector2.Distance(transform.position, targetPosition) < attackDistance)
                {
                    state = states.telegraphing;
                    perchedPoint = transform.position;
                    anim.SetTrigger("Telegraph");
                    anim.SetFloat("WindupSpeed", windupSpeedScalar);
                    Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                    windUpTarget = (Vector2)target.transform.position;
                    if (targetRb != null)
                        windUpTarget += (Vector2)(targetRb.velocity * Random.Range(0, 1f/windupSpeedScalar)); //UpperRange time should be length of telegraph anim
                }

                if ((facingLeft ^ (transform.position.x > targetPosition.x)))
                {
                    anim.SetTrigger("Turn");
                    ((FlyingEnemy)enemyBase).scaleAnimator = 1;
                    state = states.turning;
                }

                break;

            case states.turning:
                if (Mathf.Abs(targetPosition.x - transform.position.x) > acceptableRange.x || Mathf.Abs(targetPosition.y - transform.position.y) > acceptableRange.y)
                    Pursuit();
                

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon4_turn"))
                {
                    updateFacingDirection();
                    state = states.pursuit;
                    anim.ResetTrigger("Turn");
                }
                else
                {
                    transform.localScale = new Vector3((facingLeft ? 1 : -1) * ((FlyingEnemy)enemyBase).scaleAnimator, 1, 1);
                }

                break;

            case states.telegraphing:
                //Play animation that shows it is about to attack. Perhaps can be interrupted if it takes damage while in this state

                //if (!anim.GetCurrentAnimatorStateInfo(0).IsName("mon4_dive_anticipate"))
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("mon4_dive_attack"))
                {
                    state = states.lunging;
                    anim.ResetTrigger("Telegraph");
                }
                else
                {
                    anim.SetTrigger("Telegraph");
                    moveToPoint(perchedPoint + (perchedPoint - windUpTarget).normalized * 1, windupSpeedScalar * .5f);
                    rotateToDirection(windUpTarget - (Vector2)transform.position, windupSpeedScalar);
                    //windUpTarget = ((Vector2)target.position);
                }
                break;

            case states.lunging:
                //Lunge attack stuff, then return to pursuit. Use just setting velocity for now
                lastAttack = Time.time;
                rb.velocity = Vector2.ClampMagnitude((windUpTarget - (Vector2)transform.position) * 7, attackDistance * 10); //constants were 7s
                state = states.lungeReturn;
                break;

            case states.lungeReturn:
                if (Vector2.Distance(transform.position, windUpTarget) <= 1f || rb.velocity.magnitude <= 3f)
                {
                    state = states.lungeFlee;
                    DamageBox(transform.position + transform.rotation * new Vector2(facingLeft ? -1 : 1, 0), Vector2.one);
                }
                //rotateToDirection((Vector2)transform.position - windUpTarget);
                rotateToDirection(rb.velocity);
                break;

            case states.lungeFlee:
                Flee();
                //rotateToDirection((Vector2)transform.position - targetPosition);
                rotateToDirection(Vector2.right * (rb.velocity.normalized.x) * 64 + Vector2.up * 8);
                if (Vector2.Distance(windUpTarget, transform.position) > 4.5f)
                    state = states.pursuit;
                break;
        }
        
    }

    public void Pursuit()
    {
        Vector2 targetPosition = prefferedOffset + (Vector2)target.transform.position;

        ((FlyingMovement)movement).MoveDirection((targetPosition - (Vector2)transform.position).normalized * moveSpeed);
    }

    public void updateFacingDirection()
    {
        if(target != null)
        {
            if (target.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                facingLeft = true;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                facingLeft = false;
            }
        }
    }

    public void moveToPoint(Vector2 targetPosition, float speedScalar = 1)
    {
        ((FlyingMovement)movement).MoveDirection((targetPosition - (Vector2)transform.position).normalized * moveSpeed * speedScalar);    
    }

    public void rotateToDirection(Vector2 direction, float speedScalar = 1)
    {
        direction.y *= .5f;
        if(facingLeft)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, Mathf.Clamp(Vector2.Angle(Vector2.left, direction) * speedScalar, -75, 90)), 180 * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, Mathf.Clamp(-Vector2.Angle(Vector2.left, -direction) * speedScalar, -90, 75)), 180 * Time.deltaTime);
        }
    }

    public void Flee()
    {
        ((FlyingMovement)movement).MoveDirection(Vector2.right * (rb.velocity.normalized.x) * 12 + Vector2.up * 8);
    }

}
