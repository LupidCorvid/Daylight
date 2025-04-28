using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerylBehavior : DialogNPC
{
    Animator anim;
    //float waitToLook = 0f;
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
    /*
    public void questIsAsked()
    {
        bool accept;
        string test = Console.ReadLine();
        if(test == "yes")
        {
            accept = true;
        }
        else
        {

        }

        
        


    }*/
    private void idle()
    {
        anim.Play("MED_idle");
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

        //TODO: make head turn animation so that this can be implemented

        //If the player is on the left and npc is facing right...
        /*if (playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            anim.Play("GEN_lookAtPlayer");
        }

        //If the player is on the right and npc is facing left...
        //TODO: currently not working
        if (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            anim.Play("GEN_lookAtPlayer");
        }*/
    }

    IEnumerator finishedTalking()
    {
        foundPlayerPosition = false;
        //waitToLook = 0;
        /*
        if ((playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
                || (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX))
        {
            anim.Play("GEN_resetHead");
        }*/

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }
    public override void interact(Entity user)
    {
        base.interact(user);
        QuestsManager.main.setQuestProgress(new MerylAloeQuest(), user.getAssociatedInventory().CountItem(new TeardropAloe()));
    }

    public override void eventCalled(params string[] input)
    {
        base.eventCalled(input);
        switch(input[0])
        {
            case "heal":
                healUser();
                break;
            case "AloeQuestAssign":
                CanvasManager.main.queuedQuestNotif = ("New quest: " + new MerylAloeQuest().questDescription);
                QuestsManager.main.AssignQuest(new MerylAloeQuest());
                break;
            case "AloePayment":
                Utilities.main.SpawnLooseItem(new Bone(10), interactor.transform.position);
                interactor.getAssociatedInventory().RemoveItem(new TeardropAloe(), 2);
                CanvasManager.main.queuedQuestNotif = ("Completed quest: " + new MerylAloeQuest().questDescription);
                QuestsManager.main.setQuestCompletion(1, true);
                break;
        }
    }
    public void healUser()
    {
        interactor?.GetComponent<PlayerHealth>()?.Heal(100);
    }

}
