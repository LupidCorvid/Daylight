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

    public void Start()
    {
        buildRoom();
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
        else
            SwordFollow.instance.SetActive(false);
        if(!roomState.finishedIntroCutscene)
        {
            roomState.finishedIntroCutscene = true;
            CutsceneController.PlayCutscene("Intro");
        }
    }

    public void collectSword()
    {
        //Giveplayer sword
        roomState.swordCollected = true;
        SwordFollow.instance.transform.position = looseSword.transform.position;
        Destroy(looseSword);
        SwordFollow.instance.SetActive(true);
    }
}
