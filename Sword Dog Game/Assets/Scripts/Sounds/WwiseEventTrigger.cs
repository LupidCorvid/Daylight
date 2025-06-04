using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventTrigger : MonoBehaviour
{
    public AK.Wwise.Event Event;
    public bool Global = true;
    public GameObject Object;
    public string CheckState = "";
    public AK.Wwise.State CheckStateValue;
    public bool KillOnActivate = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Event != null && !CutsceneController.inCutscene)
        {
            if (CheckState == "" || CheckStateValue == null)
            {
                Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
            }
            else
            {
                uint stateID;
                AkUnitySoundEngine.GetState(CheckState, out stateID);
                if (stateID != CheckStateValue.Id)
                    Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
            }
            if (KillOnActivate)
                Destroy(gameObject);
        }
    }
}
