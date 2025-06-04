using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventCutscene : CutsceneData
{
    public AK.Wwise.Event Event;
    public bool Global = true;
    public GameObject Object;
    public string CheckState = "";
    public AK.Wwise.State CheckStateValue;

    public override void startSegment()
    {
        base.startSegment();
        if (CheckState == "" || CheckStateValue == null)
        {
            Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
        }
        else
        {
            uint stateID;
            AkUnitySoundEngine.GetState(CheckState, out stateID);
            if (stateID != CheckStateValue.Id)
                Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
        }
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
