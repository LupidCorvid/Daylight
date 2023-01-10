using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();

    public static bool inCutscene = false;

    public int cutsceneNumber = 0;

    public static Dictionary<string, CutsceneController> AllCutscenes = new Dictionary<string, CutsceneController>();

    public string cutsceneName;

    public bool playingThisCutscene;

    //Maybe make a different cutscene holder so that multiple cutscenes can be saved without needing multiple controllers (although currently mutliple controllers is fine)

    /*Other cutscene data ideas:
     * wait until another cutscene finishes
     * Move to different points in a sequence
     * Integrate player movement to allow use of animator when moving
     * Allow for loading new scenes through cutscenes
     * 
     */

    public static void CheckForLoadCutsceneTransition()
    {
        foreach(string[] param in SceneHelper.betweenSceneData)
        {
            if (param.Length <= 1)
                continue;
            if(param[0] == "cutsceneOnLoad")
            {
                if (AllCutscenes.ContainsKey(param[1]))
                    AllCutscenes[param[1]].StartCutscene();
                else
                    Debug.LogError("Couldnt find a cutscene in new scene of name " + param[1]);
            }
            
        }
    }

    public static void PlayCutscene(string name)
    {
        if (AllCutscenes.ContainsKey(name))
            AllCutscenes[name].StartCutscene();
        else
            Debug.LogWarning("Failed load cutscene. No cutscene with name " + name + " found");
    }

    public void Awake()
    {
        setupCutsceneChain();
        if (AllCutscenes.ContainsKey(cutsceneName))
        {
            //The is recalled everytime a scene is reloaded. Behavior therefore is expected and not an error
            //Debug.LogError("There is already a cutscene with the name " + cutsceneName);
        }
        else
            AllCutscenes.Add(cutsceneName, this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
        
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
            cutscenes[^1].finish += (() => cutsceneNumber++);
            cutscenes[^1].finish += FinishCutscene;
        }
    }


    public void FillListFromComponents()
    {
        cutscenes.Clear();
        List<CutsceneData> newList = new List<CutsceneData>();
        newList.AddRange(GetComponents<CutsceneData>());

        List<CutsceneData> nestedScenes = new List<CutsceneData>();

        foreach(CutsceneData data in newList)
        {
            
            if (data as SimultaneousCutscene != null)
            {
                SimultaneousCutscene multiCutscene = (SimultaneousCutscene)data;
                foreach (SimultaneousCutscene.CutscenePair pair in multiCutscene.cutscenes)
                {
                    nestedScenes.Add(pair.cutscene);
                }
            }
        }

        for(int i = newList.Count - 1; i >= 0; --i)
        {
            if(nestedScenes.Contains(newList[i]))
            {
                newList.RemoveAt(i);
            }
        }
        cutscenes = newList;
    }

    public void StartCutscene()
    {
        if (cutscenes.Count <= 0)
            return;
        inCutscene = true;
        playingThisCutscene = true;
        cutscenes[0].startSegment();
    }

    public void FinishCutscene()
    {
        inCutscene = false;
        playingThisCutscene = false;
    }

    private void Update()
    {
        if(cutsceneNumber < cutscenes.Count && playingThisCutscene)
            cutscenes[cutsceneNumber].cycleExecution();
    }
}
