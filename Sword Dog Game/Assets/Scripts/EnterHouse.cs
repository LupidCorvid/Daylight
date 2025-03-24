using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Optimization suggestions/Minor bugs:
 * Make exit areas an array to accomodate 2 or more exits
 * Sometimes entiering from the top flickers the sprite, find out how to fix that
*/

/*Program Details:
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

//TODO: Enable/disable NPCs, 


public class EnterHouse : MonoBehaviour
{
    public GameObject houseFront, houseBack; //Sprite of the front/back of the house
                                             //houseFront has component for fadeItem
    public GameObject exitArea, exitArea1; //The exits/entrances of the house that don't require a prompt
    public GameObject frontDoor; //The exit/entrance that requires a prompt to enter
    public GameObject insideWall, insideFloor;
    public GameObject[] NPCs;

    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;

    public bool increaseAlphaOnHouse = false;
    public bool playerIsInsideHouse = false;

    //All colliders and NPCs inside the house are disabled at start
    void Start()
    {
        houseBack.GetComponent<BoxCollider2D>().enabled = false;
        insideWall.SetActive(false);
        insideFloor.SetActive(false);

        NPCsActive(false);
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

    //Either activates or disables NPCs depending on parameter
    //TODO: Keep sprites active but disable talking access
    void NPCsActive(bool flag)
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i].SetActive(flag);
        }
    }

    //Either activates or disables internal colliders depending on parameter
    void insideCollidersActive(bool flag)
    {
        insideWall.SetActive(flag);
        insideFloor.SetActive(flag);
    }

    //If you entered through the exitArea, you're now inside the house perpetually
    //Enable inside of house
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.IsTouching(exitArea.GetComponent<BoxCollider2D>()) || collision.IsTouching(exitArea1.GetComponent<BoxCollider2D>()))
            {
                houseBack.GetComponent<BoxCollider2D>().enabled = true;
                insideCollidersActive(true);
                NPCsActive(true);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            bool isTouchingExitArea = collision.IsTouching(exitArea.GetComponent<BoxCollider2D>()) || collision.IsTouching(exitArea1.GetComponent<BoxCollider2D>());

            //If touching frontDoor
            //Spawn the prompt
            if (collision.IsTouching(frontDoor.GetComponent<BoxCollider2D>()))
                print("Spawn the prompt");

            //If touching exitArea
            //Decrease alpha on the front of the house
            if (isTouchingExitArea)
                houseFront.GetComponent<fadeItem>().decreaseAlpha(); 
            
            //If still inside but went downstairs while decreaseAlpha() was happening
            //Make sure that the alpha continues decreasing
            if(playerIsInsideHouse && collision.IsTouching(houseBack.GetComponent<BoxCollider2D>()) && !isTouchingExitArea)
                houseFront.GetComponent<fadeItem>().decreaseAlpha();
                
            //Check if the player is still inside the house
            //Update internal colliders and NPCs as needed
            if (collision.IsTouching(houseBack.GetComponent<BoxCollider2D>())){
                playerIsInsideHouse = true;
            }
            else
            {
                playerIsInsideHouse = false;
                increaseAlphaOnHouse = true;
                houseBack.GetComponent<BoxCollider2D>().enabled = false;
                insideCollidersActive(false);
                NPCsActive(false);
            }
        }
    }
    
}
