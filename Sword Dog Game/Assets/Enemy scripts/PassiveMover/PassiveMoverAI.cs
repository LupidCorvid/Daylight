using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveMoverAI : BaseAI
{
    public bool MovingRight = false;

    public CollisionsTracker cldr;

    public const float maxSlopeClimb = 75;

    private float maxClimbAngle;

    bool waitingTurnFin = false;

    public float hideDist = 6;

    public float lastSeen;

    public enum AIState
    {
        Idle,
        Moving,
        Hiding,
        Turning
    }

    public float WanderTime;
    public float IdleTime;

    public float WanderProgress;
    public float IdleProgress;

    AIState state = AIState.Moving;

    public PassiveMoverAI(EnemyBase baseScript) : base(baseScript)
    {
        cldr = enemyBase.GetComponent<CollisionsTracker>();
        cldr.colliderEnter += CollisionEntered;
        maxClimbAngle = Mathf.Sin(90 - maxSlopeClimb);
    }

    public void CollisionEntered(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            //Only turn around if it is likely a wall
            if(collision.GetContact(i).normal.normalized.y > maxClimbAngle)
                continue;

            if (collision.GetContact(i).point.x > transform.position.x)
            {
                MovingRight = false;
                anim.Play("turn");
                state = AIState.Turning;
            }
            else
            {
                MovingRight = true;
                anim.Play("turn");
                state = AIState.Turning;
            }
            waitingTurnFin = true;
        }
    }
   

    public override void Update()
    {
        base.Update();

        if (enemyBase.stunned)
            return;

        WanderBehavior();

        switch(state)
        {
            case AIState.Idle:
                Idle();
                break;

            case AIState.Moving:
                Move();
                break;

            case AIState.Turning:
                Turning();
                break;

            case AIState.Hiding:
                Hiding();
                break;
        }
    }

    public void WanderBehavior()
    {
        if(state == AIState.Idle)
        {
            IdleProgress += Time.deltaTime;
            if (IdleProgress > IdleTime)
            {
                if (Random.Range(0f, 1f) <= .5f)
                {
                    state = AIState.Moving;
                }
                else
                {
                    MovingRight = !MovingRight;
                    anim.Play("turn");
                    state = AIState.Turning;
                    waitingTurnFin = true;
                }

                IdleTime = Random.Range(1f, 3.5f);
                IdleProgress = 0;
            }
            return;
        }

        if(state == AIState.Moving)
        {
            WanderProgress += Time.deltaTime;
            if (WanderProgress > WanderTime)
            { 
                if(Random.Range(0f,1f) <= .5f)
                {
                    state = AIState.Idle;
                }
                else
                {
                    MovingRight = !MovingRight;
                    anim.Play("turn");
                    state = AIState.Turning;
                    waitingTurnFin = true;
                }
                WanderTime = Random.Range(3.5f, 10f);
                WanderProgress = 0;

            }

        }
    }

    public void Idle()
    {

        movement.NotMoving();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            anim.Play("idle");
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < hideDist && !Physics2D.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")))
        {
            Hide();
        }


    }

    public void Move()
    {

        if (target != null && Vector2.Distance(transform.position, target.transform.position) < hideDist && !Physics2D.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")))
        {
            Hide();
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("walk"))
            anim.Play("walk");
        anim.SetFloat("Speed", moveSpeed/2f);

        if (MovingRight)
            movement.MoveRight(moveSpeed);
        else
            movement.MoveLeft(moveSpeed);
    }

    public void Hide()
    {
        movement.NotMoving();
        anim.Play("hide");
        state = AIState.Hiding;
        
    }

    public void Hiding()
    {
        movement.NotMoving();


        //if (!(target != null && Vector2.Distance(transform.position, target.transform.position) < hideDist * 1.25f && !Physics2D.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain"))))
        //{
        //    Unhide();
        //}


        if (target != null && Vector2.Distance(transform.position, target.transform.position) < hideDist * 1.25f && !Physics2D.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")))
        {
            lastSeen = Time.time;
        }

        if (lastSeen + 0.75f < Time.time)
            Unhide();
    }

    public void Unhide()
    {
        anim.SetTrigger("Unhide");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            state = AIState.Moving;
            anim.ResetTrigger("Unhide");
        }
        
    }

    public void Turning()
    {
        movement.NotMoving();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("turn"))
            waitingTurnFin = false;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("turn") && !waitingTurnFin)
        {
            if(MovingRight)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            state = AIState.Moving;
        }
        
    }

    
}
