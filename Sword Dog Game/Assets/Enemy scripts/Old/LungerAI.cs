using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungerAI : BaseAI
{
    private const float START_LEAP_TIME = 25565.7777f;

    AIStates state;
    public enum AIStates
    {
        charging,
        lunging,
        moving
    }

    float attackAngle = 0.22f;

    public float chargeTime = 1;
    public float chargeStart;

    public bool waitForFullStop = true;


    public float cooldown
    {
        get
        {
            if (enemyBase.attackDamage == 0)
                return 999999;
            return 1 / enemyBase.attackDamage;
        }
    }

    public float lastLunge;

    public Collider2D cldr;

    public LungerAI(EnemyBase enemyBase) : base(enemyBase)
    {
        state = AIStates.moving;
        cldr = enemyBase.GetComponent<Collider2D>();
    }

    public override void Update()
    {
        base.Update();

        if (enemyBase.stunned)
            return;

        if(state == AIStates.moving)
        {
            cldr.sharedMaterial = ((Lunger)enemyBase).slippery;

            if (target == null)
                return;
            if (target.transform.position.x < transform.position.x)
            {
                //rb.AddForce(Vector2.left * moveSpeed * Time.deltaTime * 500);
                movement.MoveRight();
            }
            else if (target.transform.position.x > transform.position.x)
            {
                //rb.AddForce(Vector2.right * moveSpeed * Time.deltaTime * 500);
                movement.MoveRight();
            }
            if(Mathf.Abs(target.transform.position.y - transform.position.y) < 2 && Mathf.Abs(target.position.x - transform.position.x) < 10 && Mathf.Abs(target.position.x - transform.position.x) > 1.5f)
            {
                if(lastLunge + cooldown < Time.time)
                {
                    state = AIStates.charging;
                    //chargeStart = Time.time;
                    chargeStart = START_LEAP_TIME;
                }
            }

        }
        else if (state == AIStates.charging)
        {
            //Maybe should be moved after trajectory check
            cldr.sharedMaterial = ((Lunger)enemyBase).stopping;
            if (chargeStart == START_LEAP_TIME)
            {
                if (Mathf.Abs(rb.velocity.x) > .05f && enemyBase.slopeChecker.isGrounded)
                    return;
                chargeStart = Time.time;
            }
            float strength = getLungeStrength();
            if (float.IsNaN(strength) || Mathf.Abs(strength) > 35)
            {
                state = AIStates.moving;
                return;
            }
            rb.drag = 5;
            if (chargeStart + chargeTime > Time.time && (!waitForFullStop || rb.velocity.x < .05f))
            {
                state = AIStates.lunging;
                Lunge();
            }
        }
        else if (state == AIStates.lunging)
        {
            rb.drag = 0;
            if (Mathf.Abs(rb.velocity.x) < 1)
                state = AIStates.moving;
        }
    }

    public void Lunge()
    {
        float strength = getLungeStrength();
        if(!float.IsNaN(strength))
        {
            int neg = 1;
            if (target.transform.position.x - transform.position.x < 0)
                neg = -1;
            rb.velocity = new Vector2(strength * Mathf.Cos(attackAngle) * neg, strength * Mathf.Sin(attackAngle));
        }
    }

    public float getLungeStrength()
    {
        float grav = rb.gravityScale * 9.8f;
        //Adds some to the direction so that the enemy doesnt just stop on top of the player (removed because it will only land on top at edge of range)
        Vector2 relTar = (target.transform.position - transform.position) * 1f;
        if (relTar.x < 0)
            relTar = new Vector2( relTar.x * -1, relTar.y);
        float strength = ((1.0f / Mathf.Cos(attackAngle)) * Mathf.Sqrt(((grav * Mathf.Pow(relTar.x, 2)/2)/(relTar.x * Mathf.Tan(attackAngle) - relTar.y))));
        return strength;
    }

    

    

}
