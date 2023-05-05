using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomStates
{
    public PrologueAreaState prologueState;

    public RoomStates()
    {
        prologueState = new PrologueAreaState();
    }
}
