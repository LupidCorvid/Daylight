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


    // Start is called before the first frame update
    void Start()
    {
        if (savePath == "")
            return;

        saveData = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(savePath));

        if (saveData.emptySave == false)
        {
            
            LastLocation.text = saveData.player.spawnpoint.scene;
            SaveName.text = "Save #" + saveNum;
            LastModified.text = File.GetLastWriteTime(savePath).ToString("d");
        }
        else
        {
            //Change to say new game
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSave()
    {


    }

    public void ClearData()
    {
        GameSaver.SaveData newData = new GameSaver.SaveData();
        newData.emptySave = true;

        SaveSystem.SaveData(saveNum, JsonUtility.ToJson(newData, true)); //Disable prettyPrint to make file size much smaller
    }
}
