using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RivalBehavior : DialogNPC, ICutsceneCallable
{
    Animator rivalAnim, swordAnim;
    bool prologueBehaviorActive = true;
    float animWaitTime = 0f; //For waiting for an animation to execute
    
    public Entity monster;

    bool monsterTransitionExecuted = false; //For transitioning from cowering to idle

    //Copy pasted from general code
    //Animator anim;
    float waitToLook = 0f;
    bool talking, finishTalkingSequence = false;
    Vector2 playerPosition = new Vector2(0, 0);
    bool foundPlayerPosition = false;
    public float dialogDistance;
    public string interruptDialog;
    private MiniBubbleController bubble;

    public bool isTurning = false; //Toggle if turnAnim is triggered //Attempt for turn animation
    public GameObject rivalSword;

    public NPCFollow followScript;

    void Start()
    {
        rivalAnim = gameObject.GetComponent<Animator>();
        swordAnim = GameObject.Find("rival sword").GetComponent<Animator>();
        rivalSword = GameObject.Find("rival sword");
        if (monster != null)
            monster.killed += monsterKilled;

        if (SceneManager.GetActiveScene().name != "prologue area")
            prologueBehaviorActive = false;
    }
    

    //Copy pasted from general code
    void FixedUpdate()
    {
        if (prologueBehaviorActive)
            prologueBehavior();

        if (alreadyTalking && rivalAnim.GetCurrentAnimatorStateInfo(0).IsName("rival_idle"))
        {
            talkingToPlayer();
        }

        if (talking && !isTurning && !prologueBehaviorActive) speak();
    }

    public void idle()
    {
        swordAnim.Play("sword_idle");
        if (alreadyTalking)
            talkingToPlayer();
        else if (!finishTalkingSequence)
            rivalAnim.Play("rival_idle");
    }

    public void talkingToPlayer()
    {
        rivalAnim.Play("rival_silentToSpeak");
    }

    public override void exitDialog()
    {
        base.exitDialog();
        if(rivalAnim != null)
            rivalAnim.SetTrigger("FinishedTalking");
        //StartCoroutine(finishedTalking());

    }
    public override void interact(Entity user)
    {
        base.interact(user);
        if (bubble != null && bubble.gameObject != null)
            bubble.close();
    }
    IEnumerator finishedTalking()
    {
        //foundPlayerPosition = false;
        //waitToLook = 0;
        //if ((playerPosition.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
        //        || (playerPosition.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX))
        //{
        //    rivalAnim.Play("rival_speakToSilent");
        //}

        yield return new WaitForSeconds(2);

        finishTalkingSequence = false;
    }

    void speak()
    {
        rivalAnim.Play("rival_speak");
    }

    void walking()
    {
        rivalAnim.Play("rival_walk");
    }
    bool monsterDefeated = false;

    void prologueBehavior()
    {

        if (((PrologueManager)RoomManager.currentRoom)?.roomState.prologueMonsterKilled == true)
            monsterDefeated = true;

        //Rival cowers in front of the monster
        if (!monsterDefeated)
        {
            rivalAnim.Play("rival_cower");
            swordAnim.Play("sword_cower");
        }

        //Monster is defeated, getting up to idle is executed
        if(monsterDefeated && !monsterTransitionExecuted)
        {
            animWaitTime += Time.deltaTime;
            rivalAnim.Play("rival_cowerToIdle");
            swordAnim.Play("sword_cowerToIdle");
            //Debug.Log("Setting rival anim");

            if (animWaitTime >= 1)
            {
                animWaitTime = 0f;
                monsterTransitionExecuted = true;
                //print("animWaitTime Done");
            }
            
        }
    }

    //Dialog Events Below
    
    public void CutsceneEvent(string functionToCall)
    {
        switch (functionToCall)
        {
            case "flashRed":
                rivalAnim.Play("rival_injury");
                break;
            case "turnAnimRival":
                followScript.TurnAnim();
                //print("triggered turnAnimRival");
                break;
        }
    }

    public void monsterKilled()
    {
        //if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
        //{
        //    monsterDefeated = true;
        //    CutsceneController.PlayCutscene("SavedFromMonster");
        //}
    }

    //Used for [CE, p1, p2, ...]
    //Takes an arbitrary number of arguments, use how ever you need
    //This calls another cutscene from Unity, and it happens at the same time as the currect cutscene
    public override void eventCalled(params string[] input)
    {
        switch(input[0])
        {
            case "backPedal":
                CutsceneController.PlayCutscene("U_rickenBackpedal");
                break;
        }
    }
}
