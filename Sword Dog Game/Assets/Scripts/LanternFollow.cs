using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanternFollow : MonoBehaviour
{
    public GameObject player;
    Vector3 playerLocation;
    Vector3 lantTargetLocation;
    Vector3 lantPreviousLocation;
    //public float speed;
    //public float adjustLocationY, adjustDefaultY;
    //public float adjustLocationX, adjustDefaultX;
    SpriteRenderer sr;
    public Rigidbody2D rb;
    
    public PlayerMovement pmScript;

    // Start is called before the first frame update
    void Start()
    {
        //speed = 100;
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player ??= GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            GameObject mouth = GameObject.Find("mouthLantern");
            pmScript = player.GetComponent<PlayerMovement>();
            lantPreviousLocation = gameObject.transform.position;

            //Check for being flipped
            if (pmScript.facingRight)
                lantTargetLocation = new Vector3(mouth.transform.position.x, mouth.transform.position.y - .75f, mouth.transform.position.z);
            else
                lantTargetLocation = new Vector3(mouth.transform.position.x, mouth.transform.position.y - .75f, mouth.transform.position.z);


            if (player != null)
            {
                gameObject.transform.position = Vector3.Lerp(lantPreviousLocation, lantTargetLocation, 1);

            }
        }
    }

    public void AttackMove()
    {
        

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
        //player = GameObject.FindGameObjectWithTag("Player");
        ////player ??= GameObject.FindGameObjectWithTag("Player(Clone)");
        //var offset = player.transform.rotation * new Vector2((player.GetComponent<PlayerMovement>().facingRight ? 1 : -1) * adjustDefaultX, adjustLocationY);
        //swordTargetLocation = player.transform.position + offset;
        //sword.transform.position = swordTargetLocation;
        //sceneChange -= Snap;
    }

    public static void DisableMovement()
    {
        //canMove = false;
        //cantMoveFor = 0f;
    }
}
