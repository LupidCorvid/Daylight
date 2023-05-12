using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithBehavior : DialogNPC
{
    Animator anim;
    float waitToLook = 0f;
    bool talking, finishTalkingSequence = false;

    Vector2 playerPosition = new Vector2(0, 0);
    bool foundPlayerPosition = false;

    public float dialogDistance;

    public string interruptDialog;
    private MiniBubbleController bubble;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (alreadyTalking)
            talkingToPlayer();
        else if (!finishTalkingSequence)
            idle();

        //if (alreadyTalking)
        //{
        //    if (Vector2.Distance(interactor.transform.position, transform.position) >= dialogDistance)
        //    {
        //        exitDialog();
        //        GameObject addedObj = Instantiate(miniBubblePrefab, transform.position + (Vector3)miniBubbleOffset, Quaternion.identity);
        //        bubble = addedObj.GetComponent<MiniBubbleController>();
        //        bubble.speaker = this;
        //        bubble.offset = miniBubbleOffset;
        //        bubble.setSource(new DialogSource(interruptDialog));
        //    }
        //}
    }

    private void idle()
    {
        anim.Play("bsIdle");
    }

    private void talkingToPlayer()
    {
        //Find the side the player is on
        if (!foundPlayerPosition)
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position; //Edit for possible null reference
            foundPlayerPosition = true;
        }
    }

    public override void interact(Entity user)
    {
        base.interact(user);
        if (bubble != null && bubble.gameObject != null)
            bubble.close();
    }
    public override void exitDialog()
    {
        base.exitDialog();
        StartCoroutine(finishedTalking());

    }
    IEnumerator finishedTalking()
    {
        foundPlayerPosition = false;
        //waitToLook = 0;
        //if ((playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
        //        || (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX))
        //{
        //    anim.Play("GEN_resetHead");
        //}

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }
}
