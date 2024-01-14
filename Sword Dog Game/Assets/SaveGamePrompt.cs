using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGamePrompt : MonoBehaviour
{
    public static SaveGamePrompt main;
    public SavePoint openedBy;
    public CanvasGroup promptGroup;

    // Start is called before the first frame update
    void Start()
    {
        main ??= this;
    }

    public void openDialog(SavePoint by)
    {
        openedBy = by;

        promptGroup.alpha = 1;
        promptGroup.interactable = true;
        promptGroup.blocksRaycasts = true;

    }

    public void SaveGame()
    {
        Close();
        Player.instance.GetComponent<Spawnpoint>().SetSpawnpoint(openedBy.transform);
        GameSaver.main.SaveGame();
    }

    public void SaveAndQuit()
    {
        Close();
        SaveGame();
        PauseScreen.main.QuitToTitle();
        CutsceneController.StopAllCutscenes?.Invoke();

    }

    public void Close()
    {
        promptGroup.alpha = 0;
        promptGroup.interactable = false;
        promptGroup.blocksRaycasts = false;
        openedBy.FinishedSavePrompt();
    }
}
