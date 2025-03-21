using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomStates
{
    public PrologueAreaState prologueState;
    public TownAreaState townState;
    public ForestAreaState forest1State;

    public RoomStates()
    {
        prologueState = new PrologueAreaState();
        townState = new TownAreaState();
        forest1State = new ForestAreaState();
    }
}
