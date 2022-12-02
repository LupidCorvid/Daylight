using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeepBehavior : MonoBehaviour
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!talking)
        {
            if (finishTalkingSequence) StartCoroutine(finishedTalking());

            else idle();

            //Debug
            if (Input.GetKeyDown(KeyCode.Alpha9) && !finishTalkingSequence)
            {
                talking = true;
            }
        }

        else talkingToPlayer();
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

        //Find the side the player is on
        if (!foundPlayerPosition)
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position; //Edit for possible null reference
            foundPlayerPosition = true;
        }

        //Play the head turn anim
        if(playerPosition.x < transform.position.x)
        {
            anim.Play("SK_upright_lookAtPlayer");
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            finishTalkingSequence = true;
            talking = false;
        }
    }

    IEnumerator finishedTalking()
    {
        foundPlayerPosition = false;
        waitToLook = 0;
        if(playerPosition.x < transform.position.x) anim.Play("SK_upright_resetHead");

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }

}
