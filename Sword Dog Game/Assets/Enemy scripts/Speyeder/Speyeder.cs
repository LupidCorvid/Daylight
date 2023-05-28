using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speyeder : EnemyBase
{
    public DistanceJoint2D web;

    public override bool stunned 
    { 
        get => base.stunned; 
    
        set 
        {
            ((SpeyederAI)ai).state = SpeyederAI.states.returning;
        } 
    }

    private void Awake()
    {
        ai = new SpeyederAI(this, web);

    }

    public override void Parried()
    {
        base.Parried();
    }

    public override void Blocked()
    {
        base.Blocked();
        web.distance = Vector2.Distance(transform.position, web.connectedBody.position);
        ((SpeyederAI)ai).state = SpeyederAI.states.landStop;
        ai.anim.SetTrigger("Land");
    }
}
