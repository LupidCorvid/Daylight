using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventOnLoad : MonoBehaviour
{
    public AK.Wwise.Event Event;
    public bool Global = true;
    public GameObject Object;
    public float delay = 0;
    public string CheckState = "";
    public AK.Wwise.State CheckStateValue;

    // Start is called before the first frame update
    void Start()
    {
        if (Event != null)
        {
            Debug.Log("Hello! " + Event);
            if (CutsceneController.inCutscene && CutsceneController.cutsceneControlMusic)
            {
                return;
            }

            if (delay > 0)
            {
                StartCoroutine(PostEventDelayed());
            }
            else
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
            }
        }
    }

    IEnumerator PostEventDelayed()
    {
        yield return new WaitForSeconds(delay);
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
    }
}
