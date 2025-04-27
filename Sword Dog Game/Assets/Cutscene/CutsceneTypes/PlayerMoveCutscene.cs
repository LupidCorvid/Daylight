using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveCutscene : CutsceneData
{
    public bool goToPoint = false;
    public Transform targ;
    public List<Vector2> targPoints = new List<Vector2>();
    public bool backpedal = false;
    public bool keepTargetsAfter = false;
    int currPoint = 0;
    PlayerMovement player;
    public bool sprinting = false;
    private bool couldSprint = true;

    public override void startSegment()
    {
        base.startSegment();
        player = Player.controller;
        player.externalControl = true;
        player.isSprinting = sprinting;

        if (!sprinting)
            player.StopSprint();

        //targetNPC.backpedal = backpedal;
    }

    public override void cycleExecution()
    {
        base.cycleExecution();

        if(player == null)
            player = Player.controller;

        if (goToPoint)
        {
            if (Mathf.Abs(targPoints[currPoint].x - player.transform.position.x) < 5)
            {
                if (currPoint < targPoints.Count - 1)
                {
                    currPoint++;
                    
                }
                else
                {
                    //player.stopMovement();
                    finishedSegment();
                }
            }
        }
        else if (Mathf.Abs(targ.position.x - player.transform.position.x) < 5)
        {
            finishedSegment();
            Debug.Log("logger");
        }

        MovePlayerToPoint();
    }

    Vector2 getCurrentTarget()
    {
        if (goToPoint)
            return targPoints[currPoint];
        else
            return targ.transform.position;
    }

    public void MovePlayerToPoint()
    {
        if (!sprinting)
        {
            player.StopSprint();
        }

        player.FakeInput(((getCurrentTarget().x - player.transform.position.x) > 0) ? 1 : -1);
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        player.externalControl = false;
    }
}
