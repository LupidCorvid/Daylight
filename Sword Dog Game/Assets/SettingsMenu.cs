using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBinds()
    {
        InputReader.inputs.actions.RemoveAllBindingOverrides();
    }

    public void SaveSettings()
    {
        SettingsManager.SaveSettings();
    }

    public void CloseMenu()
    {

    }
}
