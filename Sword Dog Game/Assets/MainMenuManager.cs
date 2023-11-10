using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public MenuBackgrounds Backgrounds;
    public string lastSave;

    public TMPro.TextMeshProUGUI lastSaveDetails;

    public List<UnityEngine.UI.Button> buttons;

    public static bool inMainMenu;
    private bool quit = false;


    // Start is called before the first frame update
    void Start()
    {
        inMainMenu = true;

        lastSave = GetMostRecentSave();
        if (lastSave != "")
        {
            lastSaveDetails.text = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(lastSave)).player.spawnpoint.scene;
        }
        else
        {
            Debug.Log("No most recent save found");
            lastSaveDetails.text = "";
        }

        CanvasManager.InstantHideHUD();

        BuffList.main?.clearBuffIcons();
        Buff.totalBuffs = 0;
        //if (SwordFollow.instance?.transform != null)
        //    SwordFollow.instance.transform.position = new Vector3(100, -100);

        //Destroy(FindObjectOfType<AudioListener>().gameObject);
        Destroy(PlayerMovement.controller?.gameObject);
        if (SwordFollow.instance != null)
            Destroy(SwordFollow.instance);
        if (PlayerMovement.instance != null)
            Destroy(PlayerMovement.instance);
        PauseScreen.canPause = false;

        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);

        Invoke("ActivateButtons", 0.5f);
    }

    public void ActivateButtons()
    {
        // Conditionally activate continue game button if a save exists
        buttons[1].interactable = lastSave != "";

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != 1) buttons[i].interactable = true;
        }
    }

    public void Update()
    {
        // Select new game button if nothing else is selected
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }
    }

    public void StartNewSave()
    {
        if (!ChangeScene.changingScene && !GameSaver.loading)
        {
            //SceneHelper.LoadScene("Prologue Area");
            GameSaver.currData = new GameSaver.SaveData();
            //InventoryManager.currInventory.contents = new List<ItemSlot>(3);
            InventoryManager.currInventory = new Inventory();
            QuestsManager.main?.questsDatabase.ResetAllQuestProgress();
            InventoryManager.main?.refreshInventory();
            AudioManager.instance.FadeOutCurrent();
            DialogSource.stringVariables = GameSaver.currData.dialogStringVariables;
            ChangeScene.LoadScene("Prologue Area", "", false);
            //CanvasManager.ShowHUD();
        }
    }

    public void LoadMostRecentSave()
    {
        if (!ChangeScene.changingScene && !GameSaver.loading)
        {
            //GameSaver.main.LoadGame();
            AudioManager.instance.FadeOutCurrent();
            GameSaver.main.LoadGame();
        }
        
    }

    public void Quit()
    {
        quit = true;
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
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
        if (!quit)
        {
            PauseScreen.canPause = true;
            //Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
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