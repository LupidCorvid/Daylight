using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBehavior : DialogNPC
{
    Animator anim;
    float waitToLook = 0f;
    bool talking, finishTalkingSequence = false;

    Vector2 playerPosition = new Vector2(0, 0);
    bool foundPlayerPosition = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (alreadyTalking)
            talkingToPlayer();
        else if (!finishTalkingSequence)
            idle();
    }

    private void idle()
    {
        waitToLook += Time.deltaTime;
        // print(waitToLook);
        

        if (waitToLook >= 7)
        {
            anim.Play("GEN_look");
        }
        else
        {
            anim.Play("GEN_idle");
        }

        if (waitToLook >= 15) waitToLook = 0;
    }


    public override void exitDialog()
    {
        base.exitDialog();
        StartCoroutine(finishedTalking());

    }
    private void talkingToPlayer()
    {
        //if the player is on the left side, play turn head animation
        //When the player finishes talking, if the head is turned, reset the position

        //Find the side the player is on
        if (!foundPlayerPosition)
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position; //Edit for possible null reference
            foundPlayerPosition = true;
        }
        
        //If the player is on his left and he's facing right...
        if (playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            anim.Play("GEN_lookAtPlayer");
        }

        //If the player is on his right and he's facing left...
        //TODO: currently not working
        if (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            anim.Play("GEN_lookAtPlayer");
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            finishTalkingSequence = true;
            talking = false;
        }
    }

    IEnumerator finishedTalking()
    {
        foundPlayerPosition = false;
        waitToLook = 0;
        if ((playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX) 
                || (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX))
        {
            anim.Play("GEN_resetHead");
        }

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }

}
