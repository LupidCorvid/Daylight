using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalBehavior : MonoBehaviour
{
    Animator rivalAnim, swordAnim;
    bool prologueBehaviorActive = true;
    float animWaitTime = 0f; //For waiting for an animation to execute

    void Start()
    {
        rivalAnim = gameObject.GetComponent<Animator>();
        swordAnim = GameObject.Find("rival sword").GetComponent<Animator>();
    }
    
    void Update()
    {
        if(prologueBehaviorActive)
            prologueBehavior();
        

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
        
        bool monsterTransitionExecuted = false; //For transitioning from cowering to idle

        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            monsterDefeated = true;
        }

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

            if (animWaitTime >= 1)
            {
                animWaitTime = 0f;
                monsterTransitionExecuted = true;
                print("animWaitTime Done");
            } 
        }

        //Talking to player

        //Leaving for the town
    }
}