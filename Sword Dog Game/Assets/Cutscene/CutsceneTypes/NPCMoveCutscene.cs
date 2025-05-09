using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveCutscene : CutsceneData
{
    public NPCFollow targetNPC;
    public bool goToPoint = false;
    public Transform targ;
    public List<Vector2> targPoints = new List<Vector2>();
    public bool backpedal = false;
    public bool lockMovementAfter = true;
    public bool keepTargetsAfter = false;
    int currPoint = 0;
    public Entity AIToFreeze;
    public bool sprint = false;

    bool origSprint = false;

    public override void startSegment()
    {
        base.startSegment();
        targetNPC.allowingForMovement = true;
        targetNPC.currentlyTryingMove = true;
        targetNPC.MovingToPoint = goToPoint;
        if (goToPoint)
        {
            targetNPC.targPoint = targPoints[currPoint];
        }
        else
            targetNPC.target = targ;
        targetNPC.backpedal = backpedal;
        if (AIToFreeze != null)
            AIToFreeze.freezeAI = true;

        origSprint = targetNPC.running;
        targetNPC.running = sprint;
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if(goToPoint)
        {
            if(Mathf.Abs(targPoints[currPoint].x - targetNPC.transform.position.x) < 5)
            {
                if (currPoint < targPoints.Count - 1)
                {
                    currPoint++;
                    targetNPC.targPoint = targPoints[currPoint];
                }
                else
                {
                    targetNPC.SetStopMoving();
                    finishedSegment();
                }
            }
        }
        else if(!targetNPC.moving || Mathf.Abs(targ.position.x - targetNPC.transform.position.x) < 5)
        {
            finishedSegment();
            Debug.Log("logger");
        }
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        if(lockMovementAfter)
        {
            targetNPC.allowingForMovement = false;
        }
        if(!keepTargetsAfter)
        {
            targetNPC.currentlyTryingMove = false;
        }
        backpedal = false;
        if (AIToFreeze != null)
            AIToFreeze.freezeAI = false;
        targetNPC.running = origSprint;
    }
}
