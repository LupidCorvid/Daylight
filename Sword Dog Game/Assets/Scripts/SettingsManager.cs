using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;

public class SettingsManager : MonoBehaviour
{
    public static Settings currentSettings;
    public const string fileName = "Settings.txt";


    // Start is called before the first frame update
    void Start()
    {

        if (currentSettings == null)
            currentSettings = new Settings();

        if (File.Exists(Application.persistentDataPath + "/" + fileName))
            LoadSettings();
        else
        {
            currentSettings = new Settings();
            SaveSettings();
        }
    }

    public void LoadSettings()
    {
        Settings newSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + fileName));
        currentSettings = newSettings;
    }

    public void SaveSettings()
    {
        string path = Application.persistentDataPath + "/" + fileName;
        string settingsJSON = JsonUtility.ToJson(currentSettings, true);

        File.WriteAllText(path, settingsJSON);
            
    }
}
