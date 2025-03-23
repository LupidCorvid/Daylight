using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Interact with house OR step into exitArea collider
    Enable the inside the house collider
        .enabled on inside the house collider
    Enable inside wall and floor colliders
	    .enabled for stuff dragged into inspector
    Enable NPCs that are inside
	    .enabled for stuff dragged into inspector

Stay enabled until the user interact exits or exits inside the house collider
	while colliding (front has alpha 0 so it's still active)
	.enabled = false for all enabled objects, immediately

 */

public class EnterHouse : MonoBehaviour
{
    public GameObject houseFront, houseBack; //Sprite of the front/back of the house
                                             //houseFront has component for fadeItem
    public GameObject exitArea; //The exits/entrances of the house that don't require a prompt
    public GameObject frontDoor; //The exit/entrance that requires a prompt to enter
    public GameObject insideWall, insideFloor;

    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;

    public bool increaseAlphaOnHouse = false;
    public bool playerIsInsideHouse = false;

    //All colliders and NPCs inside the house are disabled at start
    void Start()
    {
        houseBack.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    void Update()
    {
        //Update the house back to default if the player leaves
        //TODO: find a way to only trigger if player LEFT house only
        if (!playerIsInsideHouse)
        {
            //Increase the alpha
            if (increaseAlphaOnHouse)
            {
                houseFront.GetComponent<fadeItem>().increaseAlpha();

                if (houseFront.GetComponent<SpriteRenderer>().color.a >= 1)
                    increaseAlphaOnHouse = false;
            }
        }
        
    }

    //If you entered through the exitArea, you're now inside the house perpetually
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(collision.IsTouching(exitArea.GetComponent<BoxCollider2D>()))
                houseBack.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    //collision is the thing that's colliding with it, most likely the player
    //isTouching(gameobject's box collider) is how you get what object the alien collision is touching
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            bool isTouchingExitArea = collision.IsTouching(exitArea.GetComponent<BoxCollider2D>());

            //If touching frontDoor
            if (collision.IsTouching(frontDoor.GetComponent<BoxCollider2D>()))
                print("Spawn the prompt");
            
            //If touching exitArea
            if (isTouchingExitArea)
                houseFront.GetComponent<fadeItem>().decreaseAlpha(); //Decrease alpha on the front of the house
            
            //If still inside but went downstairs while decreaseAlpha() was happening
            if(playerIsInsideHouse && collision.IsTouching(houseBack.GetComponent<BoxCollider2D>()) && !isTouchingExitArea)
                houseFront.GetComponent<fadeItem>().decreaseAlpha();
                
            //Check if the player is still inside the house
            if (collision.IsTouching(houseBack.GetComponent<BoxCollider2D>())){
                playerIsInsideHouse = true;
            }
            else
            {
                playerIsInsideHouse = false;
                increaseAlphaOnHouse = true;
                houseBack.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
    
}
