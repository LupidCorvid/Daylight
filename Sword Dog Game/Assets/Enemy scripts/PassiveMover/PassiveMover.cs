using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveMover : EnemyBase
{
    public void Awake()
    {
        ai = new PassiveMoverAI(this);
    }
}
