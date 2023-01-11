using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    //This class is to only be used for generating movement. Minimize reading it does and save that for AI
    public SlopeAdjuster slopeChecker;

    public PhysicsMaterial2D slippery;
    public PhysicsMaterial2D friction;

    protected Vector3 velocity;

    protected Collider2D cldr;

    protected Rigidbody2D rb;

    //flipping and animating based on movement
    public bool applyVisualModifications = false;
    [Range(0, .3f)] [SerializeField] 
    protected float movementSmoothing = 0.05f;

    //Need to make a version where enemies dont get to turn immediately
    public bool snappyDirectionChange = true;

    public virtual void Start()
    {
        slopeChecker ??= GetComponent<SlopeAdjuster>();
        cldr ??= GetComponent<Collider2D>();
        rb ??= GetComponent<Rigidbody2D>();
    }

    public void MoveRight(float targetVelocity = 4)
    {
        cldr.sharedMaterial = slippery;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(targetVelocity, rb.velocity.y), ref velocity, movementSmoothing);
    }

    public void MoveLeft(float targetVelocity = 4)
    {
        cldr.sharedMaterial = slippery;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(-targetVelocity, rb.velocity.y), ref velocity, movementSmoothing);
    }

    public void Jump(float jumpForce = 50)
    {
        if (!slopeChecker.isGrounded)
            return;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0f, jumpForce * rb.mass)); // force added during a jump

    }

    //Could try to account for velocity. As of now only predictable when standing still
    //Meant to be used for the ai to clear gaps
    public void TargetedJump(Vector2 target)
    {
        Vector2 relTar = target - (Vector2)transform.position;
        float projSpeed = 5;
        float grav = rb.gravityScale * 9.8f;
        float angle;
        angle = Mathf.Atan((Mathf.Sqrt(Mathf.Pow(projSpeed, 4) - grav * (grav * Mathf.Pow(relTar.x, 2) + 2 * relTar.y
                          * Mathf.Pow(projSpeed, 2))) + Mathf.Pow(projSpeed, 2)) / (grav * relTar.x));
        if (relTar.x < 0)
            angle += Mathf.PI;
        if(!float.IsNaN(angle))
            rb.velocity = new Vector2(projSpeed * Mathf.Cos(angle), projSpeed * Mathf.Sin(angle));
    }

    public void NotMoving()
    {
        cldr.sharedMaterial = friction;
    }

    //Only for controlling animator and reading when landing and such. LEAVE ALL CONTROL TO AI CONTROLLER
    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {

    }

    /* if (moveX == 0.0 && rb.velocity.x != 0.0f)
        {
            if (canWalkOnSlope)
                GetComponent<BoxCollider2D>().sharedMaterial = friction;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing * 2.5f);
        }
        else
        {
            GetComponent<BoxCollider2D>().sharedMaterial = slippery;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }*/
}
