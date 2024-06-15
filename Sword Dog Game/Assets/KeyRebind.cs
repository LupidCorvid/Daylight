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

    public List<string> blackList = new List<string>();

    public Button rebindButton;

    public bool StartedListeningThisFrame = false;

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
            if (blackList.Contains(data.name) || blackList.Contains(data.path)) //Specifically meant to allow for disallowing binding menu key to lmb
                return;
            InputReader.inputs.actions[assignedAction].ApplyBindingOverride(data.path);
            //currKeyDisplay.text = data.name;
            currKeyDisplay.text = InputReader.inputs.actions[assignedAction].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            listeningForNewBind = false;
            rebindButton.interactable = true;
        }
    }

    private void LateUpdate()
    {
        StartedListeningThisFrame = false;
    }

    public void StartListening()
    {
        if (listeningForNewBind)
            return;
        listeningForNewBind = true;
        currKeyDisplay.text = "-";
        rebindButton.interactable = false;
        StartedListeningThisFrame = true;
    }

  


}
