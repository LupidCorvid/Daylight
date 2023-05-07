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
        //If you don't want rewards to be completed as soon as finished, override without calling base
        completed = true;
    }
}
