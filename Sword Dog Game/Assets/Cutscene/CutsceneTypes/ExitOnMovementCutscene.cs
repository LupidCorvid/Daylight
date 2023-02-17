using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitOnMovementCutscene : CutsceneData
{

    public override void startSegment()
    {
        base.startSegment();

    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        //Temp implementation
        if(Input.GetAxisRaw("Horizontal") >= .01f || Input.GetKeyDown(KeyCode.Space))
        {
            finish();
        }
    }
}
