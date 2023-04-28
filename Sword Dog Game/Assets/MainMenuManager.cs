using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class MainMenuManager : MonoBehaviour
{
    public MenuBackgrounds Backgrounds;
    public string lastSave;

    public TMPro.TextMeshProUGUI lastSaveDetails;


    // Start is called before the first frame update
    void Start()
    {
        lastSave = GetMostRecentSave();
        lastSaveDetails.text = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(lastSave)).player.spawnpoint.scene;
        CanvasManager.HideHUD();
    }

    public void StartNewSave()
    {
        SceneHelper.LoadScene("prologue area");
        
    }

    public void LoadMostRecentSave()
    {
        //GameSaver.main.LoadGame();
        GameSaver.main.LoadGame();
    }

    public string GetMostRecentSave()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath + @"\SaveData");

        string mostRecentFile = "";
        System.DateTime mostRecentDate = System.DateTime.Now;
        foreach(string file in files)
        {
            if(File.GetLastWriteTime(file) < System.DateTime.Now)
            {
                mostRecentDate = File.GetLastWriteTime(file);
                mostRecentFile = file;
            }
        }
        Debug.Log("most recent save: " + mostRecentFile + " accessed on " + mostRecentDate.ToString());

        return mostRecentFile;
    }


}

[System.Serializable]
public class MenuBackgrounds
{
    public GameObject Forest;
    public GameObject Fields;
    public GameObject Caves;
    public GameObject Town;
    public GameObject Desert;
}