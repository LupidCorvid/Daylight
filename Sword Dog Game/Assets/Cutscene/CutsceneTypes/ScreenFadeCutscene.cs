using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeCutscene : CutsceneData
{
    public bool FadeIn = false;

    private const int fadeTime = 1; //Animations are set to be 1 second long

    float startTime;
    public bool Stall = true;

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
        if (FadeIn)
            Crossfade.FadeStart?.Invoke();
        else
            Crossfade.FadeEnd?.Invoke();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (startTime + fadeTime <= Time.time || !Stall)
            finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        //Snap to fully faded in or out
    }
}
