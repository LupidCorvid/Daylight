using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollow : MonoBehaviour
{
    public GameObject player;
    Vector3 playerLocation;
    Vector3 swordTargetLocation;
    Vector3 swordPreviousLocation;
    public float speed;
    public float adjustLocationY;
    public float adjustLocationX, adjustDefaultX;
    SpriteRenderer sr;
    Rigidbody2D rb;

    private PlayerMovement pmScript;
    bool triggeredPMScript;
    public GameObject tip;

    public GameObject attackMoveTracker;

    // Start is called before the first frame update
    void Start()
    {
        //adjust the adjustLocation variables per sword type
        speed = 12;
        adjustLocationY = 1;
        adjustDefaultX = -.5f;
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        
        triggeredPMScript = false;
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        //Find attackMoveTracker if it is null
        pmScript ??= player.GetComponent<PlayerMovement>();
        attackMoveTracker = pmScript.attackMoveTracker;

        if (pmScript.attacking)
        {
            AttackMove();
            //Check for contact damage
            return;
        }
        ////Accesses PlayerMovement script ONCE
        //if(!triggeredPMScript)
        //{
        //    pmScript = player.GetComponent<PlayerMovement>();
        //    triggeredPMScript = true;
        //}
        
        if(player != null && !(PlayerHealth.dead && !PlayerHealth.gettingUp))
        {
            rb.isKinematic = false;
            rb.gravityScale = 0;
            //rb.constraints = RigidbodyConstraints2D.FreezeRotation;

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
                transform.position = Vector3.Lerp(transform.position, swordTargetLocation, 2 + 4 * pmScript.calculatedSpeed * Time.deltaTime); //start value, end val, value used to interpolate between a and b
                transform.rotation = player.transform.rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, swordTargetLocation, 4 * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, 4 * Time.deltaTime);
            }

            //Checks when to flip and adjust sprite
            
            if (pmScript.isJumping)
            {
                if (!pmScript.facingRight) adjustLocationX = -adjustDefaultX - .8f;
                else adjustLocationX = adjustDefaultX + .8f;
                
                adjustLocationY = 0.9f;
            }
            else
            {
                adjustLocationY = 0.7f;
            }

            if (pmScript.isSprinting)
            {
                adjustDefaultX = Mathf.Lerp(adjustDefaultX, 0.2f, 0.1f);
            }
            else
            {
                adjustDefaultX = Mathf.Lerp(adjustDefaultX, -0.2f, 0.4f);
            }
            
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
        }
        else
        {
            rb.gravityScale = 5;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.AddTorque(transform.localScale.x * 5f);
        }
    }

    public void AttackMove()
    {
        //Moves to match the attack tracker animator

        rb.freezeRotation = false;

        rb.velocity = (attackMoveTracker.transform.position - transform.position) * 60;
        int neg = -1;
        if (transform.rotation.eulerAngles.z < attackMoveTracker.transform.rotation.eulerAngles.z)
            neg = 1;

        //Multiplied by 60 to match the time of the frame in the animation
        rb.angularVelocity = (Quaternion.Angle(transform.rotation, attackMoveTracker.transform.rotation)) * 60 * neg;

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
}
