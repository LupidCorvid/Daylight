using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeCutscene : CutsceneData
{
    public bool FadeIn = false;

    private const int fadeTime = 1; //Animations are set to be 1 second long

    float startTime;

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
        if (FadeIn)
            Crossfade.current.StartFade();
        else
            Crossfade.current.StopFade();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (startTime + fadeTime <= Time.time)
            finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        //Snap to fully faded in or out
            
    }


}
