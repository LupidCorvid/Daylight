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

    public bool SpriteFacesRight = true;

    bool turning = false;

    public Animator anim;

    public bool ControlFlip = true;
    public string turnAnimName;

    float moveSpeed
    {
        get { return running ? sprintSpeed : walkSpeed; }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    //Some NPCS have swords to animate as well
    public Animator swordAnim;
    public string swordTurnAnim = "";

    float turnMinTime = .1f;
    float turnStartTime = 0;

    // Update is called once per frame
    void Update()
    {
        rb.simulated = allowingForMovement;

        if (turning && !currentlyTryingMove)
        {
            TurnAnim();
        }

        if (!currentlyTryingMove)
            return;

        //Rptate if not already facing right direction
        if(turning || (target != null && (!turning && (transform.position.x - target.position.x < 0 ^ (gameObject.transform.localScale.x > 0 ^ !SpriteFacesRight)))))
        {
            TurnAnim();
        }


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
                    MoveLeft();
                else
                    MoveRight();
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
                    MoveLeft();
                else
                    MoveRight();
            }
            else
            {
                moving = false;
                movement.NotMoving();
                StopRunning();
            }
        }
    }

    public void TurnAnim()
    {
        if (!turning)
        {
            turning = true;
            
            anim.Play(turnAnimName);
            swordAnim?.Play(swordTurnAnim);
            turnStartTime = Time.time;
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName(turnAnimName) && turnMinTime + turnStartTime < Time.time)
        {
            turning = false;
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
    }

    public void SetStopMoving()
    {
        movement.NotMoving();
    }

    public void StartRunning()
    {
        if (!running)
        {

        }

        running = true;
    }

    public void StopRunning()
    {
        if (running)
        {

        }
        running = false;
    }

    public void MoveLeft()
    {
        if ((gameObject.transform.localScale.x > 0 ^ !SpriteFacesRight)&& !turning)
            TurnAnim();
        movement.MoveLeft();
    }

    public void MoveRight()
    {
        if ((gameObject.transform.localScale.x < 0 ^ !SpriteFacesRight)&& !turning)
            TurnAnim();
        movement.MoveRight();
    }
}
