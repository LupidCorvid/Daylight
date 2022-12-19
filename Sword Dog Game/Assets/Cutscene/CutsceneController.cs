using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();


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

}
