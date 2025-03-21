using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : RoomManager
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

    public override void receivedEvent(string name, params object[] parameters)
    {

        Debug.Log("Event called: " + name);
        //Only do these if calling from room event object that calls the cutscene
        switch (name)
        {
            //The VERY first time the player enters
            //The General talks to the player
            case "P_FirstTimeEnter":
                if (!DialogController.dialogOpen && roomState.P_FirstTimeEnter_triggered == false)
                {
                    CutsceneController.PlayCutscene("P_FirstTimeEnter");
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
        }
    }
}

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
