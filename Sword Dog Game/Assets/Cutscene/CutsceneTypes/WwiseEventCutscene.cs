using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventCutscene : CutsceneData
{
    public AK.Wwise.Event Event;
    public bool Global = true;
    public GameObject Object;

    public override void startSegment()
    {
        base.startSegment();
        Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
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
