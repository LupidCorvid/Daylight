using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : RoomManager
{
    public PrologueAreaState roomState;

    public GameObject looseSword;

    public override void  Awake()
    {
        base.Awake();
        roomState = GameSaver.currData.roomStates.prologueState;
    }

    public override void receivedEvent(string name, params object[] parameters)
    {
        switch (name)
        {
            case "GetSword":
                collectSword();
                break;
        }
    }

    public void buildRoom()
    {
        if (roomState.swordCollected)
            Destroy(looseSword);
    }

    public void collectSword()
    {
        //Giveplayer sword
        roomState.swordCollected = true;
        Destroy(looseSword);
    }
}
