using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungerAI : BaseAI
{
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

    public LungerAI(EnemyBase enemyBase) : base(enemyBase)
    {
        state = AIStates.moving;
    }

    public override void Update()
    {
        base.Update();
        if(state == AIStates.moving)
        {
            rb.sharedMaterial = ((Lunger)enemyBase).slippery;
            if (target.transform.position.x < transform.position.x)
            {
                rb.AddForce(Vector2.left * moveSpeed * Time.deltaTime * 500);
            }
            else if (target.transform.position.x > transform.position.x)
            {
                rb.AddForce(Vector2.right * moveSpeed * Time.deltaTime * 500);
            }
            if(Mathf.Abs(target.transform.position.y - transform.position.y) < 2 && Mathf.Abs(target.transform.position.x - transform.position.x) < 10)
            {
                if(lastLunge + cooldown < Time.time)
                {
                    state = AIStates.charging;
                    chargeStart = Time.time;
                }
            }

        }
        else if (state == AIStates.charging)
        {
            rb.sharedMaterial = ((Lunger)enemyBase).friction;
            if (chargeStart + chargeTime > Time.time)
            {
                state = AIStates.lunging;
                Lunge();
                state = AIStates.lunging;
            }
        }
        else if (state == AIStates.lunging)
        {
            if (Mathf.Abs(rb.velocity.x) < 1)
                state = AIStates.moving;
        }
    }
    public void Lunge()
    {
        float strength = getLungeStrength();
        if(!float.IsNaN(strength))
        {
            rb.velocity = new Vector2(strength * Mathf.Cos(attackAngle), strength * Mathf.Sin(attackAngle));
        }
    }
    public float getLungeStrength()
    {
        float grav = rb.gravityScale * 9.8f;
        //Adds some to the direction so that the enemy doesnt just stop on top of the player
        Vector2 relTar = (target.transform.position - transform.position) * 1.5f;
        float strength = ((1.0f / Mathf.Cos(attackAngle)) * Mathf.Sqrt(((grav * Mathf.Pow(relTar.x, 2)/2)/(relTar.x * Mathf.Tan(attackAngle) - relTar.y))));
        return strength;
    }

}
