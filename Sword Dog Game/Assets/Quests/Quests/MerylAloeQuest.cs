using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerylAloeQuest : Quest
{
    
    public MerylAloeQuest()
    {
        questDescription = "Get Meryl some Teardrop Aloe";
        rewardsDescription = "Reward: 20 Bones";
        type = QuestType.side;
        assigner = 1;
        questId = 1;
    }
}

public partial class QuestDatabase
{
    MerylAloeQuest merylQuest = new MerylAloeQuest();
}
