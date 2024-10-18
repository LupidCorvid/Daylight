using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeCutscene : CutsceneData
{
    public bool FadeToBlack = false;

    private const int fadeTime = 1; //Animations are set to be 1 second long

    float startTime;
    public bool Stall = true;
    public float speed = 1;

    public override void startSegment()
    {
        Crossfade.current.speed = speed;
        base.startSegment();
        startTime = Time.time;
        if (FadeToBlack)
        {
            Crossfade.FadeStart?.Invoke();
        }
        else
        {
            Crossfade.FadeEnd?.Invoke();
        }
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
        Crossfade.current.speed = 1;
        //Snap to fully faded in or out
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        Crossfade.current.speed = speed;
    }
}
