using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitCutscene : CutsceneData
{
    public float waitTime;

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
        wait();
    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(waitTime);
        finishedSegment();
    }
}
