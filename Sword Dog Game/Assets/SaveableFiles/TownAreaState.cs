using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//All town scenes will be managed by one class
public class TownAreaState
{
    //Essentially, put all story beats that need to be saved to file as variables here. This guides the direction of the story.
    public bool P_FirstTimeEnter_triggered = true;
    public bool P_TownPan = false;
}
