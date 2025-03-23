using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeItem : MonoBehaviour
{
    Color logAlpha;
    bool exitedLog; //If the player has exited the fadable item's hitbox

    // Start is called before the first frame update
    void Start()
    {
        logAlpha = gameObject.GetComponent<SpriteRenderer>().color;
        logAlpha.a = 1;
        exitedLog = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().color = logAlpha;

        if (exitedLog)
        {
            increaseAlpha();
        }
    }

    //Decreases the alpha value until it hits zero
    public void decreaseAlpha()
    {
        if (logAlpha.a > 0)
        {
            logAlpha.a -= 0.02f;
        }
    }

    //Increases the alpha value until it hits 1
    public void increaseAlpha()
    {
        if (logAlpha.a < 1)
        {
            logAlpha.a += .02f;
        }
        else
        {
            logAlpha.a = 1;
            exitedLog = false;
        }
    }
    
    //Do not do trigger events if the object is a House, which has more than one way to exit the premisies
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && gameObject.tag != "House")
        {
            decreaseAlpha();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && gameObject.tag != "House")
        {
            exitedLog = true;
        }
    }
}
