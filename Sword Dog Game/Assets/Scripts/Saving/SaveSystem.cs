using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public const string saveName = "SaveData_";
    [Range(0, 10)]
    public int saveDataIndex = 0;

    public static SaveSystem current;

    private void Awake()
    {
        current = this;
    }

    public void SaveData(string dataToSave)
    {
        WriteToFile(saveName + saveDataIndex, dataToSave);
    }

    public static void SaveData(int index, string dataToSave)
    {
        WriteToFile(saveName + index, dataToSave);
        
    }


    public string LoadData()
    {
        string data = "";
        if (ReadFromFile(saveName + saveDataIndex, out data))
        {
            Debug.Log("Successfully loaded data");
        }
        return data;
    }

    private static bool WriteToFile(string name, string content)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, @"SaveData", name + ".DOG");
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, @"SaveData")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, @"SaveData"));

        try
        {
            File.WriteAllText(fullPath, content);
            Debug.Log("Successfully saved data to " + fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving to a file " + e.Message);
        }
        return false;
    }

    private bool ReadFromFile(string name, out string content)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, @"SaveData", name + ".DOG");
        try
        {
            content = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when loading the file " + e.Message);
            content = "";
        }
        return false;
    }

    public bool SaveFileExists() 
    {
        var fullPath = Path.Combine(Application.persistentDataPath, @"SaveData", saveName + saveDataIndex);
        return File.Exists(fullPath);
    }
}