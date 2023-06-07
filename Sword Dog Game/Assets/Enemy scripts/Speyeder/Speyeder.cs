using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speyeder : EnemyBase
{
    public DistanceJoint2D web;
    public SpeyederEye eye;

    public override bool attacking
    {
        get
        {
            return (((SpeyederAI)ai).state == SpeyederAI.states.dropping || ((SpeyederAI)ai).state == SpeyederAI.states.landStop);
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
            ((SpeyederAI)ai).state = SpeyederAI.states.returning;
            if (value)
                anim.speed = 0;
            else
                anim.speed = 1;

            eye.paused = value;

            base.stunned = value;
        } 
    }

    private void Awake()
    {
        ai = new SpeyederAI(this, web);

    }

    public override void Parried(SwordFollow by)
    {
        base.Parried(by);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * 1.25f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 1000);
        ((SpeyederAI)ai).preventNextDamage = true;
    }

    public override void Blocked(SwordFollow by)
    {
        base.Blocked(by);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = (transform.position - by.pmScript.transform.position).normalized * .75f;
        rb.AddForce((transform.position - by.pmScript.transform.position).normalized * 250);
        web.distance = Vector2.Distance(transform.position, web.connectedBody.position);
        ((SpeyederAI)ai).state = SpeyederAI.states.landStop;
        ai.anim.SetTrigger("Land");
    }
}
