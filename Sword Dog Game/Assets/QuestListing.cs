using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestListing : MonoBehaviour
{
    public TextMeshProUGUI questDesc;
    public TextMeshProUGUI questReward;
    public Image questType;
    public Image assigner;

    public Sprite mainQuestIcon;
    public Sprite sideQuestIcon;

    public Quest quest;

    public void setQuest(Quest quest)
    {
        this.quest = quest;
        questDesc.text = quest.questDescription;
        questReward.text = quest.rewardsDescription;
        
        switch(quest.type)
        {
            case Quest.QuestType.side:
                questType.sprite = sideQuestIcon;
                break;
            case Quest.QuestType.story:
                questType.sprite = mainQuestIcon;
                break;
        }
    }
}
