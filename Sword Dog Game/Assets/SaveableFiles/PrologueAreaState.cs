using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Cannot put gameobjects in here
//If you can't convert it into a string, it can't be put here (it's going in a JSON file)
public class PrologueAreaState
{
    public bool swordCollected = false;

    //Maybe should be a number for progress in it? Could also just prevent leaving until it is done
    public bool finishedIntroCutscene = false;

    public bool prologueMonsterKilled = false;

    public PrologueAreaState()
    {

    }
}
