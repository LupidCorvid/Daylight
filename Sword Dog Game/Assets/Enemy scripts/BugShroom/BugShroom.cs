using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugShroom : EnemyBase
{

    public override bool stunned
    {
        get => base.stunned;

        set
        {
            ((BugShroomAI)ai).state = BugShroomAI.AIState.idle;
            if (value)
            {
                anim.speed = 0;
                GetComponent<Rigidbody2D>().drag = 5;
            }
            else
            {
                anim.speed = 1;
                GetComponent<Rigidbody2D>().drag = 0;
            }

            base.stunned = value;
        }
    }
    public void Awake()
    {
        ai = new BugShroomAI(this);
    }

    public override void Start()
    {
        base.Start();
        //ai.target = GameObject.Find("Player(Clone)").transform;
    }

    //The bugshroom attack can't really be parried.
}
