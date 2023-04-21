using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeCutscene : CutsceneData
{
    public bool FadeIn = false;
    public float Duration = 1f;

    public override void startSegment()
    {
        base.startSegment();
        if (FadeIn)
            AudioManager.instance.FadeInCurrent(Duration);
        else
            AudioManager.instance.FadeOutCurrent(Duration);
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        //Snap to fully faded in or out
    }


}
