using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomStates
{
    public PrologueAreaState prologueState;
    public TownAreaState townState;
    public TownAreaState medbayState;
    public TownAreaState dojoState;
    public ForestAreaState forest1State;

    public RoomStates()
    {
        prologueState = new PrologueAreaState();
    }
}
