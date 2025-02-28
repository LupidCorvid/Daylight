using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: removing monobehavior means it can't be a compnent in the inspector. Make sure that's what you want
//Look into either making this a json or hardwriting a file format

public class storyTracker
{
    int goodRepCount, badRepCount = 0; //Reputation result for the chapter
    enum Chapters {PROLOGUE, FOREST, CAVES, DESERT, OCEAN, MOUNTAIN, EPILOGUE}; 
    public int currentChapter = 0; //What chapter the player is currently on.

    storyCutscene[] prologueScenes, forestScenes, caveScenes, desertScenes, oceanScenes, mountainScenes, epilogueScenes;

    //Initializes everything when a new game is made
    //Make new files for list of cutscenes
    //Load in prologue
    void NewGame()
    {
        //Make the 6 cutscene files
        //Fill the cutscene files with all possible cutscenes and set triggered to false for all of them
        //LoadCutscenes(PROLOGUE)
        //If the player leaves without saving the new game, delete all files

    }

    //Save new flags/cutscene changes to file
    void SaveGame()
    {
        //Check current cutscene and open relevant file
        //Overwrite file with current array contents
    }

    //When the game is booted up, or a chapter changes, load the current chapter's cutscenes
    //Free old cutscenes if swapping chapters
    //chapter: what list of cutscenes to load
    void LoadCutscenes(Chapters chapter)
    {
        //Check if booting or changing chapters

        //Booting
            //Load cutscenes from file

        //Changing chapters
            //Ask the player if they want to save the game
            //Free old cutscenes
            //Load new cutscenes
            //Save changes if player wanted to save
    }
}

/* This class keeps track of which story relevant dialogue/cutscenes have happened, and makes sure things trigger in the right order.
 * Each chapter has a list of cutscene objects that will trigger when the player has the right criteria met.
 * 
 * The status of every cutscene should be saved to file. Either make new files or update the one save data made
 */
