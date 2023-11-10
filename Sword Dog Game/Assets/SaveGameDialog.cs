using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameDialog : DialogNPC
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void eventCalled(params string[] input)
    {
        base.eventCalled(input);
        if (input.Length == 0)
            return;
        if (input[0] == "SaveGame")
            SaveGame();
        if (input[0] == "SaveQuit")
            SaveAndQuit();

        
    }

    public void SaveGame()
    {
        PlayerMovement.instance.GetComponent<Spawnpoint>().SetSpawnpoint(transform);
        GameSaver.main.SaveGame();
    }

    public void SaveAndQuit()
    {
        exitDialog();
        SaveGame();
        PauseScreen.main.QuitToTitle();
        CutsceneController.StopAllCutscenes?.Invoke();

    }


}
