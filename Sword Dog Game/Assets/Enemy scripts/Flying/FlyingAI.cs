using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Concept for dot product design is from Game Endeavor
public class FlyingAI : BaseAI
{
    public Vector2 prefferedOffset = new Vector2(0, 4);
    public Vector2 acceptableRange = new Vector2(5, 1);

    public enum states
    {
        lunging,
        pursuit,
        telegraphing,
        lungeReturn,
        lungeFlee
    }

    public states state;

    float lastAttack;
    float attackCooldown = 5;

    float attackDistance = 5.5f;

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
        state = states.pursuit;
        //target ??= GameObject.Find("Player(Clone)").transform;
    }

    public override void FoundTarget(Transform newTarget)
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
            case states.pursuit:
                if (Mathf.Abs(targetPosition.x - transform.position.x) > acceptableRange.x || Mathf.Abs(targetPosition.y - transform.position.y) > acceptableRange.y)
                    Pursuit();
                if (attackSpeed != 0 && lastAttack + (attackCooldown / attackSpeed) <= Time.time && Vector2.Distance(transform.position, targetPosition) < attackDistance)
                    state = states.telegraphing;
                break;
            case states.telegraphing:
                //Play animation that shows it is about to attack. Perhaps can be interrupted if it takes damage while in this state
                windUpTarget = ((Vector2)target.position);
                Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                    windUpTarget += (Vector2)(targetRb.velocity * Random.Range(0, .1f)); //UpperRange time should be length of telegraph anim
                state = states.lunging;
                break;
            case states.lunging:
                //Lunge attack stuff, then return to pursuit. Use just setting velocity for now
                lastAttack = Time.time;
                rb.velocity = Vector2.ClampMagnitude((windUpTarget - (Vector2)transform.position) * 7, attackDistance * 7);
                state = states.lungeReturn;
                break;
            case states.lungeReturn:
                if (Vector2.Distance(transform.position, windUpTarget) <= 1f || rb.velocity.magnitude <= 2f)
                    state = states.lungeFlee;
                break;
            case states.lungeFlee:
                Flee();
                if(Vector2.Distance(target.position, transform.position) > 3.5f)
                    state = states.pursuit;

                break;
        }
        
    }

    public void Pursuit()
    {
        Vector2 targetPosition = prefferedOffset + (Vector2)target.transform.position;

        ((FlyingMovement)movement).MoveDirection((targetPosition - (Vector2)transform.position).normalized * moveSpeed);
    }

    public void Flee()
    {
        ((FlyingMovement)movement).MoveDirection((-targetPosition + (Vector2)transform.position).normalized.x * Vector2.right * 0 + Vector2.right * (rb.velocity.normalized.x) * 8 + Vector2.up * 8);
    }

}
