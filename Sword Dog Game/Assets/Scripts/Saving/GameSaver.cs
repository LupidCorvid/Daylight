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

    public void Awake()
    {
        main = this;
    }

    public void Clear()
    {
        Destroy(PlayerMovement.instance);
        PlayerMovement.instance = null;
        
    }

    public void SaveGame()
    {
        if (!PlayerHealth.dead && !PlayerMovement.controller.resetting && !loading) {
            SaveData data = new SaveData();
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
        AudioListener currentAudioListener = GameObject.FindObjectOfType<AudioListener>();

        if (!String.IsNullOrEmpty(dataToLoad))
        {
            // TODO replace all of this with cutscene
            AudioManager.instance.FadeOutCurrent();
            // GameObject.FindObjectOfType<AudioListener>().enabled = false;
            GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
            yield return new WaitForSeconds(0.9f);
            Clear();
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);

            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                GameObject.Destroy(eventSystem.gameObject);
            }
            SceneHelper.LoadScene(data.player.spawnpoint.scene);
            currentAudioListener.enabled = false;

            var newPlayer = Instantiate(prefab);
            newPlayer.GetComponent<Rigidbody2D>().simulated = false;
            newPlayer.transform.position = data.player.spawnpoint.position;

            // data transfers
            data.player.controller.SetValues(newPlayer);
            // data.player.inventory.SetValues(newPlayer);
            // data.player.health.SetValues(newPlayer);
            // data.player.attack.SetValues(newPlayer);
            data.player.spawnpoint.SetValues(newPlayer);
            data.options.SetValues();
            
            PlayerMovement.instance = newPlayer;
            PlayerMovement.controller = newPlayer.GetComponent<PlayerMovement>();
            GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("stop");
            CanvasManager.ShowHUD();
        }

        loading = false;
    }

    [Serializable]
    public class SaveData
    {
        public PlayerSerialization player;
        public OptionsSerialization options;

        public void SetPlayer(GameObject playerObj) {
            player = new PlayerSerialization(playerObj);
        }
        public void SetOptions(AudioManager am) {
            options = new OptionsSerialization(am);
        }
    }

    // Saves game if player exists on application quit
    private void OnApplicationQuit() {
        if (PlayerMovement.instance != null) {
            SaveData data = new SaveData();
            data.SetPlayer(PlayerMovement.instance);
            data.SetOptions(AudioManager.instance);
            var dataToSave = JsonUtility.ToJson(data);
            saveSystem.SaveData(dataToSave);
        }
    }
}