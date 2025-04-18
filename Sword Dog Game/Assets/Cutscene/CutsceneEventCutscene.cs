using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEventCutscene : CutsceneData
{
    public GameObject personWithScript; //The object that has the eventToCall script. Must have the interface ICutsceneCallable
    //public ICutsceneCallable eventToCall; //Script that has the event?
    public string parameters = "";

    public override void startSegment()
    {
        base.startSegment();
        personWithScript.GetComponent<ICutsceneCallable>().CutsceneEvent(parameters); //eventToCall.CutsceneEvent(parameters);
        finishedSegment();
        Debug.Log(gameObject.name);
        this.enabled = false;
    }

}
