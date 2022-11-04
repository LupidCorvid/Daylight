using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoverAI : BaseAI
{

    public float maxSpeed = 20;

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

        testForLedge(rb.velocity.x > 0);
        moveInDirection(target.position);
    }

    //determines if the AI should jump when moving in a direction
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
            Jump();
        return false;
    }

    private void moveInDirection(Vector3 target)
    {
        if(target.x > transform.position.x)
        {
            if(rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.right * Time.deltaTime * 500 * moveSpeed);
        }
        else
        {
            if(rb.velocity.x > -maxSpeed)
                rb.AddForce(Vector2.left * Time.deltaTime * 500 * moveSpeed);
        }
        if(target.y - transform.position.y > 4)//Make sure is grounded 
        {
            Jump();
        }
    }
    public void Jump()
    {
        foreach (Collider2D collision in enemyBase.GetComponentInChildren<CollisionsTracker>().triggersInContact)
        {
            if (Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("Terrain"))
            {
                rb.AddForce(Vector2.up * 100);
                return;
            }
        }
    }

}
