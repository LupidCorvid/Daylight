using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCutscene : CutsceneData
{
    public CutsceneController otherCutscene;

    public override void startSegment()
    {
        base.startSegment();
        otherCutscene.StartCutscene();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (!otherCutscene.playingThisCutscene)
            finishedSegment();
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
    }
}
