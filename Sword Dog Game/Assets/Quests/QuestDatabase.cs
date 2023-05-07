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
                }
            }
        }
    }

    public List<SaveableQuest> packQuests()
    {
        List<SaveableQuest> output = new List<SaveableQuest>();

        foreach(Quest quest in allQuests)
        {
            if(quest.assigned)
            {
                output.Add(new SaveableQuest(quest));
            }
        }

        return output;
    }

    public void AddQuest(int id)
    {
        foreach(Quest quest in allQuests)
        {
            if(quest.questId == id)
            {
                quest.assigned = true;
                return;
            }
        }
    }

    public void AddQuest(Quest quest)
    {
        AddQuest(quest.questId);
    }
}
