using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunger : EnemyBase
{
    public PhysicsMaterial2D slippery;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D stopping;

    public void Awake()
    {
        ai = new LungerAI(this);
    }

    public override void Start()
    {
        base.Start();
        ai.target = GameObject.Find("Player(Clone)").transform;
    }
}
