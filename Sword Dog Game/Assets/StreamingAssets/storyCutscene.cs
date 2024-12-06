using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storyCutscene : MonoBehaviour
{
    enum CutsceneTypes {DIALOGUE, IN_GAME, ANIMATED}; //Dialogue: Required dialogue that the player cannot skip or walk away from
                                                      //In-game: Animated using in-game assets. Player cannot move
                                                      //Animated: Full raster animation/video
    public int cutsceneType = 0;
    bool hasTriggered = false; //If the cutscene has been instantiated yet
    bool hasFinished = false; //If the cutscene has finished. Player cannot go back to it.
    bool isPlaying = false; //If the cutscene is currently playing
}
/* This class is an object for story relevant dialogue and cutscenes
*/
