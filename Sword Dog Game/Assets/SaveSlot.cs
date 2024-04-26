using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveSlot : MonoBehaviour
{
    GameSaver.SaveData saveData;
    public string savePath = "";
    public int saveNum = 0;

    public TMPro.TextMeshProUGUI LastLocation;
    public TMPro.TextMeshProUGUI SaveName;
    public TMPro.TextMeshProUGUI LastModified;

    public CanvasGroup defaultGroup;
    public CanvasGroup ConfirmDeleteGroup;

    // Start is called before the first frame update
    void Start()
    {
        if (savePath == "")
            return;

        saveData = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(savePath));

        if (saveData.emptySave == false)
        {
            
            LastLocation.text = saveData.player.spawnpoint.scene;
            SaveName.text = "Save #" + (saveNum + 1);
            LastModified.text = File.GetLastWriteTime(savePath).ToString("d");
        }
        else
        {
            SetToEmptyVisuals();
        }

    }

    public void SetToEmptyVisuals()
    {
        SaveName.text = "New Game";
        LastLocation.text = "";
        LastModified.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSave()
    {
        SaveSystem.current.saveDataIndex = saveNum;

        if (saveData.emptySave)
            MainMenuManager.StartNewSave();
        else
        {
            MainMenuManager.LoadSave(saveData);
        }

        
    }

    public void PromptClearData()
    {
        ConfirmDeleteGroup.alpha = 1;
        ConfirmDeleteGroup.blocksRaycasts = true;
        ConfirmDeleteGroup.interactable = true;

        defaultGroup.alpha = 0;
        defaultGroup.blocksRaycasts = false;
        defaultGroup.interactable = false;
    }

    public void RestoreDefaultDisplay()
    {
        defaultGroup.alpha = 1;
        defaultGroup.blocksRaycasts = true;
        defaultGroup.interactable = true;

        ConfirmDeleteGroup.alpha = 0;
        ConfirmDeleteGroup.blocksRaycasts = false;
        ConfirmDeleteGroup.interactable = false;
    }

    public void ClearData()
    {
        GameSaver.SaveData newData = new GameSaver.SaveData();
        newData.emptySave = true;

        SaveSystem.SaveData(saveNum, JsonUtility.ToJson(newData, true)); //Disable prettyPrint to make file size much smaller

        saveData = newData;
        SetToEmptyVisuals();
    }

    public void ClearDataAndClosePrompt()
    {
        RestoreDefaultDisplay();
        ClearData();
        MainMenuManager.main.FetchLastSave();
    }

    
}
