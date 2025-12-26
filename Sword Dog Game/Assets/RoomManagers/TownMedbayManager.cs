using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMedbayManager : MonoBehaviour
{
    public MerylBehavior meryl;

    // Start is called before the first frame update
    void Start()
    {

        QuestsManager.main.setQuestProgress(new MerylAloeQuest(), Player.controller.entityBase.getAssociatedInventory().CountItem(new TeardropAloe()));
        Debug.Log("Quest progress: " + Player.controller.entityBase.getAssociatedInventory().CountItem(new TeardropAloe()) + " / " + QuestsManager.main.getQuest(1).neededProgress);

        if (!QuestsManager.main.getQuest(1).assigned && !QuestsManager.main.getQuest(1).completed)
            meryl.dialog[0] = "[lf,Meryl.txt,Prologue5.1]";


        if(QuestsManager.main.getQuest(1).progress >= QuestsManager.main.getQuest(1).neededProgress)
        {
            QuestsManager.main.setQuestCompletion(1, true);
            CutsceneController.PlayCutscene("AloeQuestFinish");
        }
    }
}
