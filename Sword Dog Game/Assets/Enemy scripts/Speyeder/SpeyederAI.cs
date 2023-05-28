using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeyederAI : BaseAI
{
    public float preferredHeight;

    public Vector2 resetPosition;

    private Rigidbody2D _tarPhys;

    public Rigidbody2D targetPhys
    {
        get
        {
            if (target != null && (_tarPhys == null || _tarPhys.transform != target))
                _tarPhys = target.GetComponent<Rigidbody2D>();

            return _tarPhys;
        }
    }

    public DistanceJoint2D web;

    public float lastLand;
    public float returnWaitTime = 1.25f;


    public enum states
    {
        idle,
        dropping,
        landStop,
        returning
    }

    public states state;

    public SpeyederAI(EnemyBase enemy, DistanceJoint2D web) : base(enemy)
    {
        this.web = web;
        preferredHeight = web.distance;
    }

    public override void Start()
    {
        base.Start();
        //target ??= GameObject.Find("Player(Clone)").transform;
        resetPosition = (Vector2)transform.position - web.connectedBody.position;
        state = states.idle;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        

        transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(web.connectedBody.transform.position - transform.position, Vector2.up));

        if (enemyBase.stunned)
            return;

        anim.SetFloat("XVel", rb.velocity.x);
        anim.SetFloat("YVel", rb.velocity.y);


        switch (state)
        {
            case states.idle:
                anim.SetBool("Returning", false);
                if(target != null && target.transform.position.y < transform.position.y)
                {
                    
                    //Find time it would take to drop to player's height. Might be wrong as it isnt relative height?
                    float timeToFall = Mathf.Sqrt(Mathf.Abs((target.transform.position.y - transform.position.y) / (9.8f * rb.gravityScale)));
                    //Given that time find if the player would be in range given their velocity in that time
                    if (Mathf.Abs(((Vector2)target.transform.position + (targetPhys.velocity * timeToFall)).x - transform.position.x) <= 1)
                    {
                        web.distance = Mathf.Abs((transform.position.y - target.position.y) + preferredHeight);
                        transform.position -= Vector3.down * Time.deltaTime * 0.001f;//Done to make physics update, otherwise it wont fall
                        state = states.dropping;
                        rb.drag = 0;
                    }
                }
                break;
            case states.dropping:
                if(target.position.y < ((Vector2)web.connectedBody.position + resetPosition + Vector2.down * web.distance).y)
                    web.distance = Mathf.Abs((transform.position.y - target.position.y) + preferredHeight);

                if ((transform.position.y - target.position.y) + rb.velocity.y * .15f < 0)
                {
                    anim.SetTrigger("Land");
                }
                //if (Vector2.Distance(transform.position, web.connectedBody.position + resetPosition + Vector2.up * preferredHeight) - web.distance <= .05f)
                if(Vector2.Distance(transform.position, web.connectedBody.position + Vector2.down * web.distance) <= .05f)
                {
                    lastLand = Time.time;
                    state = states.landStop;
                    //applyAttackDamage();
                    anim.SetTrigger("Land");
                }
                break;
            case states.landStop:
                if ((transform.position.y - target.position.y) + rb.velocity.y * .15f < 0)
                {
                    anim.SetTrigger("Land");
                }
                //anim.SetTrigger("Land");
                if (lastLand + returnWaitTime <= Time.time)
                    state = states.returning;
                break;
            case states.returning:
                anim.ResetTrigger("Land");
                anim.SetBool("Returning", true);
                //Prevents it from swinging too much if hit with knockback (swings get bigger as it gets closer to the resetPosition)
                rb.drag = 1;
                web.distance -= 1 * moveSpeed * Time.deltaTime;
                transform.position += Vector3.up * Time.deltaTime * moveSpeed * 1;
                if (web.distance <= preferredHeight)
                    state = states.idle;
                break;
        }
    }

    public override void applyAttackDamage()
    {
        enemyBase.cry();
        Vector2 location = transform.position + Vector3.down * 1.25f;
        Vector2 range = new Vector2(.75f, 1.25f);
        DamageBox(location, range);
    }
}
