using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrologueManager : RoomManager
{
    //Only variables put in roomState will be saved, any variables locally here will not be saved upon leaving the room

    public PrologueAreaState roomState;

    public Entity prologueMonster;

    public GameObject looseSword;
    public GameObject NoSwordBlock;
    public GameObject Ricken;

    bool attemptedLeave = false;

    public override void  Awake() //Called immediately when the object is loaded into the scene
    {
        base.Awake();
        roomState = GameSaver.currData.roomStates.prologueState; //Reads in the save data (copy by reference)
    }

    public void Start() //Called on frame 1
    {
        buildRoom();
    }

    public override void receivedEvent(string name, params object[] parameters)
    {
        switch (name)
        {
            case "SpawnSwordPrompt":
                looseSword.GetComponent<SwordPickupBehavior>().spawnPrompt();
                break;
            case "GetSword":
                collectSword();
                break;
            case "ApproachedRival":
                if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene && !DialogSource.stringVariables.ContainsKey("ListenedWithSword") && !DialogController.dialogOpen)
                {
                    CutsceneController.PlayCutscene("RivalApproached");
                }
                break;
            case "AttemptedLeave":
                if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene && roomState.prologueMonsterKilled && !attemptedLeave)
                {
                    CutsceneController.PlayCutscene("AttemptedLeave");
                    attemptedLeave = true;
                }
                break;
            case "EnemyPan":
                if(!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
                {
                    CutsceneController.PlayCutscene("EnemyPan");
                }
                break;
            case "MonsterKilled":
                if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
                {
                    CutsceneController.PlayCutscene("SavedFromMonster");
                }
                break;
        }
    }

    //If the room will look different depending on the save data, use this function
    //Not required
    public void buildRoom()
    {
        //Make modifications according to the save data
        if (roomState.swordCollected)
            Destroy(looseSword);
        else
        {
            SwordFollow.instance.SetActive(false);
            NoSwordBlock.SetActive(true);
        }
        if(!roomState.finishedIntroCutscene && !roomState.swordCollected)
        {
            //roomState.finishedIntroCutscene = true;
            CutsceneController.PlayCutscene("Intro");
        }

        if (roomState.prologueMonsterKilled)
            Destroy(prologueMonster.gameObject);
        else
            prologueMonster.killed += savedFriend;

        if (!QuestsManager.main.checkIfAssigned(0))
            QuestsManager.main.AssignQuest(new GetupQuest());
    }

    public void savedFriend()
    {
        roomState.prologueMonsterKilled = true;
        QuestsManager.main.setQuestProgress(new GetupQuest(), 1);

        if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
        {
            CutsceneController.PlayCutscene("SavedFromMonster");
        }
    }


    public void collectSword()
    {
        //Giveplayer sword
        roomState.swordCollected = true;
        if (!DialogSource.stringVariables.ContainsKey("HasGottenSword"))
            DialogSource.stringVariables.Add("HasGottenSword", "True");
        else
            DialogSource.stringVariables["HasGottenSword"] = "True";
        SwordFollow.instance.transform.position = looseSword.transform.position;
        SwordFollow.instance.transform.rotation = looseSword.transform.GetChild(0).rotation;
        Destroy(looseSword);
        SwordFollow.instance.SetActive(true);
        NoSwordBlock.SetActive(false);
    }

    private void OnDestroy()
    {
        roomState.finishedIntroCutscene = true;
    }
}
