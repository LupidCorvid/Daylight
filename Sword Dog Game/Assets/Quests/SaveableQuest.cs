using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveableQuest
{
    public int questId;
    public float progress;
    public bool completed;

    public SaveableQuest(Quest from)
    {
        questId = from.questId;
        progress = from.progress;
        completed = from.completed;
    }

}
