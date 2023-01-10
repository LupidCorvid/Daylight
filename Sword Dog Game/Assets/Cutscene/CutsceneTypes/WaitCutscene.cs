using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitCutscene : CutsceneData
{
    public float waitTime;

    float startTime;
    public WaitCutscene(float time)
    {
        waitTime = time;
    }

    public WaitCutscene()
    {

    }

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (startTime + waitTime <= Time.time)
            finishedSegment();
    }

}
