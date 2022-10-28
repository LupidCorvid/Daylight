using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLobberAI : BaseAI
{
    public GameObject rockProjectile;

    public float projectileSpeed = 17;


    public AIstate state;
    public enum AIstate
    {
        moving,
        attacking
    }

    public float lastAttack = -100;

    public float attackCooldown
    {
        get
        {
            if (enemyBase.attackSpeed == 0)
                return 999999;
            return 1 / enemyBase.attackSpeed;
        }
    }
    public float moveSpeed
    {
        get
        {
            return enemyBase.moveSpeed;
        }
        set
        {
            enemyBase.moveSpeed = value;
        }

    }

    public RockLobberAI(EnemyBase baseScript, GameObject rockPrefab) : base(baseScript)
    {
        rockProjectile = rockPrefab;
        state = AIstate.moving;
    }

    public void attack()
    {
        if(target != null)
        {
            GameObject addedProjectile = EnemyBase.Instantiate(rockProjectile, transform.position, transform.rotation);
            Rigidbody2D projectileVelocity = addedProjectile.GetComponent<Rigidbody2D>();
            //arctan((sqrt(c^4 - g(gd^2 + 2yv^2))+c^2)/gd)
            Vector2 relTar = target.position - transform.position;
            float projSpeed = projectileSpeed;
            float grav = projectileVelocity.gravityScale;
            float angle;
            //angle = Mathf.Atan((Mathf.Sqrt(Mathf.Pow(projSpeed, 4) - (grav * ((grav * Mathf.Pow(relTar.x, 2)) + (2 * relTar.y 
            //                  * Mathf.Pow(projSpeed, 2))))) + Mathf.Pow(projSpeed, 2))/ grav * relTar.x);
            angle = getAttackAngle();
            if (!float.IsNaN(angle))
            {
                projectileVelocity.velocity = new Vector2(projSpeed * Mathf.Cos(angle), projSpeed * Mathf.Sin(angle));
                lastAttack = Time.time;
            }
        }
    }
    public void attack(float angle)
    {
        GameObject addedProjectile = EnemyBase.Instantiate(rockProjectile, transform.position, transform.rotation);
        Rigidbody2D projectileVelocity = addedProjectile.GetComponent<Rigidbody2D>();
        projectileVelocity.velocity = new Vector2(projectileSpeed * Mathf.Cos(angle), projectileSpeed * Mathf.Sin(angle));
        lastAttack = Time.time;
    }
    public float getAttackAngle()
    {
        Vector2 relTar = target.position - transform.position;
        float projSpeed = projectileSpeed;
        float grav = rockProjectile.GetComponent<Rigidbody2D>().gravityScale * 9.8f;
        float angle;
        angle = Mathf.Atan((Mathf.Sqrt(Mathf.Pow(projSpeed, 4) - grav * (grav * Mathf.Pow(relTar.x, 2) + 2 * relTar.y
                          * Mathf.Pow(projSpeed, 2))) + Mathf.Pow(projSpeed, 2)) / (grav * relTar.x));
        if (relTar.x < 0)
            angle += Mathf.PI;
        Debug.DrawLine(transform.position, transform.position + new Vector3(projSpeed * Mathf.Cos(angle), projSpeed * Mathf.Sin(angle)), Color.red, 1);
        return angle;
    }

    public override void Update()
    {
        base.Update();

        if (state == AIstate.attacking)
        {
            
            //Wait for attack animation to playout and finish
            attack();
            state = AIstate.moving;
            
        }
        else if (state == AIstate.moving)
        {
            if(target.transform.position.x < transform.position.x)
            {
                rb.AddForce(Vector2.left * moveSpeed * Time.deltaTime * 500);
            }
            else if(target.transform.position.x > transform.position.x)
            {
                rb.AddForce(Vector2.right * moveSpeed * Time.deltaTime * 500);
            }
            if (lastAttack + attackCooldown < Time.time && !float.IsNaN(getAttackAngle()) && Mathf.Abs(Vector2.Distance(target.position, transform.position)) > 4)
                state = AIstate.attacking;
        }
    }
}
