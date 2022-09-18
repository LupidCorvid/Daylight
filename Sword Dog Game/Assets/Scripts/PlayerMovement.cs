using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject instance;
    public static PlayerMovement controller;
    private Rigidbody2D rb;
    private Animator anim;
    public bool facingRight, trotting, isGrounded, wasGrounded, isJumping, holdingJump;
    public float moveX, prevMoveX, beenOnLand, lastOnLand, jumpTime, jumpCooldown, timeSinceJumpPressed;
    public int stepDirection, stops;
    private Vector3 targetVelocity, velocity = Vector3.zero;
    [SerializeField] private float speed = 4f;

    // Radius of the overlap circle to determine if grounded
    const float groundedRadius = 0.2f;

    // A mask determining what is ground to the character
    [SerializeField] public LayerMask whatIsGround;

    // Positions marking where to check if the player is grounded
    [SerializeField] public Transform[] groundChecks;

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
    public int slopeSamples = 2;

    public bool dead, resetting, invincible;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cldr = GetComponent<Collider2D>();
        colliderSize = GetComponent<BoxCollider2D>().size;
        timeSinceJumpPressed = 0.2f;

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
        if (moveX != 0 && !trotting && rb.velocity.x != 0)
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
            jumpCooldown = 0.0f;
        }

        // TODO REMOVE - debug cinematic bars keybind
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameObject.FindObjectOfType<CinematicBars>().Show(500, .3f);
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
        Vector3 targetVelocity = new Vector2(moveX * speed, rb.velocity.y);

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
            if (holdingJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce / 400f / jumpTime));
            }
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

        // clamp to minimum speed + scale magnitude of normal speed (makes for smoother transitions)
        speedMultiplier = Mathf.Max(0.7f, 1.1f * speedMultiplier / 4);

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
        if (slopeSamples < 2)
        {
            Debug.LogError("SlopeCheck needs at least 2 samples!");
            return;
        }
        List<Vector3> samples = new List<Vector3>(slopeSamples);
        float xStep = (upperRightCorner.x - upperLeftCorner.x) / (slopeSamples - 1);
        for(int i = 0; i < slopeSamples; i++)
        {
            Vector2 position = new Vector2(upperLeftCorner.x + (xStep * i), upperLeftCorner.y);
            RaycastHit2D hit = Physics2D.Raycast((position) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + cldr.bounds.size.y, whatIsGround);
            //Debug.DrawLine(position + (Vector2)transform.position, position + (Vector2)transform.position + Vector2.down * (slopeCheckDistance + cldr.bounds.size.y));
            //Debug.DrawLine(position + (Vector2)transform.position, hit.point, Color.red);
            samples.Add(hit.point);
            //if (hit.point != new Vector2(0, 0))
            //    samples[i] = hit.point;
            //else
            //    samples[i] = (position + Vector2.down * (/*slopeCheckDistance + */cldr.bounds.size.y)) + (Vector2)transform.position;
            Debug.DrawLine(position + (Vector2)transform.position, position + (Vector2)transform.position + Vector2.down * (slopeCheckDistance + cldr.bounds.size.y));
            Debug.DrawLine(position + (Vector2)transform.position, samples[i], Color.red);

        }
        for(int i = samples.Count - 1; i >= 0; i--)
        {
            if(samples[i] == new Vector3(0,0,0))
            {
                samples.RemoveAt(i);
            }
        }
        if(samples.Count == 0)
        {
            return;
        }
        Vector2 totalSlope = default;
        for (int i = 1; i < samples.Count; i++)
        {
            //totalSlope += new Vector2((samples[i].y - samples[i - 1].y), (samples[i].x - samples[i - 1].x));
            totalSlope += new Vector2(samples[i].y - samples[i - 1].y, samples[i].x - samples[i - 1].x);
        }
        Vector2 finalSlope = new Vector2( totalSlope.x, totalSlope.y * 1.0f / (samples.Count));
        int posNeg = (finalSlope.y/finalSlope.x) > 0 ? 1 : -1;
        slopeSideAngle = Vector2.Angle(finalSlope, Vector2.up) * posNeg;
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (hit)
        {
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

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        foreach (Transform groundCheck in groundChecks)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag == "Ground")
                {
                    isGrounded = true;
                    lastOnLand = 0f;

                    if (!wasGrounded && jumpCooldown <= 0.1f)
                        jumpCooldown = 0.05f;
                }
            }
        }

        if (!isGrounded)
        {
            beenOnLand = 0f;
        }
        else 
        {
            if (beenOnLand < 5f)
                beenOnLand += Time.fixedDeltaTime;
            if (jumpTime > 0.1f && !(rb.velocity.y > 0f))
            {
                isJumping = false;
                jumpTime = 0f;
            }
            if (jumpCooldown > 0f)
                jumpCooldown -= Time.fixedDeltaTime;
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
        if (timeSinceJumpPressed < 0.2f && (isGrounded || lastOnLand < 0.2f) && jumpCooldown <= 0f && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            // Add a vertical force to the player
            isGrounded = false;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce)); //force added during a jump
        }

        if (timeSinceJumpPressed < 1f)
            timeSinceJumpPressed += Time.deltaTime;
    }
}