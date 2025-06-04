using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : RoomManager
{
    public TownAreaState roomState;
    public SwitchMusicOnLoad musicOnLoad;

    public Collider2D FirstEnterCutsceneCameraBounds;

    public override void Awake() //Called immediately when the object is loaded into the scene
    {
        base.Awake();
        roomState = GameSaver.currData.roomStates.townState; //Reads in the save data (copy by reference)
    }

    public void Start() //Called on frame 1
    {
        if (roomState.P_FirstTimeEnter_triggered)
        {
            Debug.Log("fhfjh");
            musicOnLoad.enabled = true;
        }
        buildRoom();
    }

    //Read in variables from file, things that might change throughout the game
    public void buildRoom()
    {
        if (roomState.P_FirstTimeEnter_triggered)
            FirstEnterCutsceneCameraBounds.enabled = false;
    }

    public override void receivedEvent(string name, params object[] parameters)
    {

        //Debug.Log("Event called: " + name);
        //Only do these if calling from room event object that calls the cutscene
        switch (name)
        {
            //The VERY first time the player enters
            //Ricken talks to the player
            case "P_FirstTimeEnter":
                if (!DialogController.dialogOpen && roomState.P_FirstTimeEnter_triggered == false)
                {
                    CutsceneController.PlayCutscene("P_FirstTimeEnter");
                    roomState.P_FirstTimeEnter_triggered = true;
                }
                break;
            //There is a box collider in the scene that will check this case if the player triggers it. 
            case "P_TownPan":
                if (roomState.P_TownPan == false && roomState.P_FirstTimeEnter_triggered == true)
                {
                    CutsceneController.PlayCutscene("P_TownPan");
                    roomState.P_TownPan = true;
                }
                break;
            case "P_GeneralFirstTimeEnter":
                if (roomState.P_TownPan == true && roomState.P_GeneralFirstTimeEnter == false)
                {
                    CutsceneController.PlayCutscene("P_GeneralFirstTimeEnter");
                    roomState.P_GeneralFirstTimeEnter = true;
                    
                }
                break;
            case "P_SpeakToMeryl":
                if(roomState.P_SpeakToMeryl == false && roomState.P_GeneralFirstTimeEnter == true)
                {
                    CutsceneController.PlayCutscene("P_SpeakToMeryl");
                    roomState.P_SpeakToMeryl = true;
                }
                break;
        }
    }
}

//Probably not needed, delete later 
/*
public class DojoManager : RoomManager
{
    public TownAreaState roomState;

    public override void Awake() //Called immediately when the object is loaded into the scene
    {
        base.Awake();
        roomState = GameSaver.currData.roomStates.townState; //Reads in the save data (copy by reference)
    }

    public void Start() //Called on frame 1
    {
        buildRoom();
    }

    //Read in variables from file, things that might change throughout the game
    public void buildRoom()
    {
    }
}

public class MedbayManager : RoomManager
{
    public TownAreaState roomState;

    public override void Awake() //Called immediately when the object is loaded into the scene
    {
        base.Awake();
        roomState = GameSaver.currData.roomStates.townState; //Reads in the save data (copy by reference)
    }

    public void Start() //Called on frame 1
    {
        buildRoom();
    }

    //Read in variables from file, things that might change throughout the game
    public void buildRoom()
    {
    }
}
*/
