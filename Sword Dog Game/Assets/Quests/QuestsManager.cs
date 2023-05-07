using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    public GameObject questsHolder;
    public GameObject questListingPrefab;
    public QuestDatabase questsDatabase = new QuestDatabase();

    public static QuestsManager main;

    public void Awake()
    {
        if (main == null || main.gameObject == null)
            main = this;
    }

    public void AssignQuest(int id)
    {
        questsDatabase.AddQuest(id);
    }

    public void AssignQuest(Quest quest)
    {
        questsDatabase.AddQuest(quest);
    }

    public Quest getQuest(int id)
    {
        return questsDatabase.findQuest(id);
    }

    public Quest getQuest(Quest quest)
    {
        return questsDatabase.findQuest(quest.questId);
    }

    public void setQuestProgress(int id, float newProgress)
    {
        getQuest(id).progress = newProgress;
    }

    public void setQuestProgress(Quest quest, float newProgress)
    {
        getQuest(quest).progress = newProgress;
    }

    public bool checkIfCompleted(Quest quest)
    {
        return questsDatabase.checkIfCompleted(quest);
    }

    public bool checkIfCompleted(int id)
    {
        return questsDatabase.checkIfCompleted(id);
    }

    public void Start()
    {
        Debug.Log(questsDatabase.allQuests.Count);
    }

    public bool checkIfAssigned(int id)
    {
        return questsDatabase.checkIfAssigned(id);
    }

    public bool checkIfAssigned(Quest quest)
    {
        return checkIfAssigned(quest.questId);
    }


}
