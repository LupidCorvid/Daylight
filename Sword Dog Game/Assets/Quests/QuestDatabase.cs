using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public partial class QuestDatabase
{


    public List<Quest> allQuests = new List<Quest>();


    public QuestDatabase()
    {
        populateQuestsList();
    }

    public void populateQuestsList()
    {
        allQuests.Clear();

        FieldInfo[] fields = typeof(QuestDatabase).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            Quest currQuest = field.GetValue(this) as Quest;
            if (currQuest != null)
            {
                //currQuest.questId = allQuests.Count;//Partial classes might not be the best thing to use this with
                allQuests.Add(currQuest);
            }

        }
    }

    public void unpackSavedQuests(GameSaver.SaveData data)
    {
        foreach (Quest quest in allQuests)
        {
            foreach (SaveableQuest packedQuest in data.quests.quests)
            {
                if (quest.questId == packedQuest.questId)
                {
                    quest.assigned = true;
                    quest.progress = packedQuest.progress;
                    quest.completed = packedQuest.completed;
                }
            }
        }
    }

    public QuestList packQuests()
    {
        List<SaveableQuest> output = new List<SaveableQuest>();

        foreach(Quest quest in allQuests)
        {
            if(quest.assigned)
            {
                output.Add(new SaveableQuest(quest));
            }
        }

        return new QuestList(output);
    }

    public void AddQuest(int id)
    {
        foreach(Quest quest in allQuests)
        {
            if(quest.questId == id)
            {
                quest.assigned = true;
                quest.onAssign();
                return;
            }
        }
    }

    public void AddQuest(Quest quest)
    {
        AddQuest(quest.questId);
    }

    public Quest findQuest(int id)
    {
        return allQuests.Find((e) => e.questId == id);
    }

    public bool getIfAwaitingCompletion(int id)
    {
        Quest foundQuest = findQuest(id);
        return (foundQuest.progress >= foundQuest.neededProgress && !foundQuest.completed);
    }

    public bool checkIfCompleted(int id)
    {
        return findQuest(id).completed;
    }

    public bool checkIfCompleted(Quest quest)
    {
        return checkIfCompleted(quest.questId);
    }

    public bool checkIfAssigned(int id)
    {
        return findQuest(id).assigned;
    }

    public bool checkIfAssigned(Quest quest)
    {
        return checkIfAssigned(quest.questId);
    }

    public void ResetAllQuestProgress()
    {
        foreach(Quest quest in allQuests)
        {
            quest.assigned = false;
            quest.progress = 0;
            quest.completed = false;
        }
    }

    public void setQuestCompletion(int id, bool completion)
    {
        findQuest(id).completed = completion;
    }

    public void setQuestCompletion(Quest quest, bool completion)
    {
        findQuest(quest.questId).completed = completion;
    }

}
