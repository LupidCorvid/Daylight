using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeepBehavior : DialogNPC
{
    Animator anim;
    float waitToLook = 0f;
    bool talking, finishTalkingSequence = false;

    Transform playerPosition;
    bool foundPlayerPosition = false;

    public float advertisingCooldown = 15;
    public float lastAdvert = -15;

    public CollisionsTracker advertCldr;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        advertCldr.triggerEnter += advertise;
    }

    public void advertise(Collider2D collider)
    {
        if(!alreadyTalking && lastAdvert + advertisingCooldown < Time.time)
        {
            GameObject addedObj = Instantiate(miniBubblePrefab, transform.position + (Vector3)miniBubbleOffset, Quaternion.identity);
            MiniBubbleController bubble = addedObj.GetComponent<MiniBubbleController>();
            bubble.speaker = this;
            bubble.offset = miniBubbleOffset;
            bubble.setSource(new DialogSource("[ss, .05][IA,<size=75%>]Interested in buying anything?[w, 1] [exit]"));
            lastAdvert = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (alreadyTalking)
            talkingToPlayer();
        else if(!finishTalkingSequence)
            idle();
    }

    private void idle()
    {
        waitToLook += Time.deltaTime;

        if (waitToLook >= 7)
        {
            anim.Play("SK_upright_look");
        }
        else
        {
            anim.Play("SK_upright_idle");
        }

        if (waitToLook >= 9) waitToLook = 0;
    }


    private void talkingToPlayer()
    {
        //if the player is on the left side, play turn head animation
        //When the player finishes talking, if the head is turned, reset the position

        //Play the head turn anim
        if(playerPosition.position.x < transform.position.x)
        {
            anim.Play("SK_upright_lookAtPlayer");
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("SK_upright_idle"))
        {
            anim.Play("SK_upright_resetHead");
        }

    }

    public override void exitDialog()
    {
        base.exitDialog();
        StartCoroutine(finishedTalking());
        
    }

    public override void interact(GameObject user)
    {
        base.interact(user);
        playerPosition = user.transform;
    }
    IEnumerator finishedTalking()
    {
        foundPlayerPosition = false;
        waitToLook = 0;
        if(playerPosition.position.x < transform.position.x) anim.Play("SK_upright_resetHead");
        finishTalkingSequence = true;

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }
    public override void eventCalled(params string[] input)
    {
        base.eventCalled(input);
        if (input.Length == 0)
            return;
        if (input[0] == "OpenShop")
            openShop();
    }

    public void openShop()
    {
        Debug.Log("Attempted to open shop. Shop is not yet implemented!");
    }

}
