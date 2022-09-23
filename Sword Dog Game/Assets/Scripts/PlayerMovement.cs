using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject instance;
    public static PlayerMovement controller;
    private Rigidbody2D rb;
    private Animator anim;
    private bool trotting, wasGrounded, holdingJump;
    public bool facingRight, isGrounded, isJumping;
    private float moveX, prevMoveX, beenOnLand, lastOnLand, jumpTime, jumpSpeedMultiplier, timeSinceJumpPressed;
    private int stepDirection, stops;
    private Vector3 targetVelocity, velocity = Vector3.zero;
    [SerializeField] private float speed = 4f;

    // Radius of the overlap circle to determine if grounded
    const float groundedRadius = 0.2f;

    // A mask determining what is ground to the character
    [SerializeField] public LayerMask whatIsGround;

    // Positions marking where to check if the player is grounded
    //[SerializeField] public Transform[] groundChecks;
    public CollisionsTracker groundCheck;

    // Amount of force added when the player jumps
    [SerializeField] private float jumpForce = 2000f;

    // How much to smooth out movement
    [Range(0, .3f)][SerializeField] private float movementSmoothing = 0.05f;

    // Slope variables
    private Vector2 colliderSize;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerp;
    private bool isOnSlope, canWalkOnSlope;
    public PhysicsMaterial2D slippery, friction;

    Collider2D cldr;

    Vector2 upperLeftCorner;
    Vector2 upperRightCorner;

    public bool dead, resetting, invincible;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cldr = GetComponent<Collider2D>();
        colliderSize = GetComponent<BoxCollider2D>().size;
        timeSinceJumpPressed = 0.2f;
        jumpSpeedMultiplier = 1.0f;

        stepDirection = 1;
        facingRight = true;

        // Singleton design pattern
        if (instance != null && instance != this)
        {
            // Destroy(gameObject);
        }
        else
        {
            controller = this;
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        upperLeftCorner = new Vector2(-cldr.bounds.extents.x + cldr.offset.x, cldr.bounds.extents.y + cldr.offset.y);
        upperRightCorner = new Vector2(cldr.bounds.extents.x + cldr.offset.x, upperLeftCorner.y);
    }

    IEnumerator RemoveStop()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        stops--;
    }

    void Update()
    {
        // remember previous movement input
        prevMoveX = moveX;

        // grab movement input from horizontal axis
        moveX = Input.GetAxisRaw("Horizontal");

        // track stops per second
        if (prevMoveX != 0 && moveX == 0)
        {
            stops++;
            StartCoroutine("RemoveStop");
        }

        // fix input spam breaking trot state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idleAnim"))
        {
            trotting = false;
        }

        // start trotting if player gives input and is moving
        if (moveX != 0 && !trotting && rb.velocity.x != 0 && !isJumping)
        {
            anim.SetTrigger("trot");
            trotting = true;
        }

        // jump code
        Jump();

        // release jump
        if (Input.GetButtonUp("Jump"))
        {
            holdingJump = false;
        }

        // TODO REMOVE - debug cinematic bars keybind
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameObject.FindObjectOfType<CinematicBars>().Show(200, .3f);
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            GameObject.FindObjectOfType<CinematicBars>().Hide(.3f);
        }

    }

    void FixedUpdate()
    {
        // flip sprite depending on direction of input
        if ((moveX < 0 && facingRight) || (moveX > 0 && !facingRight))
        {
            Flip();
        }

        // calculate target velocity
        Vector3 targetVelocity = new Vector2(moveX * speed * jumpSpeedMultiplier, rb.velocity.y);

        // sloped movement
        if (isOnSlope && isGrounded && !isJumping && canWalkOnSlope)
        {
            targetVelocity.Set(moveX * speed * -slopeNormalPerp.x, moveX * speed * -slopeNormalPerp.y, 0.0f);
        }

        // apply velocity, dampening between current and target
        if (moveX == 0.0 && rb.velocity.x != 0.0f)
        {
            if (canWalkOnSlope)
                GetComponent<BoxCollider2D>().sharedMaterial = friction;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing * 2.5f);
        }
        else
        {
            GetComponent<BoxCollider2D>().sharedMaterial = slippery;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        }

        // calculate speed multiplier for trot animation
        CalculateSpeedMultiplier();

        // check if player is on ground
        CheckGround();

        // rotate player based on slope
        transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);

        // hold jump distance extentions
        if (isJumping)
        {
            jumpTime += Time.fixedDeltaTime;
            jumpSpeedMultiplier = 1f + 2f/(10f * jumpTime + 4f);
            if (holdingJump)
            {
                jumpSpeedMultiplier *= 1.25f;
                rb.AddForce(new Vector2(0f, jumpForce / 400f / jumpTime));
            }
        }
        else 
        {
            jumpSpeedMultiplier = Mathf.Lerp(jumpSpeedMultiplier, 1, 0.3f);
        }

        // trigger fall animation
        if (!isJumping && rb.velocity.y < 0 && !isGrounded)
        {
            anim.SetTrigger("fall");
        }
        if (isGrounded)
        {
            anim.ResetTrigger("fall");
        }
    }

    // flips sprite when player changes movement direction
    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
    }

    // stops trotting on specific frames if player has released input
    public void StopTrot(int frame)
    {
        if (((moveX == 0 || (moveX != 0 && rb.velocity.x == 0)) && stops <= 2) // if either not giving input or giving input against a barrier *and* hasn't stopped moving more than twice in the last second
            || (stops > 2 && Mathf.Abs(rb.velocity.x) < 0.01f)) // or has stopped moving more than twice in the last second and moving sufficiently slowly
        {
            switch (frame)
            {
                // stop here
                case 0 or 5 or 6 or 11:
                    if (!isJumping)
                        anim.SetTrigger("trot");
                    trotting = false;
                    stepDirection = 1;
                    break;

                // step backwards here
                case 1 or 2 or 7 or 8:
                    stepDirection = -1;
                    break;

                // step forwards here
                case 3 or 4 or 9 or 10:
                    stepDirection = 1;
                    break;
            }
        }
        // step forwards again if still moving
        else
        {
            stepDirection = 1;
        }

        CalculateSpeedMultiplier();
    }

    void CalculateSpeedMultiplier()
    {
        // calculate trot "speed" multiplier
        float speedMultiplier = rb.velocity.x;

        // disregard direction of movement
        speedMultiplier = Mathf.Abs(speedMultiplier);

        // scale + clamp magnitude of normal speed (makes for smoother transitions)
        speedMultiplier = 1.1f * speedMultiplier / 4;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.7f, 1.5f);

        // multiply by "step direction" - determines whether animation plays forwards/backwards for smoother stopping
        speedMultiplier *= stepDirection;

        // send speed multiplier to animator parameter
        anim.SetFloat("speed", speedMultiplier);
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D leftHit = Physics2D.Raycast((upperLeftCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast((upperRightCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);

        if (leftHit.point == new Vector2(0, 0))
        {
            leftHit.point = upperLeftCorner + (Vector2)transform.position + (Vector2.down * (slopeCheckDistance + colliderSize.y));
            leftHit.distance = (Vector2.Distance(upperLeftCorner + (Vector2)transform.position, leftHit.point));
        }
        if (rightHit.point == new Vector2(0, 0))
        {
            rightHit.point = upperRightCorner + (Vector2)transform.position + (Vector2.down * (slopeCheckDistance + colliderSize.y));
            rightHit.distance = (Vector2.Distance(upperRightCorner + (Vector2)transform.position, rightHit.point));
        }

        Debug.DrawLine(upperLeftCorner + (Vector2)transform.position, leftHit.point, Color.red);
        Debug.DrawLine(upperRightCorner + (Vector2)transform.position, rightHit.point, Color.red);

        if (leftHit.distance == rightHit.distance)
        {
            slopeSideAngle = 0;
            return;
        }

        RaycastHit2D farHit = rightHit.distance > leftHit.distance ? rightHit : leftHit;
        RaycastHit2D nearHit = rightHit.distance < leftHit.distance ? rightHit : leftHit;

        int right = leftHit.distance < rightHit.distance ? -1 : 1;
        
        Vector2 acrossCheckSpot = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2);
        RaycastHit2D across = Physics2D.Raycast(acrossCheckSpot, 
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        Debug.DrawLine(across.point, acrossCheckSpot, Color.green);

        float unsmoothedSlope = Mathf.Atan((rightHit.point.y - leftHit.point.y)/(rightHit.point.x - leftHit.point.x)) * Mathf.Rad2Deg;
        float acrossPercent = across.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));
        slopeSideAngle = unsmoothedSlope * Mathf.Lerp(1, 0, (Mathf.Abs((acrossPercent/.5f) - 1)));

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        anim.SetBool("ground_close", false);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit)
        {
            anim.SetBool("ground_close", true);
            // Debug.DrawRay(hit.point, hit.normal, Color.red, 0.01f, false);
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;
            // Debug.DrawRay(hit.point, slopeNormalPerp, Color.yellow, 0.01f, false);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
    }

    void CheckGround()
    {
        SlopeCheck();

        lastOnLand = Mathf.Clamp(lastOnLand + Time.fixedDeltaTime, 0, 20f);

        bool wasGrounded = isGrounded;
        isGrounded = false;

        //// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        //for (int i = 0; i < groundChecks.Length; i++)
        //{
        //    Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecks[i].position, groundedRadius, whatIsGround);
        //    for (int j = 0; j < colliders.Length; j++)
        //    {
        //        if (colliders[j].gameObject != gameObject && colliders[j].gameObject.tag == "Ground")
        //        {
        //            if (i > 1 && isJumping)
        //                anim.SetBool("ground_close", true);
        //            else {
        //                isGrounded = true;
        //                lastOnLand = 0f;
        //            }
        //        }
        //    }
        //}
        foreach(Collider2D collision in groundCheck.triggersInContact)
        {
            //Debug.Log(LayerMask.GetMask("Terrain"));
            if(Mathf.Pow(2, collision.gameObject.layer) == whatIsGround)
            {
                anim.SetBool("grounded", true);
                isGrounded = true;
                lastOnLand = 0f;
                break;
            }
        }
        if (isJumping && jumpTime < 0.1f)
            anim.SetBool("grounded", false);
        else
            anim.SetBool("grounded", isGrounded);


        if (!isGrounded)
        {
            beenOnLand = 0f;
        }
        else 
        {
            if (beenOnLand < 5f)
                beenOnLand += Time.fixedDeltaTime;
            if (!(rb.velocity.y > 0f) && isJumping)
            {
                jumpSpeedMultiplier = 1f;
                isJumping = false;
                jumpTime = 0f;
            }
        }
    }

    void Jump()
    {
        // if player presses jump button
        if (Input.GetButtonDown("Jump"))
        {
            timeSinceJumpPressed = 0.0f;
        }

        if (Input.GetButton("Jump") && timeSinceJumpPressed < 0.2f)
        {
            if (!isJumping)
            {
                holdingJump = true;
            }
        }

        // incorporates coyote time and input buffering
        if (timeSinceJumpPressed < 0.2f && (isGrounded || lastOnLand < 0.2f) && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            // Add a vertical force to the player
            isGrounded = false;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce)); // force added during a jump
            anim.SetTrigger("jump");
        }

        if (timeSinceJumpPressed < 1f)
            timeSinceJumpPressed += Time.deltaTime;
    }
}