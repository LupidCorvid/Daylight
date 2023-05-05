using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public MenuBackgrounds Backgrounds;
    public string lastSave;

    public TMPro.TextMeshProUGUI lastSaveDetails;

    public UnityEngine.UI.Button newGameButton;


    // Start is called before the first frame update
    void Start()
    {
        lastSave = GetMostRecentSave();
        lastSaveDetails.text = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(lastSave)).player.spawnpoint.scene;
        CanvasManager.HideHUD();
        //if (SwordFollow.instance?.transform != null)
        //    SwordFollow.instance.transform.position = new Vector3(100, -100);

        //Destroy(FindObjectOfType<AudioListener>().gameObject);
        Destroy(PlayerMovement.controller?.gameObject);
        if (SwordFollow.instance != null)
            Destroy(SwordFollow.instance);
        if (PlayerMovement.instance != null)
            Destroy(PlayerMovement.instance);
        PauseScreen.canPause = false;
        //EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    public void Update()
    {
        if((Input.GetAxisRaw("Vertical") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        }
    }

    public void StartNewSave()
    {
        //SceneHelper.LoadScene("prologue area");
        ChangeScene.LoadScene("prologue area");
        //CanvasManager.ShowHUD();
        
    }

    public void LoadMostRecentSave()
    {
        //GameSaver.main.LoadGame();
        GameSaver.main.LoadGame();
    }

    public void Quit()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
    public void OnDestroy()
    {
        PauseScreen.canPause = true;
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