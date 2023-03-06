using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : EnemyBase
{
    public void Awake()
    {
        ai = new BasicMoverAI(this);
    }
    public override void Start()
    {
        base.Start();
        ai.target = GameObject.Find("Player(Clone)").transform;
    }
}
