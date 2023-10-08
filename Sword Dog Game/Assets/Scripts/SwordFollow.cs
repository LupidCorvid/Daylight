using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordFollow : MonoBehaviour
{
    public GameObject player;
    Vector3 playerLocation;
    Vector3 swordTargetLocation;
    Vector3 swordPreviousLocation;
    public float speed;
    public float adjustLocationY, adjustDefaultY;
    public float adjustLocationX, adjustDefaultX;
    SpriteRenderer sr;
    public Rigidbody2D rb;
    public Collider2D cldr;

    public Animator swordParryAnimator;

    public PlayerMovement pmScript;
    public GameObject tip;

    public static SwordFollow sword;
    public static GameObject instance;

    public GameObject attackMoveTracker;
    public static Action sceneChange;
    public static Vector3 newPos;

    public ParticleSystemForceField particleFF;
    public float particlePushScalar = 2.5f;
    private static bool canMove = true;
    private static float cantMoveFor = 0.1f, maxDelay = 0.1f;
    public static bool created;
    private float xOffset = 0.0f;

    public IParryable lastParriedEntity;

    public SoundPlayer audioPlayer;

    public CanParryTracker canParryCheck;

    public float parryFailCooldown = 1.5f;
    public float lastParryFail = -100f;

    // Start is called before the first frame update
    void Start()
    {
        sceneChange += Snap;
        
        speed = 100;
        //adjust the adjustLocation variables per sword type
        adjustDefaultX = -.2f;
        adjustDefaultY = 0.6f;
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        cldr = gameObject.GetComponent<Collider2D>();
        audioPlayer ??= GetComponent<SoundPlayer>();

        //SceneHelper.FinishedChangeScene += Snap;

        // Singleton design pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sword = this;
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        if (!created) {
            adjustLocationX = adjustDefaultX;
            adjustLocationY = adjustDefaultY;
            Snap();
            created = true;
            sceneChange += Snap;
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canMove)
        {
            cantMoveFor += Time.fixedDeltaTime;
            if (cantMoveFor < maxDelay)
                return;
            canMove = true;
        }

        sword = this;
        instance = gameObject;

        if (SceneHelper.changedSceneThisFrame) // TODO this code doesn't run
            return;
        //Find attackMoveTracker if it is null
        if (pmScript == null)
        {
            pmScript = player.GetComponent<PlayerMovement>();
            attackMoveTracker = pmScript.attackMoveTracker;
            swordParryAnimator = pmScript.pAttack.parryTrackerLocation.GetComponentInChildren<Animator>();
            canParryCheck = swordParryAnimator.GetComponent<CanParryTracker>();
        }

        if (attackMoveTracker == null)
            attackMoveTracker = pmScript.attackMoveTracker;
        

        if (pmScript.attacking)
        {
            AttackMove();
            particleFF.directionX = -rb.velocity.magnitude * particlePushScalar;
            //Check for contact damage
            return;
        }
        else if (pmScript.pAttack.isParrying)
        {
            parryMove();
            particleFF.directionX = -rb.velocity.magnitude;
            return;
        }
        else
        {
            particleFF.directionX = 0;
            speed = Mathf.Lerp(speed, 100, 0.05f);
        }
        ////Accesses PlayerMovement script ONCE
        //if(!triggeredPMScript)
        //{
        //    pmScript = player.GetComponent<PlayerMovement>();
        //    triggeredPMScript = true;
        //}
        if (!(PlayerHealth.dead && !PlayerHealth.gettingUp) && player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if(player != null && !(PlayerHealth.dead && !PlayerHealth.gettingUp))
        {
            cldr.isTrigger = true;
            gameObject.layer = 11;
            rb.isKinematic = false;
            rb.gravityScale = 0;

            //Assigns target transform values
            pmScript = player.GetComponent<PlayerMovement>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerLocation = player.transform.position;

            var offset = player.transform.rotation * new Vector2(adjustLocationX, adjustLocationY);
            swordTargetLocation = playerLocation + offset;

            //Moves
            swordPreviousLocation = transform.position;
            
            if (!PlayerHealth.gettingUp)
            {
                rb.velocity = (swordTargetLocation - transform.position) * (speed + 4 * pmScript.calculatedSpeed) / 5;


                rb.angularVelocity = getAngleDirection(transform.rotation, player.transform.rotation) * 10;
                //rb.angularVelocity = getAngleDirection(transform.rotation, player.transform.rotation) * 1;
            }
            else
            {
                rb.velocity = (swordTargetLocation - transform.position) * (4 * pmScript.calculatedSpeed) / 5;
                rb.angularVelocity = getAngleDirection(transform.rotation, player.transform.rotation) * 10 * 4;
                //rb.angularVelocity = getAngleDirection(transform.rotation, player.transform.rotation) * 1;
            }

            //Checks when to flip and adjust sprite
            if (!pmScript.facingRight)
            {
                adjustLocationX = -adjustDefaultX;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                adjustLocationX = adjustDefaultX;
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (pmScript.isJumping)
            {
                xOffset = Mathf.Lerp(xOffset, pmScript.trotting ? 0.2f : 0.4f, 0.1f);
                adjustLocationY = 0.8f;
            }
            else
            {
                xOffset = Mathf.Lerp(xOffset, 0, 0.1f);
                adjustLocationY = 0.6f;
            }

            if (!pmScript.facingRight) adjustLocationX -= xOffset;
            else adjustLocationX += xOffset;

            if (pmScript.isSprinting)
            {
                adjustDefaultX = Mathf.Lerp(adjustDefaultX, 0.2f, 0.1f);
                if (pmScript.isJumping)
                {
                    adjustDefaultX = 0f;
                }
            }
            else
            {
                adjustDefaultX = Mathf.Lerp(adjustDefaultX, -0.2f, 0.4f);
            }
        }
        else
        {
            cldr.isTrigger = false;
            gameObject.layer = 10;
            rb.gravityScale = 5;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.AddTorque(transform.localScale.x * 5f);
        }
    }

    public void AttackMove()
    {
        //Make sure that the sword is facing the right direction
        if (!pmScript.facingRight)
        {
            adjustLocationX = -adjustDefaultX;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            adjustLocationX = adjustDefaultX;
            transform.localScale = new Vector3(1, 1, 1);
        }

        //Moves to match the attack tracker animator

        rb.velocity = (attackMoveTracker.transform.position - transform.position) * 60;

        ////Multiplied by 60 to match the time of the frame in the animation
        rb.angularVelocity = getAngleDirection(transform.rotation, attackMoveTracker.transform.rotation) * 60;

    }

    public void parryMove()
    {
        if (!pmScript.facingRight)
        {
            adjustLocationX = -adjustDefaultX;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            adjustLocationX = adjustDefaultX;
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (swordParryAnimator == null || canParryCheck == null)
        {
            canParryCheck = pmScript?.GetComponentInChildren<CanParryTracker>();
            swordParryAnimator = canParryCheck.GetComponent<Animator>();
        }

        if(swordParryAnimator != null)
            rb.velocity = (swordParryAnimator.transform.position - transform.position) * 60 * Time.fixedDeltaTime * 20;

        //Might need to be multiplied by some form of Time.deltaTime
        rb.angularVelocity = getAngleDirection(transform.rotation, swordParryAnimator.transform.rotation) * 60 * 20 * Time.fixedDeltaTime;
    }

    public float getAngleDirection(Quaternion rotation1, Quaternion rotation2)
    {
        int neg = (Vector3.Cross(rotation1 * Vector3.right, rotation2 * Vector3.right).z) < 0 ? -1 : 1;

        return (Quaternion.Angle(rotation2, rotation1)) * neg;
    }

    private void Update()
    {
        if(pmScript?.pAttack?.isParrying == true)
        {
            if(PlayerMovement.inputs.actions["Attack"].IsPressed() && lastParryFail + parryFailCooldown < Time.time)
            {
                swordParryAnimator.Play("ParryParry");
                if (canParryCheck.canParry)
                    lastParriedEntity?.Parried(this);
                else
                    lastParryFail = Time.time;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(pmScript?.pAttack?.isParrying  == true && collision.gameObject != pmScript.gameObject && canParryCheck.isBlocking)
        {
            ITeam team = collision.GetComponent<ITeam>();

            if (team?.GetIfEnemies(pmScript.entityBase) != true)
                return;

            //if (collision?.attachedRigidbody != null)
            //{
            //    collision.attachedRigidbody.velocity = (collision.transform.position - pmScript.transform.position).normalized;
            //    collision.attachedRigidbody.AddForce((collision.transform.position - pmScript.transform.position).normalized * 500);
            //}
            lastParriedEntity = collision?.GetComponent<IParryable>();

            if (lastParriedEntity?.canBeParried != true)
                return;
            lastParriedEntity?.Blocked(this);
            audioPlayer.PlaySound("Impacts.Sword.SwordSlash");
            //collision?.GetComponent<Entity>()?.Blocked();
            if (!swordParryAnimator.GetCurrentAnimatorStateInfo(0).IsName("ParryParry"))
            {
                swordParryAnimator.Play("ParryBlock");
                pmScript.stamina -= pmScript.pAttack.parryStaminaCost;
            }
            else
                lastParriedEntity?.Parried(this);
            //IF player parried in correct window
            //collision.GetComponent<Entity>().Parried();
        }
    }

    public void Freeze()
    {
        if (PlayerHealth.dead)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void Snap()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //player ??= GameObject.FindGameObjectWithTag("Player(Clone)");
        var offset = player.transform.rotation * new Vector2((player.GetComponent<PlayerMovement>().facingRight ? 1 : -1) * adjustDefaultX, adjustLocationY);
        swordTargetLocation = player.transform.position + offset;
        sword.transform.position = swordTargetLocation;
        sceneChange -= Snap;
    }

    public static void DisableMovement()
    {
        canMove = false;
        cantMoveFor = 0f;
    }
}
