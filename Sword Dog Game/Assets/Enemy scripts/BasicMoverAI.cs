using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoverAI : BaseAI
{
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
        if (hit.distance > 1.2)
            Jump();
        return false;
    }

    private void moveInDirection(Vector3 target)
    {
        if(target.x > transform.position.x)
        {
            rb.AddForce(Vector2.right * Time.deltaTime * 1000);
        }
        else
        {
            rb.AddForce(Vector2.left * Time.deltaTime * 1000);
        }
        if(target.y - transform.position.y > 3)//Make sure is grounded 
        {
            Jump();
        }
    }
    public void Jump()
    {
        if(Physics2D.Raycast(transform.position, Vector2.down, .25f, LayerMask.GetMask("Terrain")))
        rb.AddForce(Vector2.up * 20);
    }

}
