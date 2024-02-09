using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine.InputSystem;


public class SettingsManager : MonoBehaviour
{
    public static Settings currentSettings;
    public const string fileName = "Settings.txt";
    //public InputActionAsset currentActions;

    // Start is called before the first frame update
    void Awake()
    {
        //if (currentSettings == null)
        //    currentSettings = new Settings();

        if (File.Exists(Application.persistentDataPath + "/" + fileName))
            LoadSettings();
        else
        {
            currentSettings = new Settings();
            SaveSettings();
        }
    }

    public static void LoadSettings()
    {
        Settings newSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + fileName));
        currentSettings = newSettings;
        if(currentSettings.keybinds != "")
            InputReader.inputs.actions.LoadBindingOverridesFromJson(currentSettings.keybinds);

        //Screen.fullScreen = currentSettings.fullScreen;

        if (currentSettings.fullScreen)
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        QualitySettings.SetQualityLevel(currentSettings.quality);
        if (currentSettings.vSync)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public static void SaveSettings()
    {
        currentSettings.keybinds = InputReader.inputs.actions.SaveBindingOverridesAsJson();

        string path = Application.persistentDataPath + "/" + fileName;
        string settingsJSON = JsonUtility.ToJson(currentSettings, true);

        File.WriteAllText(path, settingsJSON);
        Debug.Log("Saved settings to: " + path); 
    }
}
