using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrologueAreaState
{
    public bool swordCollected = false;

    //Maybe should be a number for progress in it? Could also just prevent leaving until it is done
    public bool finishedIntroCutscene = false;

    public PrologueAreaState()
    {

    }
}
