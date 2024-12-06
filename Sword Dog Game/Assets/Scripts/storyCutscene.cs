using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class storyCutscene : MonoBehaviour
{
    enum CutsceneTypes {DIALOGUE, IN_GAME, ANIMATED}; //Dialogue: Required dialogue that the player cannot skip or walk away from
                                                      //In-game: Animated using in-game assets. Player cannot move
                                                      //Animated: Full raster animation/video
    public string cutsceneName = "";
    public int cutsceneType = 0;
    bool hasTriggered = false; //If the cutscene has been instantiated yet
    bool hasFinished = false; //If the cutscene has finished. Player cannot go back to it.
    bool isPlaying = false; //If the cutscene is currently playing

    /*Some sort of cutscene contents variable?
     * Dialogue just has the letter + number
     * In-game would have some sort of animation and camera movement. Code is required. So this file might get long 
     *      (or it can trigger subclasses. Inheretance moment?)
     * Animated will have their own Scenes, so swap scenes
    */
}
/* This class is an object for story relevant dialogue and cutscenes.
 * Empty gameobjects for each cutscene will be in the scene, and when relevant criteria is made, the cutscene will trigger
 * Criteria to Trigger a cutscene:
 *      Talk to an NPC
 *      Walk in the vicinity of an area/NPC
 *      Kill an enemy/boss
 *      Inspect something of importance
 *      Finish/start a chapter
*/
