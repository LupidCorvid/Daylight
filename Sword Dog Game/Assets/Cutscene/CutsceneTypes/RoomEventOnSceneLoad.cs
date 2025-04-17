using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomEventOnSceneLoad : CutsceneData
{
    public string eventName = "";
    public string[] eventParams;
    bool called = false;
    
    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null && called == false)
        {
            RoomManager.currentRoom.callRoomEvent(eventName);
            called = true;
            Destroy(gameObject); //Prevents infinite update call
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
