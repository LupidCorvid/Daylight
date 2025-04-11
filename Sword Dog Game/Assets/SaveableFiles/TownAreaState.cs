using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//All town scenes will be managed by one class
public class TownAreaState
{
    //Essentially, put all story beats that need to be saved to file as variables here. This guides the direction of the story.
    public bool P_FirstTimeEnter_triggered = false;
    /* Protag and Ricken walk in.
    * After a slight bit, the textbox appears and "woohoo we made it!"
    * Ricken turns around
    * Protag questioning emote
    * Rest of the yammering
    * Ricken trots off screen quickly
    */

    public bool P_TownPan = false;
        //Pan to three areas of the town, then return control to the character

    public bool P_GeneralFirstTimeEnter = false;
        /* The player walks into town a bit but the General is turned away from the player
         * When the player walks up, the cutscene starts
         * General emotes !
         * General turns around and walks towards player
         * General yammering
        */
}
