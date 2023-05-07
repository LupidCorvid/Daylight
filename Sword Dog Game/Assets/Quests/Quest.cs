using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public float progress;
    public int assigner; //ID of NPC
    public QuestType type;
    public int questId;
    public string questDescription;
    public string rewardsDescription;
    public bool assigned = false;

    public enum QuestType
    {
        story,
        side
    }

    public virtual void complete()
    {

    }
}
