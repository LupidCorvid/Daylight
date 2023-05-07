using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetupQuest : Quest
{
    public GetupQuest()
    {
        questId = 0;
        questDescription = "Get up and save your friend!";
        rewardsDescription = "Rewards: Saving your friend";
    }
}

public partial class QuestDatabase
{
    public GetupQuest getupQuest = new GetupQuest();
}
