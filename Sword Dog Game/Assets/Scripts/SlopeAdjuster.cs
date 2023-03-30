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

    private Vector2 groundCheckSpot = new Vector2();

    public float lastSlope;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<Collider2D>();
        groundCheck ??= GetComponentInChildren<CollisionsTracker>();

        colliderSize = cldr.bounds.size;

        upperLeftCorner = new Vector2((-cldr.bounds.extents.x * 1) + cldr.offset.x, cldr.bounds.extents.y + cldr.offset.y);
        upperRightCorner = new Vector2((cldr.bounds.extents.x * 1) + cldr.offset.x, upperLeftCorner.y);

        groundCheckSpot = (Vector2)(groundCheck.transform.position - transform.position) + Vector2.up * groundCheck.cldr.offset.y;

        groundCheck.triggerEnter += checkIfLanding;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SlopeCheckHorizontal(upperLeftCorner, upperRightCorner);
        capRotation();
        transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);
        lastSlope = slopeSideAngle;
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

        if(leftHit.point != default)
            Debug.DrawLine(upperLeftCorner + (Vector2)transform.position, leftHit.point, Color.red);
        if(rightHit.point != default)
            Debug.DrawLine(upperRightCorner + (Vector2)transform.position, rightHit.point, Color.red);

        //if (leftHit.point == Vector2.zero || rightHit.point == Vector2.zero)
        //    return;


        Vector2 leftSide = upperLeftCorner;
        Vector2 rightSide = upperRightCorner;
        float yLevel = groundCheckSpot.y + transform.position.y;

        bool needRerun = false;

        //If there is not a hit for left point, scan across just below the bottom of the collider to find one
        if (leftHit.point == Vector2.zero)
        {
            RaycastHit2D groundFinder = Physics2D.Raycast(new Vector2(upperLeftCorner.x + transform.position.x, yLevel), Vector2.right, upperRightCorner.x - upperLeftCorner.x, whatIsGround);

            Debug.DrawLine(new Vector2(upperLeftCorner.x + transform.position.x, yLevel), new Vector3(groundFinder.point.x, yLevel), Color.magenta);
            leftSide.x = groundFinder.point.x - transform.position.x;
            needRerun = true;
            //Prevent jumpyness on bumpy and tall slopes by ignoring really tall slopes
            if (groundFinder.distance > (upperRightCorner.x - upperLeftCorner.x) * .8f)
                return;

        }
        //If there is not a hit for right point, scan across just below the bottom of the collider to find one
        if (rightHit.point == Vector2.zero)
        {
            RaycastHit2D groundFinder = Physics2D.Raycast(new Vector2(upperRightCorner.x + transform.position.x, yLevel), Vector2.left, upperRightCorner.x - upperLeftCorner.x, whatIsGround);

            Debug.DrawLine(new Vector2(upperRightCorner.x + transform.position.x, yLevel), new Vector3(groundFinder.point.x, yLevel), Color.magenta);
            rightSide.x = groundFinder.point.x - transform.position.x;
            needRerun = true;
            //Prevent jumpyness on bumpy and tall slopes by ignoring really tall slopes
            if (groundFinder.distance > (upperRightCorner.x - upperLeftCorner.x) * .8f)
                return;
        }

        //Recursive call for if edges have changed
        if(needRerun)
            SlopeCheckHorizontal(leftSide, rightSide, runs + 1);

        //Determine which hit was closest to the hitbox (highest)
        RaycastHit2D farHit = rightHit.distance > leftHit.distance ? rightHit : leftHit;
        RaycastHit2D nearHit = rightHit.distance < leftHit.distance ? rightHit : leftHit;



        int right = leftHit.distance < rightHit.distance ? -1 : 1;

        //Scan across partway between highest and lowest hits to find intermediate
        Vector2 acrossCheckSpot = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2);
        Vector2 acrossCheck2 = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2.4f);
        RaycastHit2D across = Physics2D.Raycast(acrossCheckSpot,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        RaycastHit2D across2 = Physics2D.Raycast(acrossCheck2,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        
        if(acrossCheckSpot != default)
            Debug.DrawLine(across.point, acrossCheckSpot, Color.green);
        if(acrossCheck2 != default)
            Debug.DrawLine(across2.point, acrossCheck2, Color.green);

        
        //Calculate unsmoothed slope. Get how far across each across cast made it
        float unsmoothedSlope = Mathf.Atan((rightHit.point.y - leftHit.point.y) / (rightHit.point.x - leftHit.point.x)) * Mathf.Rad2Deg;
        float acrossPercent = across.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));
        float acrossPercent2 = across2.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));


        bool onLedge = false;
        //If slope is negative or almost 0 (reading the underside of something)
        if (acrossPercent2 - acrossPercent < .01)
        {
            slopeSideAngle = 0;
            //Reevaluate to the top of what it is ontop of (recursive call)
            if (acrossPercent > .01f)
            {
                if (right == 1)
                    SlopeCheckHorizontal(new Vector2(upperLeftCorner.x + colliderSize.x * acrossPercent, upperLeftCorner.y), upperRightCorner, runs + 1);
                else
                    SlopeCheckHorizontal(upperLeftCorner, new Vector2(upperRightCorner.x - colliderSize.x * acrossPercent, upperRightCorner.y), runs + 1);
            }
            onLedge = true;
        }
        //Apply smoothing
        if (!float.IsNaN(unsmoothedSlope) && !onLedge)
            slopeSideAngle = unsmoothedSlope * Mathf.Lerp(1, 0, (Mathf.Abs((acrossPercent / .5f) - 1)));

        //If not grounded, rotate to match vertical velocity
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

        //Record lastgrounded data
        if (isGrounded)
            lastGroundedSlope = slopeSideAngle;
        else
        {
            lastMidairVelocity = rb.velocity;
            lastUngroundedSlope = slopeSideAngle;
        }
        //"Animation" for landing (interpolate between ground's rotation and midair rotation for a few seconds)
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
    public void capRotation()
    {
        float angleDifference = Mathf.DeltaAngle(slopeSideAngle, lastSlope);
        if (Mathf.Abs(angleDifference) > 60 * Time.deltaTime)
        {
            if (angleDifference < 0)
                slopeSideAngle = lastSlope + 60 * Time.deltaTime;
            else
                slopeSideAngle = lastSlope + -60 * Time.deltaTime;
        }
    }
}
