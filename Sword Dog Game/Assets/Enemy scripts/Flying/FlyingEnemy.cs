using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{

    public override bool attacking
    {
        get
        {
            return ((((FlyingAI)ai).state == (FlyingAI.states.lunging)) || ((FlyingAI)ai).state == (FlyingAI.states.lungeReturn));
        }
        set
        {
            //Read only for this case
        }
    }

    public float scaleAnimator = 1;
    public float windupSpeedScalar = 1;
    public Rigidbody2D rb;

    public override bool stunned
    {
        get => base.stunned;

        set
        {
            ((FlyingAI)ai).state = FlyingAI.states.lungeFlee;
            if (value)
            {
                anim.speed = 0;
                rb.gravityScale = 5;
                rb.constraints = RigidbodyConstraints2D.None;
            }
            else
            {
                anim.speed = 1;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.gravityScale = 0;
            }

            base.stunned = value;
        }
    }


    public void Awake()
    {
        ai = new FlyingAI(this);
        rb ??= GetComponent<Rigidbody2D>();
    }

    public override void Parried(SwordFollow by)
    {
        base.Parried(by);
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * 1.25f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 500);
        rb.sharedMaterial = movement.friction;
    }

    public override void Blocked(SwordFollow by)
    {
        base.Blocked(by);
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * .75f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 250);
        ((FlyingAI)ai).state = FlyingAI.states.lungeFlee;

    }

}
