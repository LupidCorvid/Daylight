using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugShroom : EnemyBase
{
    public void Awake()
    {
        ai = new BugShroomAI(this);
    }

    public override void Start()
    {
        base.Start();
        //ai.target = GameObject.Find("Player(Clone)").transform;
    }
}
