using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceCutscene : CutsceneData
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();
    public int currCutscene = 0;

    public override void startSegment()
    {
        base.startSegment();
        currCutscene = 0;
        cutscenes[currCutscene].startSegment();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        cutscenes[currCutscene].cycleExecution();
        if(cutscenes[currCutscene].finished)
        {
            currCutscene++;
            if (currCutscene >= cutscenes.Count)
                finishedSegment();
            else
                cutscenes[currCutscene].startSegment();
        }
    }

    public override void abort()
    {
        base.abort();
        cutscenes[currCutscene].abort();
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
    }
}
