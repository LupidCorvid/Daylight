using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLobberAI : BaseAI
{
    public GameObject rockProjectile;

    public float projectileSpeed = 17;

    public float stopRange = 15f;


    public AIstate state;
    public enum AIstate
    {
        moving,
        rangedAttacking,
        meleeAttacking
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

        if (state == AIstate.rangedAttacking)
        {
            //Wait for attack prep animation to finish
            attack();
            //Wait for attack animation to finish
            state = AIstate.moving;
            
        }
        else if (state == AIstate.moving)
        {
            if (target.transform.position.x + stopRange < transform.position.x)
            {
                movement.MoveLeft(moveSpeed);
            }
            else if (target.transform.position.x - stopRange > transform.position.x)
            {
                movement.MoveRight(moveSpeed);
            }
            else
                movement.NotMoving();

            if (Vector2.Distance(target.position, transform.position) <= 7)
                state = AIstate.meleeAttacking;
            else if (Vector2.Distance(target.position, transform.position) <= stopRange * .66f)
            {
                if (target.transform.position.x < transform.position.x)
                    movement.MoveRight(moveSpeed);
                else
                    movement.MoveLeft(moveSpeed);

            }
            else if (lastAttack + attackCooldown < Time.time && !float.IsNaN(getAttackAngle()))
                state = AIstate.rangedAttacking;
        }
        else if (state == AIstate.meleeAttacking)
        {
            //NYI
            if (Vector2.Distance(target.position, transform.position) > 7)
                state = AIstate.rangedAttacking;
            else
                state = AIstate.moving;
        }
        
    }
}
