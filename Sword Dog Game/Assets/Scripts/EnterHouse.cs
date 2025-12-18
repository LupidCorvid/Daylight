using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


public class EnterHouse : MonoBehaviour
{
    public GameObject houseFront, houseBack; //Sprite of the front/back of the house
                                             //houseFront has component for fadeItem
    public GameObject [] exitAreas; //The exits/entrances of the house that don't require a prompt
    public GameObject[] exitAreaChecks; //Colliders to check if you're entering from the right direction
    public GameObject frontDoor; //The exit/entrance that requires a prompt to enter
    public GameObject insideWall, insideFloor;
    public GameObject[] NPCs;

    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;

    public bool increaseAlphaOnHouse = false;
    public bool playerIsInsideHouse = false;

    public InteractRoomEvent interactPrompt;

    //All colliders and NPCs inside the house are disabled at start
    void Start()
    {
        //houseBack.GetComponent<BoxCollider2D>().enabled = false;
        insideWall.SetActive(false);
        insideFloor.SetActive(false);

        NPCsActive(false);

        interactPrompt.interactedWith += determineAction; //Subscribes/adds the function to the list of actions in the other script
    }
    
    void Update()
    {
        //Update the house back to default if the player leaves
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

    //When entering through the front door, see if you need to open or close the house
    void determineAction()
    {
        if (playerIsInsideHouse) closeHouse();
        else openHouse();
    }

    void openHouse()
    {
        //houseBack.GetComponent<BoxCollider2D>().enabled = true;
        playerIsInsideHouse = true;
        AkUnitySoundEngine.PostEvent("EnterHouse", AudioManager.WwiseGlobal);
        insideCollidersActive(true);
        NPCsActive(true);
    }

    void closeHouse()
    {
        playerIsInsideHouse = false;
        increaseAlphaOnHouse = true;
        AkUnitySoundEngine.PostEvent("ExitHouse", AudioManager.WwiseGlobal);
        //houseBack.GetComponent<BoxCollider2D>().enabled = false;
        insideCollidersActive(false);
        NPCsActive(false);
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
            //Detect when to open the house
            //touching exit area check AND touching exit area AND NOT touching inside of house
            for (int i = 0; i < exitAreaChecks.Length; i++)
            {
                if (collision.IsTouching(exitAreaChecks[i].GetComponent<BoxCollider2D>()) && collision.IsTouching(exitAreas[i].GetComponent<BoxCollider2D>()) && !collision.IsTouching(houseBack.GetComponent<BoxCollider2D>()))
                    openHouse();
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Detect when to close the house
            if (!collision.IsTouching(houseBack.GetComponent<BoxCollider2D>()) && playerIsInsideHouse)
                closeHouse();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && playerIsInsideHouse == true)
        {
            bool isTouchingExitArea = false;
            for (int i = 0; i < exitAreas.Length; i++)
                isTouchingExitArea = collision.IsTouching(exitAreas[i].GetComponent<BoxCollider2D>());

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
        }
    }
    
}
