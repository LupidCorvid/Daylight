using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biter : EnemyBase
{
    public override bool attacking
    {
        get
        {
            return (((BiterAI)ai).state == BiterAI.AIStates.attacking || ((BiterAI)ai).state == BiterAI.AIStates.pursuit || ai.attacking);
        }
        set
        { 
            //Read only for this case
        }
    }


    public override bool stunned
    {
        get => base.stunned;

        set
        {
            ((BiterAI)ai).state = BiterAI.AIStates.keepDistance;
            if (value)
            {
                anim.speed = 0;
                GetComponent<Rigidbody2D>().drag = 5;
            }
            else
            {
                anim.speed = 1;
                GetComponent<Rigidbody2D>().drag = 0;
            }

            base.stunned = value;
        }
    }

    public bool inPrologue = false;
    bool animControlled = false;

    public void Awake()
    {
        ai = new BiterAI(this);
    }

    public override void Parried(SwordFollow by)
    {
        base.Parried(by);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * 1.25f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 150);
        rb.sharedMaterial = movement.friction;
    }

    public override void Blocked(SwordFollow by)
    {
        base.Blocked(by);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * .75f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 200);
        ((BiterAI)ai).state = BiterAI.AIStates.keepDistance;
        
        //ai.anim.SetTrigger("Land");
    }

    public override void Die()
    {
        if(!inPrologue)
            base.Die();
        else
        {
            animControlled = true;
            killed?.Invoke();
        }
    }

    public override void TakeDamage(int amount, Entity source)
    {
        base.TakeDamage(amount, source);
    }

    public override void Update()
    {
        if(!animControlled)
            base.Update();
    }

    public override void FixedUpdate()
    {
        if(!animControlled)
            base.FixedUpdate();
    }
}
