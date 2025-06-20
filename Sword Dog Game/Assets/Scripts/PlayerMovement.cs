using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;
    private bool waitingToTurn, holdingJump;
    public bool isGrounded, isRoofed, isJumping, isFalling, trotting, isSprinting, canResprint, isSkidding, wallOnRight, wallOnLeft, behindGrounded, finishedReverseTurnThisFrame = false, isDashing = false;
    public Vector2 bottom;
    public static bool created = false;
    private float beenLoaded = 0.0f, minLoadTime = 0.1f;
    private Vector3 resetPosition;
    private Quaternion resetRotation;
    public bool canTurn = true;
    public bool canSprint = true;
    public bool canDash = false;
    public bool blackout = false;
    public static bool isTurning = false, reversedTurn = false;
    public PlayerAttack pAttack;

    public PlayerInput inputManager;

    public bool stopMovement;

    public bool waterRotation;

    public bool externalControl = false;

    public bool facingRight
    {
        get
        {
            return transform.localScale.x > 0;
        }
        set
        {
            int neg = 1;
            if (value && created)
                neg *= -1;

            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * neg, transform.localScale.y, transform.localScale.z);
            entityBase.facingDir = new Vector2(neg, 0);
        }
    }
    public bool intendedFacingRight;

    public float lastLandHeight;
    private float moveX, prevMoveX, beenOnLand, lastOnLand, jumpTime, jumpSpeedMultiplier, timeSinceJumpPressed, timeSinceJump, fallTime, timeSinceSprint, timeIdle;
    public float sprintSpeedMultiplier;
    public float maxSprintSpeedMultiplier = 2.5f;
    private int stepDirection, stops;
    private Vector3 targetVelocity, velocity = Vector3.zero;
    public Entity entityBase;

    //[SerializeField] private float speed = 4f;
    public float speed
    {
        get
        {
            return entityBase.moveSpeed;
        }
        set
        {
            entityBase.moveSpeed.baseValue = value;
        }
    }

    [SerializeField] private GameObject barkFXPrefab;
    [SerializeField] private Transform mouth;

    // Radius of the overlap circle to determine if grounded
    const float groundedRadius = 0.2f;

    // A mask determining what is ground to the character
    [SerializeField] public LayerMask whatIsGround;

    // Positions marking where to check if the player is grounded
    //[SerializeField] public Transform[] groundChecks;
    public CollisionsTracker groundCheck, behindGroundCheck;
    public CollisionsTracker roofCheck;

    // Amount of force added when the player jumps
    [SerializeField] private float jumpForce = 2000f;

    // Amount of force added when the player dashes
    [SerializeField] private float dashVel = 10;

    // How much to smooth out movement
    [Range(0, .3f)][SerializeField] private float movementSmoothing = 0.05f;

    // Slope variables
    private Vector2 colliderSize;
    // [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerp;
    public bool isOnSlope, canWalkOnSlope;
    public PhysicsMaterial2D slippery, friction, immovable;
    public float calculatedSpeed = 4.0f;
    public float sprintWindUpPercent = 1.0f;

    [SerializeField] private Collider2D cldr1, cldr2;
    public Collider2D cldr;

    Vector2 upperLeftCorner;
    Vector2 upperRightCorner;

    public bool resetting, invincible;

    float lastGroundedSlope = 0;
    float lastUngroundedSlope = 0;
    public float landAnimTime = .5f;
    float lastLand = 0;

    public float stamina = 20.0f, baseStamina = 20.0f, maxStamina = 20.0f, minStamina = 1.0f;

    Vector2 lastMidairVelocity;

    //Here for access from the sword follow script
    public GameObject attackMoveTracker;

    [SerializeField] private GameObject sprintDust;

    public bool attacking = false;

    public bool swordAnimControl = false;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private SoundClip landSound;
    public Ground.Type currentGround;

    private float realVelocity;
    private Vector3 lastPosition;
    public bool overrideColliderWidth = false;
    public Vector2 colliderWidth = new Vector2();

    public Vector2 groundCheckSpot = new Vector2();

    public float lastSlope;

    public bool stopStaminaRefill = false;

    public bool allowJumpInterrupts = true;

    public float dashTime = .25f;
    public float dashStartTime = -100;
    public float dashCooldown = .25f;


    //Is swimming if swimmings <= 0, not a bool just so that when going between two seams it doesn't cancel swimming
    public int Swimming = 0;


    public GameObject submergeTrackerPrefab;
    public PlayerSubmergeTracker submergeTracker;

    bool wading = false;

    //public static PlayerInput inputs;

    //empty functions to prevent error calls from input settings
    //public void OnBark() { }

    //public void OnJump() { }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inputManager = InputReader.inputs;


        //inputManager.ActivateInput();
        //inputManager.actions["Bark"].started += BarkTest;
        //inputManager.actions.FindActionMap("Gameplay").FindAction("Bark").performed += BarkTest;


        cldr = cldr1;

        if (!overrideColliderWidth)
            colliderSize = cldr.bounds.size;
        else
            colliderSize = colliderWidth;
        timeSinceJumpPressed = 0.2f;
        jumpSpeedMultiplier = 1.0f;
        sprintSpeedMultiplier = 1.0f;
        fallTime = 0.0f;
        jumpTime = 0.0f;
        canResprint = true;

        stepDirection = 1;
        facingRight = true;
        isGrounded = true;
        created = true;

        //Set bounds of collider for use by downward raycasts in slope detection
        upperLeftCorner = new Vector2((-cldr.bounds.extents.x * 1) + cldr.offset.x, cldr.bounds.extents.y + cldr.offset.y);
        upperRightCorner = new Vector2((cldr.bounds.extents.x * 1) + cldr.offset.x, upperLeftCorner.y);
        

        if(groundCheckSpot == default)
            groundCheckSpot = (Vector2)(groundCheck.transform.position - transform.position) + Vector2.up * groundCheck.cldr.offset.y;

        groundCheck.triggerEnter += checkIfLanding;

        Camera.main.transform.position = transform.position + new Vector3(0, 2, -10);
    }


    public void checkIfLanding(Collider2D collision)
    {
        if(Mathf.Pow(2, collision.gameObject.layer) == whatIsGround && !isGrounded && !ChangeScene.changingScene && Swimming <= 0 && Crossfade.over)
        {
            lastLand = Time.time;
            soundPlayer.PlaySound(landSound);
            rb.gravityScale = 4.5f;
        }
    }

    IEnumerator RemoveStop()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        stops--;
    }

    //Pit/hazard respawn (not death respawn)
    //Moves the player to the last known safe space when they fall into a pit
    public void GoToResetPoint()
    {
        if (GetComponent<PlayerHealth>().health > 0)
        {
            if (resetting || PlayerHealth.dead)
                return;
            
            // TODO: Add cutscene here. Start death animation, then fade to black, then unfade, then wake up player at reset point position.
            transform.position = resetPosition;
            transform.rotation = resetRotation;
        }
    }

    void Update()
    {
        if (PauseScreen.paused) return;

        if (submergeTracker == null)
        {
            submergeTracker = Instantiate(submergeTrackerPrefab).GetComponent<PlayerSubmergeTracker>();
            submergeTracker.player = this;
        }

        //Swimming is number of budies of water player is in (its a counter so multiple touching or close by bodies of water aren't an issue)
        if (Swimming < 1 && (submergeTracker.wade.waterDepth <= 0 /*&& isGrounded*/))
            GroundMovementUpdate();
        else if (Swimming > 0 && (submergeTracker.wade.waterDepth <= 0) && Mathf.Abs(rb.velocity.y) < 2)
            WadingMovement();
        else
            SwimmingUpdate();

        
    }

    public void GroundMovementUpdate()
    {
        //If last frame you were in water, translate the water positioning to land positioning
        if (waterRotation)
        {
            ChangeToLandRotation();
            waterRotation = false;
        }

        bottom = new Vector2(cldr.bounds.center.x, cldr.bounds.center.y - cldr.bounds.extents.y);

        if (timeSinceJumpPressed < 1f)
            timeSinceJumpPressed += Time.deltaTime;

        if (timeSinceJump < 1f)
            timeSinceJump += Time.deltaTime;

        float deltaStamina = 0.0f;
        if (isSprinting)
            deltaStamina -= Time.deltaTime;
        else if (timeSinceSprint > 0.1f && !isDashing)
            deltaStamina += 1.5f*Time.deltaTime;
        if (PlayerHealth.dead) deltaStamina = 0;
        if (!stopStaminaRefill)
            stamina = Mathf.Clamp(stamina + deltaStamina, 0, maxStamina);

        //Freeze X only so player can finish falling to ground
        //Since presumably player is already grounded, Y is mostly locked as they can't move into the ground and gravity will keep them floored
        rb.constraints = CutsceneController.cutsceneFreezePlayerRb ? RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeRotation;
        //rb.drag = CutsceneController.cutsceneFreezePlayerRb ? 9999999 : 0;
        //if (CutsceneController.cutsceneFreezePlayerRb)
        //    rb.sharedMaterial = immovable;

        //Check flags that stop movement
        if (!PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open && DialogController.main?.source?.waiting != true && !DialogController.main?.pausePlayerMovement  == true && !ChangeScene.changingScene)
        {
            // remember previous movement input
            prevMoveX = moveX;

            // grab movement input from horizontal axis
            //moveX = Input.GetAxisRaw("Horizontal");
            //Disable moving while attacking

            //Dash
            if(dashStartTime + dashTime > Time.time)
            {
                //During dash
                Vector2 dashDir = facingRight ? Vector2.right : Vector2.left;

                rb.velocity = new Vector2(dashDir.x * dashVel, rb.velocity.y);
                isDashing = true;
                return;
            }
            else
            {
                //On end of dash
                if (isDashing)
                    rb.velocity = new Vector2(10 * (facingRight ? 1 : -1), rb.velocity.y);
                isDashing = false;
                
            }

            //Read movement input
            if (!stopMovement)
                moveX = inputManager.actions["Move"].ReadValue<Vector2>().x;
            else
            {
                moveX = 0;
                if (inputManager.actions["Move"].ReadValue<Vector2>().x != 0)
                    anim.SetTrigger("TryingMove");
            }

            //moveX = inputManager.actions["Move"].
            
            //Stop player from walking into walls (so the walk animation doesn't play while they aren't moving)
            if (wallOnRight && moveX > 0) moveX = 0;
            if (wallOnLeft && moveX < 0) moveX = 0;

            if (moveX == 0 && timeIdle < 1f)
            {
                timeIdle += Time.deltaTime;
            }
            else if (moveX != 0)
            {
                timeIdle = 0;
            }

            if (!isGrounded)
                anim.ResetTrigger("turn");

            anim.SetBool("moveX", moveX != 0 && Mathf.Abs(realVelocity) > 0.001f);
            anim.SetFloat("time_idle", timeIdle);

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
            if (isGrounded && moveX != 0 && !trotting && Mathf.Abs(realVelocity) >= 0.01f && !isJumping)
            {
                anim.SetTrigger("trot");
                trotting = true;
            }
            if (timeIdle >= 0.4f)
                anim.ResetTrigger("trot");

            // bark code
            //if (Input.GetKeyDown(KeyCode.B))

            if (inputManager.actions["Bark"].WasPressedThisFrame())
            {
                anim.SetTrigger("bark");
            }

            // jump code
            Jump();

            // release jump
            //if (Input.GetButtonUp("Jump"))

            if (inputManager.actions["Jump"].WasReleasedThisFrame())
            {
                if (holdingJump && isJumping && rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
                }

                holdingJump = false;
                
            }

            // sprinting
            if (trotting && !isSprinting && !isSkidding && canSprint)
            {
                if ((inputManager.actions["Sprint"].IsPressed() && canResprint && stamina >= minStamina) || inputManager.actions["Sprint"].WasPressedThisFrame())
                {
                    isSprinting = true;
                    if (!isJumping)
                        anim.SetTrigger("start_sprint");
                }
            }

            //Sprint cancel/interrupt conditions
            if (isSprinting && (inputManager.actions["Sprint"].WasReleasedThisFrame() || (moveX == 0 && Mathf.Abs(rb.velocity.x) <= 0.01f) || stamina <= 0 || !trotting || (moveX != 0 && Mathf.Abs(realVelocity) < 0.01f) || !canSprint))
            {
                isSprinting = false;
                anim.ResetTrigger("start_sprint");
            }

            if (stamina <= minStamina)
            {
                canResprint = false;
            }
            if (!canResprint && (inputManager.actions["Sprint"].WasReleasedThisFrame() || (timeIdle > 0.1f && stamina > minStamina)))
            {
                canResprint = true;
            }

            anim.SetBool("sprinting", isSprinting || timeSinceSprint < 0.1f);

            //Sprint windup?
            if (isSprinting && !isSkidding)
            {
                timeSinceSprint = 0;
                sprintSpeedMultiplier = Mathf.Lerp(sprintSpeedMultiplier, maxSprintSpeedMultiplier, 0.005f);
            }
            else
            {
                if (timeSinceSprint < 1f)
                    timeSinceSprint += Time.deltaTime;

                sprintWindUpPercent = 1;

                if (!isSkidding)
                    sprintSpeedMultiplier = Mathf.Lerp(sprintSpeedMultiplier, 1.0f, 0.5f);
            }

            if(!isDashing && inputManager.actions["Dash"].WasPressedThisFrame() && dashTime + dashStartTime + dashCooldown < Time.time)
            {
                Dash();
            }
        }
        else if(ChangeScene.maintainMovement)
        {
            FakeInput(moveX);
        }
        else if (!externalControl)
        {
            moveX = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
            // Disable frozen sprinting  
            if (isSprinting)
            {
                isSprinting = false;
                anim.ResetTrigger("start_sprint");
                anim.SetBool("sprinting", false);
            }
            anim.SetBool("moveX", false);
            anim.SetBool("attacking", false);
            timeSinceSprint = 1.0f;
            isJumping = false;
            sprintSpeedMultiplier = 1.0f;
            jumpSpeedMultiplier = 1.0f;
            if (PlayerHealth.dead)
            {
                stamina = Mathf.Lerp(stamina, 0, 0.02f);
                if (stamina <= 0.1f)
                    stamina = 0;
            }
        }
        canDash = stamina > baseStamina / 3;
        anim.SetBool("can_dash", canDash);
    }

    public void StopSprint()
    {
        isSprinting = false;
        anim.ResetTrigger("start_sprint");
        canResprint = true;
        anim.SetBool("sprinting", false);
    }

    public void WadingMovement()
    {
        //Debug.Log("Wading");

        if(waterRotation)
        {
            ChangeToLandRotation();
            waterRotation = false;
        }


        float waveTanAngle = submergeTracker.inWater.getTanAngleAtPoint(submergeTracker.inWater.getXLocalFromWorldSpace(transform.position.x));

        //Match angle of wave player is wading on
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, waveTanAngle * Mathf.Rad2Deg)) < 15)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, waveTanAngle * Mathf.Rad2Deg), Time.deltaTime * 15);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, waveTanAngle * Mathf.Rad2Deg), 
                Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, waveTanAngle * Mathf.Rad2Deg) * Time.deltaTime * 5));

        //Will have to rotate to level out. Can't be instant as you wade for a couple of frames when entering and exiting water

        float WaterLevel = submergeTracker.inWater.size.y/2 + submergeTracker.inWater.transform.position.y;

        
        //Divide by constant to reduce range it pushes
        WaterLevel += (submergeTracker.inWater.getHeightAtPoint(submergeTracker.inWater.getXLocalFromWorldSpace(transform.position.x), true));

        //Offset to keep more of the player in or out of the water when wading
        //WaterLevel -= 0.5f;

        Debug.DrawLine(transform.position, new Vector3(transform.position.x, WaterLevel));

        //transform.position = new Vector3(transform.position.x, WaterLevel);

        //return;
        //Float to line up with water level
        
        ///Anim stuff
        ///---------
        Vector2 inputMovement = inputManager.actions["move"].ReadValue<Vector2>();
        if (inputMovement.y > 0)
            inputMovement.y = 0;

        if (!stopMovement)
            moveX = inputMovement.x;
        else
        {
            moveX = 0;
            if (inputMovement.x != 0)
                anim.SetTrigger("TryingMove");
        }

        if (moveX == 0 && timeIdle < 1f)
        {
            timeIdle += Time.deltaTime;
        }
        else if (moveX != 0)
        {
            timeIdle = 0;
        }

        anim.SetBool("moveX", moveX != 0 && Mathf.Abs(realVelocity) > 0.001f);
        anim.SetFloat("time_idle", timeIdle);
        ///-------------
        ///End anim stuff


        //Player left/right wading movement
        if (inputMovement.x > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (inputMovement.x < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        float inputAngle = Mathf.Atan2(inputMovement.y, inputMovement.x) * Mathf.Rad2Deg;

        float swimSpeed = 10f * inputMovement.magnitude * (.5f + (Mathf.Clamp01(Mathf.Cos(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, inputAngle)))));

        //Slow player down more if they aren't pressing any direction
        if (inputManager.actions["move"].IsPressed())
        {
            //turnTowards(new Vector2(inputMovement.x, inputMovement.y));
            //Need to use mirroring sprite instead of turnTowards

            rb.drag = 1;
        }
        else
            rb.drag = 2f;

        //apply movement (conditional to cap speed)
        if (((rb.velocity + (inputMovement * swimSpeed)) * Time.deltaTime).magnitude < speed * 25)
        {
            //rb.velocity += (Vector2)(transform.rotation * Vector2.right * swimSpeed) * Time.deltaTime;
            rb.velocity += (inputMovement * swimSpeed) * Time.deltaTime;
        }

        //rb.velocity += Vector2.up * (transform.position.y - WaterLevel) * Time.deltaTime;

        //Change final constant to make it snappier
        transform.position += Vector3.up * (WaterLevel - transform.position.y) * Time.deltaTime * 2;

        //Wading jump
        if(inputManager.actions["jump"].WasPressedThisFrame())
        {
            //Jump();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce * rb.mass)); // force added during a jump
        }
        //if (inputManager.actions["jump"].WasPressedThisFrame())
        //{
        //    rb.velocity += (Vector2)(transform.rotation * (Vector2.right * 5));
        //}

    }

    public void FakeInput(float inMoveX)
    {
        prevMoveX = moveX;
        
        // grab movement input from horizontal axis
        //moveX = Input.GetAxisRaw("Horizontal");
        //Disable moving while attacking

        //Dash
        if (dashStartTime + dashTime > Time.time)
        {
            //During dash
            Vector2 dashDir = facingRight ? Vector2.right : Vector2.left;

            rb.velocity = new Vector2(dashDir.x * dashVel, rb.velocity.y);
            isDashing = true;
            return;
        }
        else
        {
            //On end of dash
            if (isDashing)
                rb.velocity = new Vector2(10 * (facingRight ? 1 : -1), rb.velocity.y);
            isDashing = false;

        }

        //if (!stopMovement)
            moveX = inMoveX;
        //else
        //{
        //    moveX = 0;
        //    if (inputManager.actions["Move"].ReadValue<Vector2>().x != 0)
        //        anim.SetTrigger("TryingMove");
        //}

        //moveX = inputManager.actions["Move"].

        if (wallOnRight && moveX > 0) moveX = 0;
        if (wallOnLeft && moveX < 0) moveX = 0;

        if (moveX == 0 && timeIdle < 1f)
        {
            timeIdle += Time.deltaTime;
        }
        else if (moveX != 0)
        {
            timeIdle = 0;
        }

        if (!isGrounded)
            anim.ResetTrigger("turn");

        anim.SetBool("moveX", moveX != 0 && Mathf.Abs(realVelocity) > 0.001f);
        anim.SetFloat("time_idle", timeIdle);

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
        if (isGrounded && moveX != 0 && !trotting && Mathf.Abs(realVelocity) >= 0.01f && !isJumping)
        {
            anim.SetTrigger("trot");
            trotting = true;
        }
        if (timeIdle >= 0.4f)
            anim.ResetTrigger("trot");

        //Sprinting stuff
        // sprinting

        anim.SetBool("sprinting", isSprinting);

        if (isSprinting && !isSkidding)
        {
            timeSinceSprint = 0;
            sprintSpeedMultiplier = Mathf.Lerp(sprintSpeedMultiplier, maxSprintSpeedMultiplier, 0.005f);
        }
        else
        {
            if (timeSinceSprint < 1f)
                timeSinceSprint += Time.deltaTime;

            sprintWindUpPercent = 1;

            if (!isSkidding)
                sprintSpeedMultiplier = Mathf.Lerp(sprintSpeedMultiplier, 1.0f, 0.5f);
        }
    }

    public void SwimmingUpdate()
    {

        if(!waterRotation)
        {
            ChangeToWaterRotation();
            waterRotation = true;
        }
        //waterDepth <= 0 means to wade
        Vector2 inputMovement = inputManager.actions["move"].ReadValue<Vector2>();
        int flipped = facingRight ? 1 : 1;
        float inputAngle = Mathf.Atan2(inputMovement.y, inputMovement.x) * Mathf.Rad2Deg;
        Debug.DrawLine(transform.position + new Vector3(Mathf.Cos(inputAngle * Mathf.Deg2Rad), Mathf.Sin(inputAngle * Mathf.Deg2Rad)), transform.position);


        float swimSpeed = 15f * inputMovement.magnitude * (.5f + (Mathf.Clamp01(Mathf.Cos(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, inputAngle)))));

        if (inputManager.actions["move"].IsPressed())
        {
            turnTowards(new Vector2(inputMovement.x * flipped, inputMovement.y));
            rb.drag = 1;
        }
        else
            rb.drag = 1.5f;

        //Apply movement in forward direction
        if(((rb.velocity + (Vector2)(transform.rotation * Vector2.right * swimSpeed)) * Time.deltaTime).magnitude < speed * 25)
        {
            rb.velocity += (Vector2)(transform.rotation * Vector2.right * swimSpeed) * Time.deltaTime;
        }

        if (inputManager.actions["jump"].WasPressedThisFrame())
        {
            rb.velocity += (Vector2)(transform.rotation * (Vector2.right * 5));
        }
    }

    //Used by swimming only
    public void turnTowards(Vector2 inputDir)
    {
        float angle;
        inputDir.Normalize();
        angle = (Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg);

        //Skip if its already at that angle
        angle = Mathf.DeltaAngle(transform.eulerAngles.z, angle);
        if (Mathf.Abs(angle) <= 0.01)
        {
            return;
        }
        //Calculate the distance to move left and right to find the optimal rotation direction
        float rotationSpeed = 60f;
        float maxRotationSpeed = 180f;

        angle = angle * Time.deltaTime * rotationSpeed;
        if (angle > maxRotationSpeed * Time.deltaTime)
        {
            angle = maxRotationSpeed * Time.deltaTime;
        }
        if (angle < -1 * maxRotationSpeed * Time.deltaTime)
        {
            angle = -1 * maxRotationSpeed * Time.deltaTime;
        }
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angle);
        //updateDirection();
        Debug.DrawLine(transform.rotation * Vector2.right + transform.position, transform.position);
        
        if ((transform.rotation * Vector2.right).x < 0)
            transform.localScale = new Vector3(transform.localScale.x, -1, 1);
        else
            transform.localScale = new Vector3(transform.localScale.x, 1, 1);

        slopeSideAngle = transform.eulerAngles.z;
        lastGroundedSlope = slopeSideAngle;
        lastUngroundedSlope = slopeSideAngle;
        facingRight = false;
    }

    void FixedUpdate()
    {
        if(Swimming < 1)
            GroundMovementFixedUpdate();

    }


    public void GroundMovementFixedUpdate()
    {
        // check if the player is against a wall
        CheckWall();

        //Hand calculated velocity
        realVelocity = (transform.position.x - lastPosition.x) / Time.fixedDeltaTime;
        lastPosition = transform.position;

        if (isSkidding)
            sprintSpeedMultiplier = Mathf.Lerp(sprintSpeedMultiplier, 1f, 0.05f);

        // calculate speed
        calculatedSpeed = speed * Mathf.Min(jumpSpeedMultiplier * sprintSpeedMultiplier, 2.0f) * sprintWindUpPercent * Time.fixedDeltaTime;

        // flip sprite depending on direction of input
        
        if ((moveX < 0 && facingRight) || (moveX > 0 && !facingRight))
        {
            if (!isTurning) reversedTurn = false;
            if (finishedReverseTurnThisFrame) finishedReverseTurnThisFrame = false;

            if (!isDashing && !isTurning && !waitingToTurn && !finishedReverseTurnThisFrame)
            {
                intendedFacingRight = !facingRight;
                if (!pAttack.isParrying && !pAttack.isAttacking && isGrounded)
                {
                    anim.SetTrigger("turn");
                    reversedTurn = false;
                    anim.SetFloat("turn_speed", 1f);
                }
                else
                {
                    Flip();
                }
            }
        }

        if (isTurning && (moveX < 0 && intendedFacingRight) || (moveX > 0 && !intendedFacingRight))
        {
            intendedFacingRight = !intendedFacingRight;
            reversedTurn = !reversedTurn;
            anim.SetFloat("turn_speed", reversedTurn ? -1 : 1f);
        }

        // calculate target velocity
        Vector3 targetVelocity = new Vector2(PlayerHealth.dead ? 0 : moveX * calculatedSpeed, rb.velocity.y);

        // sloped movement
        if (isOnSlope && isGrounded && !isJumping && canWalkOnSlope)
        {
            targetVelocity.Set(PlayerHealth.dead ? 0 : moveX * calculatedSpeed * -slopeNormalPerp.x, moveX * calculatedSpeed * -slopeNormalPerp.y, 0.0f);
        }

        // apply velocity, dampening between current and target
        if (moveX == 0.0 && rb.velocity.x != 0.0f)
        {
            if (canWalkOnSlope)
                cldr.sharedMaterial = friction;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing * 2.5f);
        }
        else
        {
            cldr.sharedMaterial = slippery;
            Vector3 newVel = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

            if(newVel.magnitude > rb.velocity.magnitude || isGrounded)
                rb.velocity = newVel;
        }

        // if (!isGrounded)
        // {
        //     cldr.sharedMaterial = slippery;
        // }

        // calculate speed multiplier for trot animation
        CalculateSpeedMultiplier();

        // handle scene loads
        if (beenLoaded < minLoadTime)
            beenLoaded += Time.fixedDeltaTime;
        else
        {
            anim.SetBool("loaded", true);
            // check if player is on ground
            CheckGround();
        }

        //// rotate player based on slope
        //transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);

        // hold jump distance extentions
        if (isJumping)
        {
            jumpTime += Time.fixedDeltaTime;
            jumpSpeedMultiplier = 1f + 2f/(10f * jumpTime + 4f);
            if (holdingJump)
            {
                jumpSpeedMultiplier *= 1.25f;
                rb.AddForce(new Vector2(0f, rb.mass * jumpForce / 400f / jumpTime));
            }
        }
        else 
        {
            jumpSpeedMultiplier = Mathf.Lerp(jumpSpeedMultiplier, 1, 0.3f);
        }

        // fall detection
        if (beenOnLand >= 0.1f && !isJumping && !isGrounded && !isFalling)
        {
            anim.SetTrigger("fall");
            isFalling = true;
        }
        if (isFalling)
        {
            fallTime += Time.fixedDeltaTime;
            if ((isGrounded && fallTime > 0.1f))
            {
                anim.ResetTrigger("fall");
                isFalling = false;
                fallTime = 0.0f;
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
            if (!(rb.velocity.y > 0f) && isJumping && timeSinceJump > 0.2f)
            {
                jumpSpeedMultiplier = 1f;
                isJumping = false;
                jumpTime = 0f;
            }
        }

        // Fixes turn deadlock
        if (anim.GetFloat("turn_speed") < 0 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
        {
            EndTurn();
        }
    }


    public void EnterWater()
    {
        Swimming++;

        if(Swimming > 0)
        {
            rb.gravityScale = 0;
            rb.drag = 1.5f;

            //ChangeToWaterRotation();
            //Change to an enter water anim
            anim.Play("IdleTread");
        }
    }

    public void LeaveWater()
    {

        Swimming--;

        if(Swimming < 1)
        {
            rb.gravityScale = 4.5f;
            rb.drag = 0;
            rb.velocity *= 1.5f;

            //ChangeToLandRotation();
            anim.Play("Fall");
        }
    }

    //Translates land into water positioning (mostly rotation changes)
    public void ChangeToWaterRotation()
    {
        if (!facingRight)
        {
            transform.localScale = new Vector3(1, -1, 1);

            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180);
            slopeSideAngle += 180;

            lastGroundedSlope += 180;
            lastUngroundedSlope += 180;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            //transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180);
        }


        //turnTowards(Quaternion.Euler(0, 0, slopeSideAngle) * Vector2.right);
    }

    //Translates water into land position (mostly rotation changes)
    public void ChangeToLandRotation()
    {
        if (transform.localScale.y < 0)
        {
            facingRight = true;
            slopeSideAngle += 180;

            lastGroundedSlope += 180;
            lastUngroundedSlope += 180;
        }
        else
        {
            facingRight = false;
        }

        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);


        int negative = facingRight ? 1 : -1;
        float rotationAmount = (rb.velocity.y * Time.deltaTime * 75 * negative); //the constant number needs to match that of ROTATION_INTENSITY in midair_rotation
        rotationAmount = Mathf.Clamp(rotationAmount, -75, 75);
        lastGroundedSlope = slopeSideAngle - rotationAmount;

        //lastUngroundedSlope = slopeSideAngle;

        //rb.velocity = Vector3.Scale(rb.velocity, new Vector3(3, 1.5f));

        transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);
    }

    // flips sprite when player changes movement direction
    void Flip()
    {
        if (!canTurn)
            return;
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
    }

    // stops trotting on specific frames if player has released input
    public void StopTrot(int frame)
    {
        if (((moveX == 0 || (moveX != 0 && Mathf.Abs(realVelocity) < 0.01f)) && stops <= 2) // if either not giving input or giving input against a barrier *and* hasn't stopped moving more than twice in the last second
            || (stops > 2 && Mathf.Abs(realVelocity) < 0.01f)) // or has stopped moving more than twice in the last second and moving sufficiently slowly
        {
            switch (frame)
            {
                // stop here
                case 0 or 5 or 6 or 11:
                    if (!isJumping)
                        anim.SetTrigger("exit_trot");
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

    //Update slope angle
    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        GetRotation();
        //SlopeCheckHorizontal(checkPos, upperLeftCorner, upperRightCorner);
        SlopeCheckVertical(checkPos);
    }

    //Wrapper for slope check recursive call
    private float? SlopeCheckHorizontal(Vector2 upperLeftCorner, Vector2 upperRightCorner, int runs = 0)
    {
        float? returnVal = OldSlopeCheck(upperLeftCorner, upperRightCorner, 5);

        //if (returnVal == null || !returnVal.HasValue)
        //{
        //    float? normalSlope = GetAvgNormal(upperLeftCorner, upperRightCorner);
        //    if (normalSlope != null && Mathf.DeltaAngle(normalSlope.Value, slopeSideAngle) <= 5)
        //        return normalSlope;
        //}

        //if(returnVal != null && Mathf.DeltaAngle(90, returnVal.Value) <= 25)
        //{
            
        //}

        return returnVal;


        //return GetAvgNormal(upperLeftCorner, upperRightCorner);

    }

    //Old method for getting slope, left in case it is reused
    public float? GetAvgNormal(Vector2 upperLeftCorner, Vector2 upperRightCorner)
    {
        int numSamples = 15;
        anim.SetBool("ground_close", false);

        List<float> angles = new List<float>();

        for (int i = 0; i < numSamples; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Mathf.Lerp(upperLeftCorner.x, upperRightCorner.x, i / (numSamples - 1f)), upperRightCorner.y) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);


            if (hit.distance < colliderSize.y + 1f && hit.distance >= colliderSize.y - 0.5f)
            {
                Debug.DrawLine(new Vector2(Mathf.Lerp(upperLeftCorner.x, upperRightCorner.x, i / (numSamples - 1f)), upperRightCorner.y) + (Vector2)transform.position, hit.point, Color.green);

                if (hit.distance < 2f + colliderSize.y / 2f)
                    anim.SetBool("ground_close", true);

                angles.Add(-Vector2.SignedAngle(hit.normal, Vector2.up));
            }
            else
                Debug.DrawLine(new Vector2(Mathf.Lerp(upperLeftCorner.x, upperRightCorner.x, i / (numSamples - 1f)), upperRightCorner.y) + (Vector2)transform.position, hit.point, Color.red);
        }

        float anglesSum = 0;

        for (int i = 0; i < angles.Count; i++)
        {
            anglesSum += angles[i];
        }

        angles.Sort();



        if (angles.Count > 0)
        {
            //return anglesSum / angles.Count;
            if (Mathf.Abs(angles[angles.Count / 2]) > 60 || Mathf.DeltaAngle(angles[angles.Count / 2], slopeSideAngle) > 90 * Time.deltaTime)
            {
                
                return null;
            }
            else
                return angles[angles.Count / 2];
        }
        else
            return null;
    }

    //Currently used slope check
    private float? OldSlopeCheck(Vector2 upperLeftCorner, Vector2 upperRightCorner, int runs = 0)
    {
        float? returnAngle = null;

        //Prevent infinite recursive runs (should never as long as geometry is fully closed)
        if (runs > 10)
            return null;

        //Get initial raycasts
        RaycastHit2D leftHit = Physics2D.Raycast((upperLeftCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast((upperRightCorner) + (Vector2)transform.position, Vector2.down, slopeCheckDistance + colliderSize.y, whatIsGround);



        //Draw vertical raycasts, set ground_close to true if hit is close enough to the collider and also != 0,0
        #region gizmo drawing and ground_close setting

        float minGroundDistance = 2f + colliderSize.y / 2;
        anim.SetBool("ground_close", false);

        if (leftHit.point != Vector2.zero)
        {
            Debug.DrawLine(upperLeftCorner + (Vector2)transform.position, leftHit.point, new Color(1 - runs * .05f, 0, 0, 1));
            if (leftHit.distance <= minGroundDistance)
                anim.SetBool("ground_close", true);
        }
        if (rightHit.point != Vector2.zero)
        {
            Debug.DrawLine(upperRightCorner + (Vector2)transform.position, rightHit.point, new Color(1 - runs * .05f, 0, 0, 1));
            if (rightHit.distance <= minGroundDistance)
                anim.SetBool("ground_close", true);
        }
        #endregion

        //If needing to rotate and there is not enough data from left or right side, test for better position for left or right and run again
        #region retake left and right if one or both are missing
        if ((leftHit.point == Vector2.zero || rightHit.point == Vector2.zero) && isGrounded)
        {
            Vector2 leftSide = upperLeftCorner;
            Vector2 rightSide = upperRightCorner;
            float yLevel = groundCheckSpot.y + transform.position.y;
            //yLevel = cldr.bounds.min.y;

            if (leftHit.point == Vector2.zero)
            {
                leftSide = acrossCastForNewSide(upperLeftCorner, upperRightCorner, leftSide, upperLeftCorner, yLevel, Vector2.right);
                if (leftSide.x == 0)
                    return null;
            }
            if (rightHit.point == Vector2.zero)
            {
                rightSide = acrossCastForNewSide(upperLeftCorner, upperRightCorner, rightSide, upperRightCorner, yLevel, Vector2.left);
                if (rightSide.x == 0)
                    return null;
            }

            return OldSlopeCheck(leftSide, rightSide, runs + 1);
        }
        #endregion


        //Get the across raycasts and unsmoothed slope
        #region acrossRaycasts
        //Get which raycast made it farther
        RaycastHit2D farHit = rightHit.distance > leftHit.distance ? rightHit : leftHit;
        RaycastHit2D nearHit = rightHit.distance < leftHit.distance ? rightHit : leftHit;

        //Get which direction the acrosscast needs to be (it should go from larger to smaller side)
        int right = leftHit.distance < rightHit.distance ? -1 : 1;

        //Setup positions of across point origins, then raycast across and drawline gizmo to show it
        Vector2 acrossCheckSpot = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2);
        Vector2 acrossCheck2 = new Vector2(farHit.point.x, nearHit.point.y + (farHit.point.y - nearHit.point.y) / 2.4f);
        RaycastHit2D across = Physics2D.Raycast(acrossCheckSpot,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        RaycastHit2D across2 = Physics2D.Raycast(acrossCheck2,
                                                new Vector2(right, 0), Mathf.Abs(upperRightCorner.x - upperLeftCorner.x), whatIsGround);
        Debug.DrawLine(across.point, acrossCheckSpot, Color.green);
        Debug.DrawLine(across2.point, acrossCheck2, Color.green);

        //angle between left point hit and right point hit
        float unsmoothedSlope = Mathf.Atan((rightHit.point.y - leftHit.point.y) / (rightHit.point.x - leftHit.point.x)) * Mathf.Rad2Deg;

        //Percentage of the distance made across
        float acrossPercent = across.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));
        float acrossPercent2 = across2.distance / (Mathf.Abs(upperRightCorner.x - upperLeftCorner.x));
        #endregion

        //Make sure it is not reading the underside of a slope the player is on. Retake with shorter bounds if it is
        #region check for if reading underside
        bool onLedge = false;
        //Makes sure that it is not reading the slope of the underside of a slope by not taking abs val. 
        if (acrossPercent2 - acrossPercent < .0001)
        {
            returnAngle = 0;
            if (acrossPercent > .0001f)
            {
                if (right == 1)
                    return OldSlopeCheck(new Vector2(upperLeftCorner.x + colliderSize.x * acrossPercent, upperLeftCorner.y), upperRightCorner, runs + 1);
                else
                    return OldSlopeCheck(upperLeftCorner, new Vector2(upperRightCorner.x - colliderSize.x * acrossPercent, upperRightCorner.y), runs + 1);
            }

            #region Unimplemented across sample find fail handler
            //New stuff that tries to make it re get slope sample points if the across points don't find somewhere. Currently can cause vibrations
            //Specifically meant to help with the low branch on forest 2
            //if(across.point == Vector2.zero)
            //{
            //    Vector2 leftSide = upperLeftCorner;
            //    Vector2 rightSide = upperRightCorner;
            //    float yLevel = groundCheckSpot.y + transform.position.y;
            //    if (right == 1)
            //    {
            //        rightSide = acrossCastForNewSide(upperLeftCorner, upperRightCorner, leftSide, upperLeftCorner, yLevel, Vector2.right);
            //        if (leftSide.x == 0)
            //            return null;
            //    }
            //    else
            //    {
            //        rightSide = acrossCastForNewSide(upperLeftCorner, upperRightCorner, rightSide, upperRightCorner, yLevel, Vector2.left);
            //        if (rightSide.x == 0)
            //            return null;
            //    }

            //    return OldSlopeCheck(leftSide, rightSide, runs + 1);
            //}
            #endregion
            onLedge = true;
        }
        #endregion

        //Apply smoothing
        if (!float.IsNaN(unsmoothedSlope) && !onLedge)
            returnAngle = unsmoothedSlope * Mathf.Lerp(1, 0, (Mathf.Abs((acrossPercent / .5f) - 1)));

        return returnAngle;
    }

    /// <summary>
    /// Does an across raycast just below the player to find any colliders to use to determine slope.
    /// </summary>
    /// <param name="upperLeftOrigin">The upperLeft scan bounds point</param>
    /// <param name="upperRightOrigin">The upperright scan bounds point</param>
    /// <param name="side">which side it is starting on</param>
    /// <param name="usedOrigin">which origin to share x-value with</param>
    /// <param name="yLevel">They y level to scan across at</param>
    /// <param name="direction">The direction (left or right) to scan in </param>
    /// <returns></returns>
    public Vector2 acrossCastForNewSide(Vector2 upperLeftOrigin, Vector2 upperRightOrigin, Vector2 side, Vector2 usedOrigin, float yLevel, Vector2 direction)
    {
        RaycastHit2D groundFinder = Physics2D.Raycast(new Vector2(usedOrigin.x + transform.position.x, yLevel), direction, (upperRightOrigin.x - upperLeftOrigin.x), whatIsGround);

        Debug.DrawLine(new Vector2(usedOrigin.x + transform.position.x, yLevel), groundFinder.point, Color.magenta);
        side.x = groundFinder.point.x - transform.position.x;
        
        ////Prevent jumpyness on bumpy and tall slopes by ignoring really tall slopes
        if (groundFinder.distance > (upperRightCorner.x - upperLeftCorner.x) * .8f)
            return Vector2.zero;
        return side;
    }

    //Get slope of surface standing on
    public float FindSurfaceRotation()
    {
        float? angle = SlopeCheckHorizontal(upperLeftCorner, upperRightCorner);
        if (angle != null)
            return angle.Value;
        else
            return slopeSideAngle;
    }

    /// <summary>
    /// Does final routing on rotation logic.
    /// </summary>
    public void GetRotation()
    {
        slopeSideAngle = FindSurfaceRotation();
        if(isGrounded)
        {
            LandInterpolation();
            if (lastLand + landAnimTime <= Time.time)
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
        else
        {
            MidAirRotation();
        }
        transform.rotation = Quaternion.Euler(0, 0, slopeSideAngle);
        lastSlope = slopeSideAngle;
    }

    /// <summary>
    /// When landing, interpolate between new surface slope and last midair slope
    /// </summary>
    public void LandInterpolation()
    {
        lastGroundedSlope = slopeSideAngle;

        if (lastLand + landAnimTime > Time.time)
        {
            slopeSideAngle = Mathf.LerpAngle(lastUngroundedSlope, slopeSideAngle, Mathf.Clamp((Time.time - lastLand) * Mathf.Abs(lastMidairVelocity.y) / (landAnimTime), 0, 1));
            //slopeSideAngle = 0;
        }
    }

    /// <summary>
    /// Adjusts midair rotation to show velocity
    /// </summary>
    public void MidAirRotation()
    {
        const float ROTATION_INTENSITY = 75;
        int negative = 1;
        if (!facingRight)
            negative = -1;
        float rotationAmount = (rb.velocity.y * Time.deltaTime * ROTATION_INTENSITY * negative);
        rotationAmount = Mathf.Clamp(rotationAmount, -75, 75);
        slopeSideAngle = lastGroundedSlope + rotationAmount;

        lastMidairVelocity = rb.velocity;
        lastUngroundedSlope = slopeSideAngle;
    }
    /// <summary>
    /// Gets if the ground is close and if the angle being walked on is too high
    /// </summary>
    /// <param name="checkPos"></param>
    private void SlopeCheckVertical(Vector2 checkPos)
    {
        // anim.SetBool("ground_close", false);
        //Down raycast to get slope player is on (at the center)
        //Differs from horizontalSlopeCheck version as horizontal takes two raycasts, one at left and one at right side. This is just one at center
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        // Debug.DrawLine(checkPos, hit.point, Color.cyan, 0);
        if (hit || isGrounded)
        {
            if(isGrounded || hit.distance <= 2f)
                anim.SetBool("ground_close", true);

            if (isGrounded && !hit)
                return;

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

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

    //Check for a wall on either side of the player
    void CheckWall()
    {
        wallOnLeft = wallOnRight = false;
        Vector2 startPosition1 = (Vector2)transform.position - new Vector2(0, cldr.bounds.size.y * 0.6f);
        startPosition1 += facingRight ? upperRightCorner : upperLeftCorner;

        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        if (isOnSlope)
        {
            direction = Quaternion.Euler(0, 0, slopeSideAngle) * direction;
        }
        
        //RaycastHit2D wallInfo = Physics2D.Raycast(startPosition1, direction, cldr.bounds.size.x + wallCheckDistance, whatIsGround);

        //if (wallInfo.point != Vector2.zero)
        //{
        //    Debug.DrawLine(startPosition1, wallInfo.point, Color.blue);
        //    if (wallInfo.distance <= wallCheckDistance) {
        //        if (facingRight) wallOnRight = true;
        //        else wallOnLeft = true;
        //    }
        //}
    }

    //Check if the player is on the ground, as well as a ceiling
    void CheckGround()
    {
        SlopeCheck();

        lastOnLand = Mathf.Clamp(lastOnLand + Time.fixedDeltaTime, 0, 20f);

        bool wasGrounded = isGrounded;
        isGrounded = false;

        bool wasRoofed = isRoofed;
        isRoofed = false;

        bool needsClean = false;

        //Scan colliders groundedCheck is touching for any ground
        foreach(Collider2D collision in groundCheck.triggersInContact)
        {
            //Debug.Log(LayerMask.GetMask("Terrain"));
            if (collision == null)
            {
                needsClean = true;
                continue;
            }
            if(Mathf.Pow(2, collision.gameObject.layer) == whatIsGround)
            {
                anim.SetBool("grounded", true);
                currentGround = collision.gameObject.TryGetComponent(out Ground ground) ? ground.type : Ground.Type.GRASS;

                // conditional collider activation
                bool branch = collision.gameObject.CompareTag("Branch");
                cldr = branch ? cldr2 : cldr1;
                cldr2.enabled = branch;
                cldr1.enabled = !branch;
                
                isGrounded = true;
                lastOnLand = 0f;
                lastLandHeight = transform.position.y;
                break;
            }
        }

        //If any colliders were deleted in groundCheck, tell it to remove any null values
        if (needsClean)
            groundCheck.clean();

        needsClean = false;
        behindGrounded = false;
        foreach (Collider2D collision in behindGroundCheck.triggersInContact)
        {
            if (collision == null)
            {
                needsClean = true;
                continue;
            }
            if (Mathf.Pow(2, collision.gameObject.layer) == whatIsGround)
            {
                behindGrounded = true;
            }
        }
        if (needsClean)
            behindGroundCheck.clean();

        // TODO this may break with the sword - need to check later
        // Respawn for falling into pits (not death respawn)
        bool isHazard = rb.IsTouchingLayers(LayerMask.NameToLayer("WorldHazard"));
        if (!isHazard) {
            if (isGrounded)
            {
                resetPosition = transform.position;
                resetRotation = transform.rotation;
            }

            if (behindGrounded)
            {
                resetPosition = behindGroundCheck.transform.position;
                resetRotation = transform.rotation;
            }
        }

        if (wallOnLeft || wallOnRight)
        {
            cldr = cldr2;
            cldr2.enabled = true;
            cldr1.enabled = true;
        }

        //Check for roofs (used to prevent spam jumping just to hit your head)
        needsClean = false;
        foreach (Collider2D collision in roofCheck.triggersInContact)
        {
            //Debug.Log(LayerMask.GetMask("Terrain"));
            if (collision == null)
            {
                needsClean = true;
                continue;
            }
            if (Mathf.Pow(2, collision.gameObject.layer) == whatIsGround && collision.gameObject.GetComponent<PlatformEffector2D>() == null)
            {
                isRoofed = true;
                break;
            }
        }
        if (needsClean)
            roofCheck.clean();
        
        if ((isJumping && jumpTime < 0.1f) || (isFalling && fallTime < 0.1f))
            anim.SetBool("grounded", false);
        else
            anim.SetBool("grounded", isGrounded);

        anim.SetBool("jump", isJumping);
        if ((!isGrounded || canResprint) && isSkidding) StopSkid();
    }


    void Jump()
    {
        if (isDashing) return;

        // if player presses jump button
        //Maybe could make the jumps that cancel the attack return anims be done once the return anim is cancelled? 
        //Currently it just stops the return animation without triggering a jump
        if (inputManager.actions["Jump"].WasPressedThisFrame())
        {
            if (!stopMovement || allowJumpInterrupts)
            {
                if (isGrounded && !(rb.velocity.y > 0f) && isJumping)
                {
                    jumpSpeedMultiplier = 1f;
                    isJumping = false;
                    jumpTime = 0f;
                }
                timeSinceJumpPressed = 0.0f;
            }
            else
                anim.SetTrigger("TryingMove");
        }

        if (inputManager.actions["Jump"].WasPressedThisFrame() && timeSinceJumpPressed < 0.2f)
        {
            if (!isJumping)
            {
                holdingJump = true;
            }
        }

        // incorporates coyote time and input buffering
        float coyoteTimeThreshold = 0.1f;
        bool coyoteTime = lastOnLand < 0.2f && transform.position.y < lastLandHeight - coyoteTimeThreshold;
        
        if (timeSinceJumpPressed < 0.2f && (isGrounded || coyoteTime) && !isRoofed && !isJumping)
        {
            if (isOnSlope && slopeDownAngle > maxSlopeAngle && cldr != cldr2)
                return;
            
            // Add a vertical force to the player
            isGrounded = false;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce * rb.mass)); // force added during a jump
            anim.SetTrigger("start_jump");
            timeSinceJump = 0.0f;
            isTurning = false;
            reversedTurn = false;
            waitingToTurn = false;
            finishedReverseTurnThisFrame = false;
        }
    }

    public void StartedJump()
    {
        anim.ResetTrigger("start_jump");
    }

    public void barkEffect()
    {
        GameObject addedObject = Instantiate(barkFXPrefab, mouth.position, transform.rotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity.y = -1 + Random.Range(-1f, 1f) * .25f;
        addedParticle.velocity.x = (facingRight ? 1 : -1) + Random.Range(-1f, 1f) * .25f;
        addedParticle.acceleration.y = 3 + Random.Range(-1f, 1f) * .25f;
        addedParticle.startTime = Time.time;
    }

    public void PlaySound(string path)
    {
        soundPlayer.PlaySound(path);
    }

    public void PlayStepSound()
    {
        if (isGrounded) {
            string path = "Impacts.Steps.SoftStep";
            switch (currentGround)
            {
                case Ground.Type.ROCK:
                    path = "Impacts.Steps.Stone";
                    break;
                case Ground.Type.WOOD:
                    path = "Impacts.Steps.Wood";
                    break;
                case Ground.Type.SAND:
                    path = "Impacts.Steps.RoughStep";
                    break;
                case Ground.Type.GRASS:
                    path = "Impacts.Steps.SoftStep";
                    break;
                case Ground.Type.GRAVEL:
                    path = "Impacts.Steps.Gravel";
                    break;
            }
            soundPlayer.PlaySound(path);
        }
    }

    public void StopSkid()
    {
        isSkidding = false;
        sprintSpeedMultiplier = 1.0f;
    }

    public void StartSkid()
    {
        sprintSpeedMultiplier = 0;
        isSprinting = false;
        isSkidding = true;
    }

    public void StartTurn()
    {
        if (!isDashing && !isTurning && !anim.GetBool("exit_turn") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0)
        {
            Flip();
            isTurning = true;
        }
    }

    public void EndTurn()
    {
        reversedTurn = false;
        isTurning = false;
        Flip();
        anim.SetBool("exit_turn", true);
        anim.SetFloat("turn_speed", 1);
        attackMoveTracker.transform.Rotate(0, 0, -180);
        SwordFollow.sword.transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        SwordFollow.sword.transform.Rotate(0, 0, -180);
        SwordFollow.sword.adjustLocationX *= -1;
        if (Player.instance.hasLantern)
        {
            var mouth = Player.instance.mouthLantern.transform;
            mouth.localPosition = new Vector3(mouth.localPosition.x * -1, 0, 0);
        }
        finishedReverseTurnThisFrame = true;
    }

    public void StopTurn()
    {
        anim.SetBool("exit_turn", true);
    }

    public void Dash()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        stamina = Mathf.Max(stamina - baseStamina / 3, 0);
        dashStartTime = Time.time;
        
    }

    // TODO not called atm but should be if dash becomes its own move
    public void StopDash() {
        isDashing = false;
    }
}