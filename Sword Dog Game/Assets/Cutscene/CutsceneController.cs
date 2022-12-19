using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();


    public enum types
    {
        NPC = 0,
        Wait,
        simult,
        interr
    }

    public types newType;

    // Start is called before the first frame update
    void Start()
    {
        foreach(CutsceneData cutscene in cutscenes)
        {
            cutscene.onLoad();
        }
    }

    public void addCutscene(CutsceneData newCutscene)
    {
        cutscenes.Add(newCutscene);
    }

    public void addNewCutscene()
    {
        CutsceneData newCutscene = null;
        switch(newType)
        {
            case types.NPC:
                newCutscene = new CutsceneNPCDialog();
                break;
            case types.interr:
                newCutscene = new InterruptibleCutscenes();
                break;
            case types.simult:
                newCutscene = new SimultaneousCustscene();
                break;
            case types.Wait:
                newCutscene = new WaitCutscene();
                break;
        }
        addCutscene(newCutscene);
    }
}
