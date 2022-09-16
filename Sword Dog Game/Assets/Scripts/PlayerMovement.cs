using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject instance;
    private Rigidbody2D rb;
    private Animator anim;
    public bool facingRight, trotting, isGrounded, wasGrounded, isJumping, holdingJump;
    public float moveX, beenOnLand, lastOnLand, jumpTime, jumpCooldown, timeSinceJumpPressed;
    private int stepDirection;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Update()
    {
        // grab movement input from horizontal axis
        moveX = Input.GetAxisRaw("Horizontal");

        // fix input spam breaking trot state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idleAnim"))
        {
            trotting = false;
        }

        // start trotting if player gives input
        if (moveX != 0 && !trotting)
        {
            anim.SetTrigger("trot");
            trotting = true;
        }

        // jump code
        Jump();

        if (Input.GetButtonUp("Jump"))
        {
            holdingJump = false;
            jumpCooldown = 0.0f;
        }
    }

    void FixedUpdate()
    {
        // flip sprite depending on direction of input
        if ((moveX < 0 && facingRight) || (moveX > 0 && !facingRight))
        {
            Flip();
        }

        // apply velocity, dampening between current and target velocity for smooth movement
        Vector3 targetVelocity = new Vector2(moveX * speed, rb.velocity.y);

        // sloped movement
        if (isOnSlope && isGrounded && !isJumping && canWalkOnSlope)
        {
            targetVelocity.Set(moveX * speed * -slopeNormalPerp.x, moveX * speed * -slopeNormalPerp.y, 0.0f);
        }

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

        if (isJumping)
        {
            jumpTime += Time.fixedDeltaTime;
        }

        //hold jump distance extentions
        if (holdingJump && isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce / 500f / jumpTime));
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
        if (moveX == 0)
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
        if (moveX != 0) {
            stepDirection = 1;
        }

        CalculateSpeedMultiplier();
    }

    void CalculateSpeedMultiplier()
    {
        // calculate trot "speed" multiplier
        float speedMultiplier = rb.velocity.x / speed;

        // disregard direction of movement
        speedMultiplier = Mathf.Abs(speedMultiplier);

        // clamp to minimum speed + scale magnitude of normal speed (makes for smoother transitions)
        speedMultiplier = Mathf.Max(0.7f, 1.1f * speedMultiplier);

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
        RaycastHit2D front = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D back = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
        if (front)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(front.normal, Vector2.up);
        }
        else if (back)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(back.normal, Vector2.up);
        }
        else
        {
            isOnSlope = false;
            slopeSideAngle = 0.0f;
        }
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

        // float currentRotZ = transform.localRotation.eulerAngles.z;
        // float rotZ = Mathf.Lerp(currentRotZ, slopeDownAngle, 0.2f);
        // transform.rotation = Quaternion.Euler(0, 0, rotZ);

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
        if (Input.GetButton("Jump"))
        {
            if (!isJumping)
            {
                holdingJump = true;
            }

            timeSinceJumpPressed = 0.0f;
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

        timeSinceJumpPressed += Time.deltaTime;
    }
}
