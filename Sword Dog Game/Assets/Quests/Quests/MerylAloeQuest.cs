using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerylAloeQuest : Quest
{
    
    public MerylAloeQuest()
    {
        questDescription = "Get Meryl some Teardrop Aloe";
        rewardsDescription = "Reward: 10 Bones";
        type = QuestType.side;
        assigner = 1;
        questId = 1;
        progress = 0;
        neededProgress = 2;
    }

    public override void complete()
    {
        
    }
}

public partial class QuestDatabase
{
    MerylAloeQuest merylQuest = new MerylAloeQuest();
}
