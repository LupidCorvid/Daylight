using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSaver : MonoBehaviour
{
    public SaveSystem saveSystem;
    public GameObject prefab;
    public static bool loading = false;
    public static GameSaver main;
    public static PlayerSerialization player;

    public static SaveData currData = new SaveData();

    public static Action StartingSave;
    public static Action<SaveData> loadedNewData;
    public static Action updateDataToSave;

    public void Awake()
    {
        main = this;
        DontDestroyOnLoad(this);
    }

    public void Clear()
    {
        Destroy(PlayerMovement.instance);
        PlayerMovement.instance = null;
    }

    public void SaveGame()
    {
        if (!PlayerHealth.dead && !PlayerMovement.controller.resetting && !loading) {
            //SaveData data = new SaveData();
            StartingSave?.Invoke();
            currData.inventory = InventoryManager.currInventory;
            SaveData data = currData;
            data.SetPlayer(PlayerMovement.instance);
            data.SetOptions(AudioManager.instance);
            var dataToSave = JsonUtility.ToJson(data, true);
            saveSystem.SaveData(dataToSave);
        }
    }


    public void LoadGame()
    {
        if (!loading) { // load game can only happen from menu
            if (PlayerMovement.controller == null) {
                StartCoroutine(LoadSaveFile());
            } else if (!PlayerMovement.controller.resetting && !PlayerHealth.dead) {
                StartCoroutine(LoadSaveFile());
            }
        }
            
    }

    IEnumerator LoadSaveFile()
    {
        loading = true;
        string dataToLoad = "";
        dataToLoad = saveSystem.LoadData();

        if (!String.IsNullOrEmpty(dataToLoad))
        {
            AudioManager.instance.FadeOutCurrent();
            Crossfade.current.StartFade();
            yield return new WaitForSeconds(1f);
            Clear();
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);
            player = data.player;
            currData = data;
            
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            GameObject.Destroy(eventSystem?.gameObject);

            SceneHelper.LoadScene(data.player.spawnpoint.scene);

            // TODO this needs to happen upon application start!!!
            data.options.SetValues();
            loadedNewData?.Invoke(data);

            Crossfade.current.StopFade();

            CanvasManager.InstantShowHUD();
        }
        else 
            loading = false;
    }

    [Serializable]
    public class SaveData
    {
        public PlayerSerialization player;
        public OptionsSerialization options;
        public RoomStates roomStates;
        public Buffs.SaveBuffs buffs;
        public Inventory inventory;

        public void SetPlayer(GameObject playerObj) {
            player = new PlayerSerialization(playerObj);
        }
        public void SetOptions(AudioManager am) {
            options = new OptionsSerialization(am);
        }

        public SaveData()
        {
            roomStates = new RoomStates();

        }
    }

    // Saves game if player exists on application quit
    private void OnApplicationQuit() {
        //if (PlayerMovement.instance != null) {
        //    SaveData data = new SaveData();
        //    data.SetPlayer(PlayerMovement.instance);
        //    data.SetOptions(AudioManager.instance);
        //    var dataToSave = JsonUtility.ToJson(data);
        //    saveSystem.SaveData(dataToSave);
        //}
    }
}