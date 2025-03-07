using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public BaseMovement movement;

    public Transform target;

    public float startMoveDistance = 4;
    public float stopMoveDistance = 3;

    public bool moving = false;

    public bool allowingForMovement = false;
    public Rigidbody2D rb;
    public bool currentlyTryingMove = false;

    public bool MovingToPoint = false;
    public Vector2 targPoint;
    public float pointTargDistance = 0.1f;

    public bool canRun = false;
    public float RunningDistance = 8;
    bool running = false;

    //Sprint logic needs to be moved to baseMovement
    public float walkSpeed = 4;
    public float sprintSpeed = 6;

    float moveSpeed
    {
        get { return running ? sprintSpeed : walkSpeed; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.simulated = allowingForMovement;

        if (!currentlyTryingMove)
            return;

        if (!MovingToPoint)
        {
            if (target == null)
                return;

            if (Mathf.Abs(transform.position.x - target.position.x) > startMoveDistance)
            {
                moving = true;

                if (canRun && Mathf.Abs(transform.position.x - target.position.x) > RunningDistance)
                    StartRunning();
            }
            else if (Mathf.Abs(transform.position.x - target.position.x) < stopMoveDistance)
            {
                moving = false;
            }


            if (moving)
            {
                if (transform.position.x > target.position.x)
                    movement.MoveLeft(moveSpeed);
                else
                    movement.MoveRight(moveSpeed);
            }
            else
            {
                StopRunning();
                movement.NotMoving();
            }
        }
        else
        {
            if (Mathf.Abs(transform.position.x - targPoint.x) > pointTargDistance)
            {
                moving = true;

                if (canRun && Mathf.Abs(transform.position.x - targPoint.x) > RunningDistance)
                    StartRunning();


                if (transform.position.x > targPoint.x)
                    movement.MoveLeft(moveSpeed);
                else
                    movement.MoveRight(moveSpeed);
            }
            else
            {
                moving = false;
                movement.NotMoving();
                StopRunning();
            }
        }
    }

    public void SetStopMoving()
    {
        movement.NotMoving();
    }

    public void StartRunning()
    {
        if(!running)
        {
            
        }

        running = true;
    }

    public void StopRunning()
    {
        if(running)
        {

        }
        running = false;
    }
}
