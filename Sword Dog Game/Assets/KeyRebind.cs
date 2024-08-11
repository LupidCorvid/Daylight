using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class KeyRebind : MonoBehaviour
{
    //public InputActionAsset targetAction;

    public bool listeningForNewBind = false;
    public string assignedAction;
    public TMPro.TextMeshProUGUI currKeyDisplay;
    public string oldKey;

    public List<string> blackList = new List<string>();

    public Button rebindButton, backgroundButton;

    public bool StartedListeningThisFrame = false;

    public InputSettingsManager manager;

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onAnyButtonPress.Call(call => UpdateToNewKey(call));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateToNewKey(InputControl data)
    {
        if (listeningForNewBind && !StartedListeningThisFrame)
        {
            if (blackList.Contains(data.name) || blackList.Contains(data.path)) //Specifically meant to allow for disallowing binding menu key to left click
            {
                // Check left click
                if (data.name == "/Mouse/leftButton" || data.path == "/Mouse/leftButton")
                    manager.pointerUpCheck = true;
                return;
            }
            InputReader.inputs.actions[assignedAction].ApplyBindingOverride(data.path);
            //currKeyDisplay.text = data.name;
            currKeyDisplay.text = InputReader.inputs.actions[assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            listeningForNewBind = false;
            manager.CheckDuplicates();
            
            // Check left click
            if (data.name == "/Mouse/leftButton" || data.path == "/Mouse/leftButton")
            {
                manager.pointerUpCheck = true;
                return;
            }

            rebindButton.interactable = true;
        }
    }

    private void LateUpdate()
    {
        StartedListeningThisFrame = false;
    }

    public void StartListening()
    {
        if (manager.pointerUpCheck)
        {
            manager.pointerUpCheck = false;
            return;
        }
        if (listeningForNewBind)
            return;
        listeningForNewBind = true;
        currKeyDisplay.text = "-";
        manager.CheckDuplicates(false);
        manager.DisableOtherButtons(this);
        currKeyDisplay.color = Color.white;
        rebindButton.interactable = false;
        StartedListeningThisFrame = true;
    }

    public void PointerUp()
    {
        if (!rebindButton.interactable)
            rebindButton.interactable = true;
    }
}
