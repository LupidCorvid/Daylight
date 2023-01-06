using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();

    public static bool inCutscene = false;

    public int cutsceneNumber = 0;

    public bool autoFillList = true;

    //Maybe make a different cutscene holder so that multiple cutscenes can be saved without needing multiple controllers (although currently mutliple controllers is fine)

    /*Other cutscene data ideas:
     * wait until another cutscene finishes
     * Move to different points in a sequence
     * Integrate player movement to allow use of animator when moving
     * 
     * 
     */

    // Start is called before the first frame update
    void Start()
    {
        if (autoFillList)
            FillListFromComponents();

        setupCutsceneChain();
        StartCutscene();
    }

    public void setupCutsceneChain()
    {
        for (int i = 0; i < cutscenes.Count - 1; i++)
        {
            cutscenes[i].finish += cutscenes[i + 1].startSegment;
            cutscenes[i].finish += (() => cutsceneNumber++);
        }

        if (cutscenes.Count > 0)
        {
            cutscenes[^1].finish += (() => inCutscene = false);
            cutscenes[^1].finish += (() => cutsceneNumber++);
        }
    }


    public void FillListFromComponents()
    {
        cutscenes.Clear();
        cutscenes.AddRange(GetComponents<CutsceneData>());
    }

    public void StartCutscene()
    {
        inCutscene = true;
        cutscenes[0].startSegment();
    }

    private void Update()
    {
        if(cutsceneNumber < cutscenes.Count)
            cutscenes[cutsceneNumber].cycleExecution();
    }
}
