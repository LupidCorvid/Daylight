using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoverAI : BaseAI
{

    public float maxSpeed = 20;
    public float stopRange = 2f;

    enum states
    {
        attacking,
        hunting,
        stunned,
        idle,
        wandering
    }

    public BasicMoverAI(EnemyBase enemy) : base(enemy)
    {

    }
    public override void Update()
    {
        base.Update();

        if (enemyBase.stunned)
            return;

        testForLedge(rb.velocity.x > 0);
        moveInDirection(target.position);
    }

    public override void FoundTarget(Entity newTarget)
    {
        base.FoundTarget(newTarget);
        if (targetEntity is Player)
        {
            AkUnitySoundEngine.PostEvent("MonstersAware", AudioManager.WwiseGlobal);
        }
    }

    //determines if the AI should jump when moving in a direction
    //Ignore this and go for a node-based system instead
    //Use colliders to the left and right to check for both walls and if the player is in range
    private bool testForLedge(bool movingRight)
    {
        //This current method will not work for this as it assumes that hitboxes are both constant and square
        int negative = movingRight ? 1 : -1;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, new Vector2(2 * negative, -2), LayerMask.GetMask("Terrain"));
        if (hit.point == default)
        {
            hit.point = transform.position + new Vector3(2 * negative, -2);
            hit.distance = new Vector3(2 * negative, -2).magnitude;
        }
        Debug.DrawLine(transform.position, hit.point);
        if (hit.distance > 3)
            movement.Jump();
        return false;
    }

    private void moveInDirection(Vector3 target)
    {
        if(target.x - stopRange > transform.position.x)
        {
            movement.MoveRight();
        }
        else if (target.x + stopRange < transform.position.x)
        {
            movement.MoveLeft();
        }
        if(target.y - transform.position.y > 4)//Make sure is grounded 
        {
            movement.Jump();
        }
    }

}
