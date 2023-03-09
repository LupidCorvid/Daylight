using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biter : EnemyBase
{
    public void Awake()
    {
        ai = new BiterAI(this);
    }
}
