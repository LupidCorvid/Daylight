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
        lungeReturn
    }

    public states state;

    float lastAttack;
    float attackCooldown = 5;

    public Vector2 targetPosition
    {
        get
        {
            return (Vector2)target.transform.position + (prefferedOffset);
        }
    }

    public override void Start()
    {
        base.Start();
        target ??= GameObject.Find("Player(Clone)").transform;
    }

    public FlyingAI(EnemyBase enemy) : base(enemy)
    {

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        switch(state)
        {
            case states.pursuit:
                if (Mathf.Abs(targetPosition.x - transform.position.x) > acceptableRange.x || Mathf.Abs(targetPosition.y - transform.position.y) > acceptableRange.y)
                    Pursuit();
                if (attackSpeed != 0 && lastAttack + (attackCooldown / attackSpeed) <= Time.time)
                    state = states.telegraphing;
                break;
            case states.telegraphing:
                //Play animation that shows it is about to attack. Perhaps can be interrupted if it takes damage while in this state
                state = states.lunging;
                break;
            case states.lunging:
                //Lunge attack stuff, then return to pursuit. Use just setting velocity for now
                lastAttack = Time.time;
                rb.velocity = (target.position - transform.position) * 5;
                state = states.lungeReturn;
                break;
            case states.lungeReturn:
                if (Vector2.Distance(transform.position, target.position) <= 1f || rb.velocity.magnitude <= 2f)
                    state = states.pursuit;
                break;
        }
        
    }

    public void Pursuit()
    {
        Vector2 targetPosition = prefferedOffset + (Vector2)target.transform.position;

        ((FlyingMovement)movement).MoveDirection((targetPosition - (Vector2)transform.position).normalized * moveSpeed);
    }

}
