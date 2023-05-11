using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    public GameObject questsHolder;
    public GameObject questListingPrefab;
    public QuestDatabase questsDatabase = new QuestDatabase();
    public List<QuestListing> questListings = new List<QuestListing>();

    public static QuestsManager main;

    public void Awake()
    {
        if (main == null || main.gameObject == null)
            main = this;
    }

    public void CreateListing(Quest quest)
    {
        GameObject addedObject= Instantiate(questListingPrefab, questsHolder.transform);
        QuestListing addedListing = addedObject.GetComponent<QuestListing>();
        addedListing.setQuest(quest);
        questListings.Add(addedListing);
    }
    public void removeListing(Quest quest)
    {
        removeListing(quest.questId);
    }

    public void removeListing(int id)
    {
        for (int i = 0; i < questListings.Count; i++)
        {
            if (questListings[i].quest.questId == id)
            {
                Destroy(questListings[i].gameObject);
                questListings.RemoveAt(i);
                return;
            }
        }
    }

    public void AssignQuest(int id)
    {
        questsDatabase.AddQuest(id);
        CreateListing(questsDatabase.findQuest(id));
    }

    public void AssignQuest(Quest quest)
    {
        questsDatabase.AddQuest(quest);
        CreateListing(questsDatabase.findQuest(quest.questId));
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
        Quest gottenQuest = getQuest(id);
        gottenQuest.progress = newProgress;
        if (gottenQuest.progress >= gottenQuest.neededProgress)
            gottenQuest.complete();
        if (checkIfCompleted(id))
            removeListing(id);

    }

    public void setQuestProgress(Quest quest, float newProgress)
    {
        setQuestProgress(quest.questId, newProgress);
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
        //Debug.Log(questsDatabase.allQuests.Count);
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
