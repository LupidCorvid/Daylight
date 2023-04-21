using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeCutscene : CutsceneData
{
    public bool FadeIn = false;

    public override void startSegment()
    {
        base.startSegment();
        if (FadeIn)
            AudioManager.instance.FadeInCurrent();
        else
            AudioManager.instance.FadeOutCurrent();
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
