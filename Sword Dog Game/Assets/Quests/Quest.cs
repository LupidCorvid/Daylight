using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public float progress;
    public float neededProgress = 1;
    public int assigner; //ID of NPC
    public QuestType type;
    public int questId;
    public string questDescription;
    public string rewardsDescription;
    public bool assigned = false;
    public bool completed = false;

    public enum QuestType
    {
        story,
        side
    }

    public virtual void complete()
    {
        completed = true;
        //finalizeCompletion();
        
    }

    public virtual void finalizeCompletion()
    {
        //Should remove the quest from the save data
        //This is called from here instead of in complete since complete is called when progress is fulfilled, which can already
        //be detected by checking on update progress
        QuestsManager.main.QuestEvent?.Invoke(this, QuestsManager.QuestEventType.Completed);
    }

    public virtual void updateProgress()
    {
        QuestsManager.main.QuestEvent?.Invoke(this, QuestsManager.QuestEventType.Updated);
    }

    public virtual void onAssign()
    {
        QuestsManager.main.QuestEvent?.Invoke(this, QuestsManager.QuestEventType.Assigned);
    }
}
