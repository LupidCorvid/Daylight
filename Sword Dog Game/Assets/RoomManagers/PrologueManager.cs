using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : RoomManager
{
    public PrologueAreaState roomState;

    public Entity prologueMonster;

    public GameObject looseSword;
    public GameObject NoSwordBlock;

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
