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
        //Temp implementation. Can be final as long as all attempts at player movement are considered
        if(Input.GetAxisRaw("Horizontal") >= .01f || Input.GetKeyDown(KeyCode.Space))
        {
            finish();
        }
    }
}
