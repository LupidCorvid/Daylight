using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    public float scaleAnimator = 1;

    public void Awake()
    {
        ai = new FlyingAI(this);
    }

}
