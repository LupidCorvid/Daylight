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
        target ??= GameObject.Find("Player(Clone)").transform;
        resetPosition = (Vector2)transform.position - web.connectedBody.position;
        state = states.idle;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(web.connectedBody.transform.position - transform.position, Vector2.up));

        switch (state)
        {
            case states.idle:
                if(target.transform.position.y < transform.position.y)
                {
                    
                    //Find time it would take to drop to player's height. Might be wrong as it isnt relative height?
                    float timeToFall = Mathf.Sqrt(Mathf.Abs((/*target.transform.position.y*/ - transform.position.y) / (9.8f * rb.gravityScale)));
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

                if (Vector2.Distance(transform.position, (Vector2)web.connectedBody.position + resetPosition + Vector2.up * preferredHeight) - web.distance <= .05f)
                {
                    lastLand = Time.time;
                    state = states.landStop;
                }
                break;
            case states.landStop:
                if (lastLand + returnWaitTime <= Time.time)
                    state = states.returning;
                break;
            case states.returning:
                //Prevents it from swinging too much if hit with knockback (swings get bigger as it gets closer to the resetPosition)
                rb.drag = 1;
                web.distance -= 1 * moveSpeed * Time.deltaTime;
                transform.position += Vector3.up * Time.deltaTime * moveSpeed * 1;
                if (web.distance <= preferredHeight)
                    state = states.idle;
                break;
        }
    }
}
