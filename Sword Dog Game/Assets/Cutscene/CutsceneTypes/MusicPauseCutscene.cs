using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPauseCutscene : CutsceneData
{
    public bool Pause = false;

    public override void startSegment()
    {
        base.startSegment();
        if (Pause)
            AudioManager.instance.PauseCurrent();
        else
            AudioManager.instance.UnPauseCurrent();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        finishedSegment();
    }

    public override void abort()
    {
        base.abort();
    }
}
