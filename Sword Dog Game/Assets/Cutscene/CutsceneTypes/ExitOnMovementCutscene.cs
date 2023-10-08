using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

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
        if(PlayerMovement.inputs.actions["Move"].ReadValue<Vector2>().magnitude > 0.1f || PlayerMovement.inputs.actions["Jump"].WasPressedThisFrame())
        {
            finish();
        }
    }
}
