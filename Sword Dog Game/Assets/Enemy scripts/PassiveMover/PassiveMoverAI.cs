using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveMoverAI : BaseAI
{
    public bool MovingRight = false;

    public CollisionsTracker cldr;

    public const float maxSlopeClimb = 75;

    private float maxClimbAngle;

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
                MovingRight = false;
            else
                MovingRight = true;
        }
    }
   

    public override void Update()
    {
        base.Update();

        if (enemyBase.stunned)
            return;

        if (MovingRight)
            movement.MoveRight(moveSpeed);
        else
            movement.MoveLeft(moveSpeed);
    }
}
