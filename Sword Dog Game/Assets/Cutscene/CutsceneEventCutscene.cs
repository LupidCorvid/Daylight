using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEventCutscene : CutsceneData
{
    public ICutsceneCallable eventToCall;
    public string parameters = "";

    public override void startSegment()
    {
        base.startSegment();
        eventToCall.CutsceneEvent(parameters);
        finishedSegment();
    }

}
