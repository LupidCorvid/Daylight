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
        //Accesses PlayerMovement script ONCE
        if(triggeredPMScript == false)
        {
            pmScript = player.GetComponent<PlayerMovement>();
            triggeredPMScript = true;
        }
        
        if(player != null)
        {
            //Assigns target transform values
            pmScript = player.GetComponent<PlayerMovement>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerLocation = player.transform.position;
            swordTargetLocation = new Vector3(player.transform.position.x + adjustLocationX, player.transform.position.y + adjustLocationY);

            //Moves
            swordPreviousLocation = transform.position;
            transform.position = Vector3.Lerp(transform.position, swordTargetLocation, speed * Time.deltaTime); //start value, end val, value used to interpolate between a and b

            //Checks when to flip and adjust sprite
            
            if (pmScript.isJumping)
            {
                if (pmScript.facingRight == false) adjustLocationX = -adjustDefaultX - .5f;
                else adjustLocationX = adjustDefaultX + .5f;
            }
            else if (pmScript.facingRight == false)
            {
                adjustLocationX = -adjustDefaultX;
                sr.flipX = true;
            }
            else
            {
                adjustLocationX = adjustDefaultX;
                sr.flipX = false;
            }
        }
    }
}
