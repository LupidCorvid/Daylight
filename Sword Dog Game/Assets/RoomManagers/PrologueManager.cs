using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : RoomManager
{
    public PrologueAreaState roomState;

    public Entity prologueMonster;

    public GameObject looseSword;

    bool attemptedLeave = false;

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
            case "ApproachedRival":
                if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
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
            //case "MonsterKilled":
            //    if (!GameSaver.currData.roomStates.prologueState.finishedIntroCutscene)
            //    {
            //        CutsceneController.PlayCutscene("SavedFromMonster");
            //    }
            //    break;
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
        SwordFollow.instance.transform.position = looseSword.transform.position;
        SwordFollow.instance.transform.rotation = looseSword.transform.GetChild(0).rotation;
        Destroy(looseSword);
        SwordFollow.instance.SetActive(true);
    }

    private void OnDestroy()
    {
        roomState.finishedIntroCutscene = true;
    }
}
