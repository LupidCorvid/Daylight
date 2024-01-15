using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class KeyRebind : MonoBehaviour
{

    //public InputActionAsset targetAction;
    

    public bool listeningForNewBind = false;
    public string assignedAction;
    public TMPro.TextMeshProUGUI currKeyDisplay;

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
        if (listeningForNewBind)
        {
            InputReader.inputs.actions[assignedAction].ApplyBindingOverride(data.path);
            
            currKeyDisplay.text = data.name;
            listeningForNewBind = false;
        }
    }

    public void StartListening()
    {
        listeningForNewBind = true;
        currKeyDisplay.text = "-";
    }

  


}
