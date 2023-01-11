using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Concept for dot product design is from Game Endeavor
public class FlyingAI : BaseAI
{
    public Vector2 prefferedOffset = new Vector2(0, 4);
    public Vector2 acceptableRange = new Vector2(5, 1);

    public Vector2 targetPosition
    {
        get
        {
            return (Vector2)target.transform.position + (prefferedOffset);
        }
    }

    public override void Start()
    {
        base.Start();
        target ??= GameObject.Find("Player(Clone)").transform;
    }

    public FlyingAI(EnemyBase enemy) : base(enemy)
    {

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Mathf.Abs(targetPosition.x - transform.position.x ) > acceptableRange.x || Mathf.Abs(targetPosition.y - transform.position.y) > acceptableRange.y)
            Pursuit();
    }

    public void Pursuit()
    {
        Vector2 targetPosition = prefferedOffset + (Vector2)target.transform.position;

        ((FlyingMovement)movement).MoveDirection((targetPosition - (Vector2)transform.position).normalized * moveSpeed);
    }

}
