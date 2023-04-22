using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeCutscene : CutsceneData
{
    public bool FadeIn = false;
    public float Duration = 1f;
    float startTime;
    public bool Stall = false;

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
        if (FadeIn)
            AudioManager.instance.FadeInCurrent(Duration);
        else
            AudioManager.instance.FadeOutCurrent(Duration);
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (startTime + Duration <= Time.time || !Stall)
            finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        //Snap to fully faded in or out
    }
}
