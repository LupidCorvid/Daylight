using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeAdjuster : MonoBehaviour
{
    public Vector2 upperRightCorner;
    public Vector2 upperLeftCorner;

    public float slopeCheckDistance;
    public Vector2 colliderSize;

    float lastGroundedSlope = 0;
    float slopeSideAngle = 0;
    float lastUngroundedSlope = 0;

    public float lastLand;
    public float landAnimTime = 1.25f;
    
    public Vector2 lastMidairVelocity;

    public LayerMask whatIsGround;

    public bool isGrounded = true;

    public Rigidbody2D rb;
    public Collider2D cldr;

    public CollisionsTracker groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<Collider2D>();
        groundCheck ??= GetComponentInChildren<CollisionsTracker>();

        colliderSize = cldr.bounds.size;

        upperLeftCorner = new Vector2((-cldr.bounds.extents.x * 1) + cldr.offset.x, cldr.bounds.extents.y + cldr.offset.y);
        upperRightCorner = new Vector2((cldr.bounds.extents.x * 1) + cldr.offset.x, upperLeftCorner.y);

        groundCheck.triggerEnter += checkIfLanding;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SlopeCheckHorizontal(upperLeftCorner, upperRightCorner);
        transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);
        groundedCheck();
        if (!isGrounded)
        {
            lastMidairVelocity = rb.velocity;
        }
        else
            lastGroundedSlope = slopeSideAngle;
    }

    public void checkIfLanding(Collider2D collision)
    {
        if (Mathf.Pow(2, collision.gameObject.layer) == whatIsGround && !isGrounded)
        {
            lastLand = Time.time;
        }
    }

    private void SlopeCheckHorizontal(Vector2 upperLeftCorner, Vector2 upperRightCorner, int runs = 0)
    {
        if (runs > 2)
            return;
        RaycastHit2D leftHit = Physics2D.Raycast((upperLeftCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast((upperRightCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);

        Debug.DrawLine(upperLeftCorner + (Vector2)transform.position, leftHit.point, Color.red);
        Debug.DrawLine(upperRightCorner + (Vector2)transform.position, rightHit.point, Color.red);

        if (leftHit.point == Vector2.zero || rightHit.point == Vector2.zero)
            return;

        RaycastHit2D farHit = rightHit.distance > leftHit.distance ? rightHit : leftHit;
        RaycastHit2D nearHit = rightHit.distance < leftHit.distance ? rightHit : leftHit;



        int right = leftHit.distance < rightHit.distance ? -1 : 1;

        Vector2 acrossCheckSpot = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2);
        Vector2 acrossCheck2 = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2.4f);
        RaycastHit2D across = Physics2D.Raycast(acrossCheckSpot,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        RaycastHit2D across2 = Physics2D.Raycast(acrossCheck2,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        Debug.DrawLine(across.point, acrossCheckSpot, Color.green);
        Debug.DrawLine(across2.point, acrossCheck2, Color.green);

        

        float unsmoothedSlope = Mathf.Atan((rightHit.point.y - leftHit.point.y) / (rightHit.point.x - leftHit.point.x)) * Mathf.Rad2Deg;
        float acrossPercent = across.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));
        float acrossPercent2 = across2.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));


        bool onLedge = false;
        //Makessure that it is not reading the slope of the underside of a slope by not taking abs val. 
        if (acrossPercent2 - acrossPercent < .01)//If issues arise get abs value
        {
            slopeSideAngle = 0;
            if (acrossPercent > .01f)
            {
                if (right == 1)
                    SlopeCheckHorizontal(new Vector2(upperLeftCorner.x + colliderSize.x * acrossPercent, upperLeftCorner.y), upperRightCorner, runs + 1);
                else
                    SlopeCheckHorizontal(upperLeftCorner, new Vector2(upperRightCorner.x - colliderSize.x * acrossPercent, upperRightCorner.y), runs + 1);
            }
            onLedge = true;
        }
        if (!float.IsNaN(unsmoothedSlope) && !onLedge)
            slopeSideAngle = unsmoothedSlope * Mathf.Lerp(1, 0, (Mathf.Abs((acrossPercent / .5f) - 1)));

        if (!isGrounded)
        {
            const float ROTATION_INTENSITY = 0;
            int negative = 1;
            //if (!facingRight)
            //    negative = -1;
            float rotationAmount = (rb.velocity.y * Time.deltaTime * ROTATION_INTENSITY * negative);
            //rotationAmount = Mathf.Clamp(rotationAmount, -75, 75);
            slopeSideAngle = lastGroundedSlope + rotationAmount;
        }

        if (isGrounded)
            lastGroundedSlope = slopeSideAngle;
        else
        {
            lastMidairVelocity = rb.velocity;
            lastUngroundedSlope = slopeSideAngle;
        }
        if (isGrounded)
        {
            if (lastLand + landAnimTime > Time.time)
            {
                slopeSideAngle = Mathf.Lerp(lastUngroundedSlope, slopeSideAngle, Mathf.Clamp((Time.time - lastLand) * Mathf.Abs(lastMidairVelocity.y) / (landAnimTime), 0, 1));
            }
        }

    }
    
    void groundedCheck()
    {
        isGrounded = false;
        foreach (Collider2D collision in groundCheck.triggersInContact)
        {
            //Debug.Log(LayerMask.GetMask("Terrain"));
            if (Mathf.Pow(2, collision.gameObject.layer) == whatIsGround)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
