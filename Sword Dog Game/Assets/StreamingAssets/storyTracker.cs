using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storyTracker : MonoBehaviour
{
    int goodRepCount, badRepCount = 0; //Reputation result for the chapter
    enum Chapters {PROLOGUE, FOREST, CAVES, DESERT, OCEAN, MOUNTAIN, EPILOGUE}; 
    public int currentChapter = 0; //What chapter the player is currently on.


}

/* This class keeps track of which story relevant dialogue/cutscenes have happened, and makes sure things trigger in the right order
 * Each chapter has a list of cutscene objects that will trigger when the player has the right criteria met
 * 
 */
