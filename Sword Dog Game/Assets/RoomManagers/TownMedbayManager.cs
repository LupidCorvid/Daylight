using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMedbayManager : MonoBehaviour
{
    public MerylBehavior meryl;

    // Start is called before the first frame update
    void Start()
    {
        if (!QuestsManager.main.getQuest(1).assigned && !QuestsManager.main.getQuest(1).completed)
            meryl.dialog[0] = "[lf,Meryl.txt,Prologue5.1]";
    }
}
