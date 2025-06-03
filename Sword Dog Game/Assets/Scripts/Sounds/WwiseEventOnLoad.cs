using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventOnLoad : MonoBehaviour
{
    public AK.Wwise.Event Event;
    public bool Global = true;
    public GameObject Object;
    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Event != null)
        {
            if (CutsceneController.inCutscene && CutsceneController.cutsceneControlMusic)
            {
                return;
            }

            if (delay > 0)
            {
                StartCoroutine(PostEventDelayed());
            }
            else
                Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
        }
    }

    IEnumerator PostEventDelayed()
    {
        yield return new WaitForSeconds(delay);
        Event?.Post(Global ? AudioManager.WwiseGlobal : Object);
    }
}
