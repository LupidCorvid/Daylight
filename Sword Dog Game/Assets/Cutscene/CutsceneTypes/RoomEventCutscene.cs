using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventCutscene : CutsceneData
{
    public string eventName = "";
    public string[] eventParams; //TODO will we want more flexible data types? object[] does not show in inspector

    public override void startSegment()
    {
        base.startSegment();
        RoomManager.currentRoom.callRoomEvent(eventName, eventParams);
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
