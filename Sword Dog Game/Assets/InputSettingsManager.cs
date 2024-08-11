using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSettingsManager : MonoBehaviour
{
    public SettingsMenu menu;
    public Button saveButton, resetButton, backButton;
    public bool pointerUpCheck; 

    public void CheckDuplicates(bool affectButtons = true)
    {
        bool duplicates = false;
        ResetDuplicates();

        // Mark duplicate pairs in red
        for (int i = 0; i < menu.keyBindings.Count; i++)
        {
            if (affectButtons)
                menu.keyBindings[i].backgroundButton.interactable = true;
            
            for (int j = i + 1; j < menu.keyBindings.Count; j++)
            {
                if (menu.keyBindings[i].currKeyDisplay.text == menu.keyBindings[j].currKeyDisplay.text)
                {
                    menu.keyBindings[i].currKeyDisplay.color = Color.red;
                    menu.keyBindings[j].currKeyDisplay.color = Color.red;
                    duplicates = true;
                }
            }
        }

        if (affectButtons)
        {
            // Save button usability depends on modifications and no duplicate keybinds
            saveButton.interactable = !duplicates && CheckModifications();
            
            resetButton.interactable = true;
            backButton.interactable = true;
        }
    }

    public void ResetDuplicates()
    {
        foreach (KeyRebind k in menu.keyBindings)
        {
            k.currKeyDisplay.color = Color.white;
        }
    }

    public void DisableOtherButtons(KeyRebind curr)
    {
        curr.backgroundButton.interactable = true;
        foreach (KeyRebind k in menu.keyBindings)
        {
            if (k != curr)
            {
                k.backgroundButton.interactable = false;
                k.rebindButton.interactable = false;
            }
        }
        saveButton.interactable = false;
        resetButton.interactable = false;
        backButton.interactable = false;
    }

    public void ResetMenu()
    {
        InputReader.inputs.actions.RemoveAllBindingOverrides();
        bool modified = false;
        foreach (KeyRebind k in menu.keyBindings)
        {
            k.currKeyDisplay.text = InputReader.inputs.actions[k.assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            k.currKeyDisplay.color = Color.white;
            k.backgroundButton.interactable = true;
            k.rebindButton.interactable = true;
            if (k.currKeyDisplay.text != k.oldKey)
                modified = true;
        }
        saveButton.interactable = modified;
        resetButton.interactable = true;
        backButton.interactable = true;
    }

    public bool CheckModifications()
    {
        foreach (KeyRebind k in menu.keyBindings)
        {
            if (k.currKeyDisplay.text != k.oldKey)
                return true;
        }
        return false;
    }

    public void SaveSettings()
    {
        saveButton.interactable = false;
    }
}
