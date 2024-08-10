using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSettingsManager : MonoBehaviour
{
    public SettingsMenu parent;
    public Button saveButton, resetButton, backButton;
    public bool pointerUpCheck;

    public void CheckDuplicates(bool enableBackground = true)
    {
        bool duplicates = false;
        ResetDuplicates();

        // Mark duplicate pairs in red
        for (int i = 0; i < parent.keyBindings.Count; i++)
        {
            if (enableBackground)
                parent.keyBindings[i].backgroundButton.interactable = true;
            
            for (int j = i + 1; j < parent.keyBindings.Count; j++)
            {
                if (parent.keyBindings[i].currKeyDisplay.text == parent.keyBindings[j].currKeyDisplay.text)
                {
                    parent.keyBindings[i].currKeyDisplay.color = Color.red;
                    parent.keyBindings[j].currKeyDisplay.color = Color.red;
                    duplicates = true;
                }
            }
        }

        if (enableBackground)
        {
            // Save button usability depends on duplicate keybinds
            saveButton.interactable = !duplicates;

            resetButton.interactable = true;
            backButton.interactable = true;
        }
    }

    public void ResetDuplicates()
    {
        foreach (KeyRebind k in parent.keyBindings)
        {
            k.currKeyDisplay.color = Color.white;
        }
    }

    public void DisableOtherButtons(KeyRebind curr)
    {
        curr.backgroundButton.interactable = true;
        foreach (KeyRebind k in parent.keyBindings)
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
        foreach (KeyRebind k in parent.keyBindings)
        {
            k.currKeyDisplay.color = Color.white;
            k.backgroundButton.interactable = true;
            k.rebindButton.interactable = true;
        }
        saveButton.interactable = false;
        resetButton.interactable = true;
        backButton.interactable = true;
    }

    public void SaveSettings()
    {
        saveButton.interactable = false;
    }
}
