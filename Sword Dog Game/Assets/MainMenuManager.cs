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
    public int lastSaveNum;

    public TMPro.TextMeshProUGUI lastSaveDetails;

    public List<UnityEngine.UI.Button> buttons;

    public static bool inMainMenu;
    private bool quit = false;

    public GameObject SaveSlotUIPrefab;
    public GameObject savesList;

    public CanvasGroup loadMenu;

    public TMPro.TextMeshProUGUI continueText;

    // Start is called before the first frame update
    void Start()
    {
        inMainMenu = true;

        (string ,int) lastSaveData = GetMostRecentSave();
        lastSave = lastSaveData.Item1;
        lastSaveNum = lastSaveData.Item2;
        ResetPlayerBools();

        
        if (lastSave != "")
        {
            lastSaveDetails.text = JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(lastSave)).player.spawnpoint.scene;
            buttons[1].interactable = true;
            continueText.text = "Continue";
        }
        else
        {
            //Debug.Log("No most recent save found");
            lastSaveDetails.text = "";
            continueText.text = "New Game";
            //buttons[1].interactable = false;

            //ColorBlock continueButtonColors = buttons[1].colors;
            //continueButtonColors.disabledColor = new Color(0,0,0,0.6f);
            //buttons[1].colors = continueButtonColors;
        }

        CanvasManager.InstantHideHUD();

        BuffList.main?.clearBuffIcons();
        Buff.totalBuffs = 0;
        //if (SwordFollow.instance?.transform != null)
        //    SwordFollow.instance.transform.position = new Vector3(100, -100);

        //Destroy(FindObjectOfType<AudioListener>().gameObject);
        Destroy(Player.instance?.gameObject);
        Destroy(SwordFollow.instance?.gameObject);
        PauseScreen.canPause = false;

        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);

        Invoke("ActivateButtons", 0.4f);
        PopulateLoadMenu();
    }

    public void ResetPlayerBools()
    {
        //!PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open
        //&& DialogController.main.source?.waiting != true
        PlayerHealth.dead = false;
        CutsceneController.cutsceneStopMovement = false;
        if (DialogController.main?.source != null)
            DialogController.main.source.waiting = false;
    }

    public void ActivateButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != 1)
            {
                buttons[i].interactable = true;

                // Set normal button disabled colors
                // Necessary because we set them to all black and/or selected color at first to disguise disabling for user
                ColorBlock buttonColors = buttons[i].colors;
                buttonColors.disabledColor = new Color(0, 0, 0, 0.6f);
                buttons[i].colors = buttonColors;
            }
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

    public static void StartNewSave()
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

    public void PopulateLoadMenu()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath + @"\SaveData");

        for(int i = 0; i < files.Length; i++)
        {
            GameObject addedSave = Instantiate(SaveSlotUIPrefab, savesList.transform);
            SaveSlot addedSlot = addedSave.GetComponent<SaveSlot>();
            addedSlot.saveNum = i;
            addedSlot.savePath = files[i];

        }
    }

    public void LoadMostRecentSave()
    {
        if(lastSaveNum == -1)
        {
            SaveSystem.current.saveDataIndex = 0;
            StartNewSave();
        }

        if (!ChangeScene.changingScene && !GameSaver.loading && lastSaveNum != -1)
        {
            //GameSaver.main.LoadGame();
            SaveSystem.current.saveDataIndex = lastSaveNum;
            AudioManager.instance.FadeOutCurrent();
            GameSaver.main.LoadGame();
        }
        
    }

    public static void LoadSave(GameSaver.SaveData data)
    {
        if (!ChangeScene.changingScene && !GameSaver.loading)
        {
            GameSaver.currData = data; //Isn't needed?
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

    public (string, int) GetMostRecentSave()
    {
        if (!Directory.Exists(Application.persistentDataPath + @"\SaveData"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + @"\SaveData");
        }
        string[] files = Directory.GetFiles(Application.persistentDataPath + @"\SaveData");

        string mostRecentFile = "";
        int mostRecentId = -1;
        System.DateTime mostRecentDate = System.DateTime.MinValue;
        for(int i = 0; i < files.Length; i++)
        {
            if(File.GetLastWriteTime(files[i]) > mostRecentDate)
            {
                if (JsonUtility.FromJson<GameSaver.SaveData>(File.ReadAllText(files[i])).emptySave)
                    continue;

                mostRecentDate = File.GetLastWriteTime(files[i]);
                mostRecentFile = files[i];
                mostRecentId = i;
                
            }
        }
        Debug.Log("most recent save: " + mostRecentFile + " accessed on " + mostRecentDate.ToString());

        return (mostRecentFile, mostRecentId);
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

    public void LoadMenuToggle()
    {
        if(loadMenu.blocksRaycasts)
        {
            loadMenu.blocksRaycasts = false;
            loadMenu.alpha = 0;
            loadMenu.interactable = false;
        }
        else
        {
            loadMenu.blocksRaycasts = true;
            loadMenu.alpha = 1;
            loadMenu.interactable = true;
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