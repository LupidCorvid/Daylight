using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollow : MonoBehaviour
{
    public GameObject player;
    Vector2 playerLocation;
    Vector2 swordTargetLocation;
    Vector2 swordPreviousLocation;
    public float speed;
    public float adjustLocationY;
    public float adjustLocationX;
    SpriteRenderer sr;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //adjust the adustLocation variables per sword type
        speed = 2;
        adjustLocationY = 1;
        adjustLocationX = -.5f;
        sr = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        //Assigns target transform values
        if(player != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerLocation = player.transform.position;
            swordTargetLocation = new Vector2(player.transform.position.x + adjustLocationX, player.transform.position.y + adjustLocationY);
        }

        //Moves and checks to flip sprite
        swordPreviousLocation = transform.position;
        transform.position = Vector2.Lerp(transform.position, swordTargetLocation, speed * Time.deltaTime); //start value, end val, value used to interpolate between a and b

        if (swordPreviousLocation.x > transform.position.x)
        {
            adjustLocationX = .5f;
            sr.flipX = true;
            //TODO: add some sort of delay so it isnt flickering
        }
        else
        {
            adjustLocationX = -.5f;
            sr.flipX = false;
        }

    }
}
