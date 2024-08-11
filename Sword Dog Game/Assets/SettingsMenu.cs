using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{

    public List<KeyRebind> keyBindings = new List<KeyRebind>();

    public CanvasGroup KeybindsGroup;

    public string oldSettings;
    private bool collectedBindings = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            SettingsManager.currentSettings.fullScreen = !SettingsManager.currentSettings.fullScreen;
            SettingsManager.UpdateFullscreen();
            SettingsManager.SaveSettings();
        }

    }
    
    public void CloseMenu()
    {
        KeybindsGroup.alpha = 0;
        KeybindsGroup.interactable = false;
        KeybindsGroup.blocksRaycasts = false;

        PauseScreen.lockPauseStatus = false;

        InputReader.inputs.actions.LoadBindingOverridesFromJson(oldSettings);
    }

    public void OpenMenu()
    {
        oldSettings = InputReader.inputs.actions.SaveBindingOverridesAsJson();

        if (!collectedBindings)
        {
            keyBindings.AddRange(GetComponentsInChildren<KeyRebind>());
            collectedBindings = true;
        }

        foreach (KeyRebind binding in keyBindings)
        {
            binding.currKeyDisplay.text = InputReader.inputs.actions[binding.assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            binding.oldKey = binding.currKeyDisplay.text;
        }

        KeybindsGroup.alpha = 1;
        KeybindsGroup.interactable = true;
        KeybindsGroup.blocksRaycasts = true;

        PauseScreen.lockPauseStatus = true;
    }

    public void SaveSettings()
    {
        oldSettings = InputReader.inputs.actions.SaveBindingOverridesAsJson();
        foreach (KeyRebind binding in keyBindings)
        {
            binding.oldKey = binding.currKeyDisplay.text;
        }
    }
}
