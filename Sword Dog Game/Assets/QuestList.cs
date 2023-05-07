using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestList
{
    public List<SaveableQuest> quests = new List<SaveableQuest>();

    public void AddQuest(SaveableQuest quest)
    {
        quests.Add(quest);
    }

    public QuestList()
    {
        quests = new List<SaveableQuest>();
    }

    public QuestList(List<SaveableQuest> quests)
    {
        this.quests = quests;
    }

}
