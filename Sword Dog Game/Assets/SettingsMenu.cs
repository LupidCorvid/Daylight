using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{

    public List<KeyRebind> keyBindings = new List<KeyRebind>();
    // Start is called before the first frame update
    void Start()
    {
        keyBindings.AddRange(GetComponentsInChildren<KeyRebind>());
        foreach (KeyRebind binding in keyBindings)
        {
            //binding.currKeyDisplay.text = InputReader.inputs.actions[binding.assignedAction].bindings[0].name;
            binding.currKeyDisplay.text = InputReader.inputs.actions[binding.assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBinds()
    {
        InputReader.inputs.actions.RemoveAllBindingOverrides();

        foreach(KeyRebind binding in keyBindings)
        {
            binding.currKeyDisplay.text = InputReader.inputs.actions[binding.assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
    }

    public void SaveSettings()
    {
        SettingsManager.SaveSettings();
    }

    public void CloseMenu()
    {

    }
}
