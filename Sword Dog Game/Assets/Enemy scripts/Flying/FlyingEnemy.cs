using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    public void Awake()
    {
        ai = new FlyingAI(this);
    }

}
