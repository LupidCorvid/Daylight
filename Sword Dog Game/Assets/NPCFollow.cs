using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public BaseMovement movement;

    public Transform target;

    public float startMoveDistance = 4;
    public float stopMoveDistance = 3;

    bool moving = false;

    public bool allowingForMovement = false;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.simulated = allowingForMovement;

        if (target == null)
            return;

        if(Mathf.Abs(transform.position.x - target.position.x) > startMoveDistance)
        {
            moving = true;
        }
        else if(Mathf.Abs(transform.position.x - target.position.x) < stopMoveDistance)
        {
            moving = false;
        }


        if (moving)
        {
            if (transform.position.x > target.position.x)
                movement.MoveLeft();
            else
                movement.MoveRight();
        }
        else
            movement.NotMoving();
    }
}
