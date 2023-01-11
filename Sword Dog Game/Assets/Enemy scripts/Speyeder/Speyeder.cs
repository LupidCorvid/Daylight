using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speyeder : EnemyBase
{
    public DistanceJoint2D web;

    private void Awake()
    {
        ai = new SpeyederAI(this, web);

    }
}
