using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBehavior : DialogNPC, ICutsceneCallable
{
    Animator anim;
    float waitToLook = 0f;
    bool talking, finishTalkingSequence = false;

    Vector2 playerPosition = new Vector2(0, 0);
    bool foundPlayerPosition = false;

    public float dialogDistance;

    public string interruptDialog;
    private MiniBubbleController bubble;

    public NPCFollow followScript;
    public GameObject sword;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        //idle()

        if(alreadyTalking)
        {
            if(Vector2.Distance(interactor.transform.position, transform.position) >= dialogDistance)
            {
                exitDialog();
                GameObject addedObj = Instantiate(miniBubblePrefab, transform.position + (Vector3)miniBubbleOffset, Quaternion.identity);
                bubble = addedObj.GetComponent<MiniBubbleController>();
                bubble.speaker = this;
                bubble.offset = miniBubbleOffset;
                bubble.setSource(new DialogSource(interruptDialog));
            }
        }
    }

    public override void interact(Entity user)
    {
        base.interact(user);
        // TODO set this depending on facing direction!!
        dialogSource.defaultBarkVelocity = new Vector2(-1, -1);
        if (bubble != null && bubble.gameObject != null)
            bubble.close();
    }

    private void idle()
    {
        if (alreadyTalking)
            talkingToPlayer();
        else if (!finishTalkingSequence)
            waitToLook += Time.deltaTime;
            if (waitToLook >= 7) anim.Play("GEN_look");
            else anim.Play("GEN_idle");
            if (waitToLook >= 15) waitToLook = 0;
    }

    public override void exitDialog()
    {
        base.exitDialog();
        StartCoroutine(finishedTalking());

    }

    //Turns the head to look at the player if player is moving while talking to the NPC
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
            anim.Play("GEN_lookAtPlayer");

        //If the player is on his right and he's facing left...
        //TODO: currently not working
        if (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX)
            anim.Play("GEN_lookAtPlayer");
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

    //Dialog Events Below

    public void CutsceneEvent(string functionToCall)
    {
        switch (functionToCall)
        {
            case "turnAnimGeneral":
                followScript.TurnAnim();
                break;
        }
    }
}
