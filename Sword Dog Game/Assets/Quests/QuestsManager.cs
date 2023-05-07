using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    public GameObject questsHolder;
    public GameObject questListingPrefab;
    public QuestDatabase questsDatabase = new QuestDatabase();

    public void addQuest()
    {

    }

    public void Start()
    {
        Debug.Log(questsDatabase.allQuests.Count);
    }


}
