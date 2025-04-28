using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    public List<CutsceneData> cutscenes = new List<CutsceneData>();

    public static bool inCutscene = false;

    public int cutsceneNumber = 0;

    public static Dictionary<string, CutsceneController> AllCutscenes = new Dictionary<string, CutsceneController>();

    public string cutsceneName;

    public bool playingThisCutscene;

    public bool stopInteractions;
    public bool stopMovement;
    public bool hideUI;
    public bool controlMusic;
    public bool FreezePlayerRigidbody = false;

    public static bool cutsceneStopInteractions = false;
    public static bool cutsceneStopMovement = false;
    public static bool cutsceneHideUI = false;
    public static bool cutsceneControlMusic = false;
    public static bool cutsceneFreezePlayerRb = false;

    public static Action StopAllCutscenes;

    public bool RemoveOnDestroy = true;
    public bool OverwriteCopies = false;

    

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

        for(int i = SceneHelper.betweenSceneData.Count - 1; i >= 0 ; i--)
        {
            if (SceneHelper.betweenSceneData[i].Length <= 1)
                continue;
            if (SceneHelper.betweenSceneData[i][0] == "cutsceneOnLoad")
            {
                if (AllCutscenes.ContainsKey(SceneHelper.betweenSceneData[i][1]))
                    AllCutscenes[SceneHelper.betweenSceneData[i][1]].StartCutscene();
                else
                    Debug.LogError("Couldnt find a cutscene in new scene of name " + SceneHelper.betweenSceneData[i]);
                SceneHelper.betweenSceneData.RemoveAt(i);
            }
        }
    }

    public static void ClearCutscenes()
    {
        AllCutscenes.Clear();
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
            if (OverwriteCopies)
            {
                AllCutscenes.Remove(cutsceneName);
                AllCutscenes.Add(cutsceneName, this);
            }
        }
        else
            AllCutscenes.Add(cutsceneName, this);
    }

    public void OnDestroy()
    {
        if(AllCutscenes.ContainsKey(cutsceneName) && RemoveOnDestroy)
        {
            AllCutscenes.Remove(cutsceneName);
        }
        if(playingThisCutscene)
        {
            StopCutscene();
        }
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

            if(data as SequenceCutscene != null)
            {
                SequenceCutscene seqCutscene = (SequenceCutscene)data;
                foreach(CutsceneData cutscene in seqCutscene.cutscenes)
                {
                    nestedScenes.Add(cutscene);
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
        cutsceneNumber = 0;
        cutscenes[0].startSegment();

        if (stopInteractions)
            cutsceneStopInteractions = true;
        if (stopMovement)
            cutsceneStopMovement = true;
        if (hideUI)
        {
            cutsceneHideUI = true;
            CanvasManager.HideHUD();
        }
        if (controlMusic)
            cutsceneControlMusic = true;
        if (FreezePlayerRigidbody)
            cutsceneFreezePlayerRb = true;
        StopAllCutscenes += FinishCutscene;
    }

    public void FinishCutscene()
    {
        StopAllCutscenes -= StopCutscene;
        StopCutscene();
    }

    public void StopCutscene()
    {
        if(playingThisCutscene && cutsceneNumber < cutscenes.Count && cutscenes[cutsceneNumber] != null)
        {
            cutscenes[cutsceneNumber].abort();
        }

        if (playingThisCutscene && cutsceneStopInteractions && stopInteractions)
            cutsceneStopInteractions = false;
        if (playingThisCutscene && cutsceneStopMovement && stopMovement)
            cutsceneStopMovement = false;
        if (playingThisCutscene && cutsceneHideUI && hideUI)
        {
            cutsceneHideUI = false;
            CanvasManager.ShowHUD();
        }
        if (playingThisCutscene && cutsceneControlMusic && controlMusic)
            cutsceneControlMusic = false;
        if (playingThisCutscene && FreezePlayerRigidbody && cutsceneFreezePlayerRb)
            cutsceneFreezePlayerRb = false;

        inCutscene = false;
        playingThisCutscene = false;
    }

    private void Update()
    {
        if(cutsceneNumber < cutscenes.Count && playingThisCutscene)
            cutscenes[cutsceneNumber].cycleExecution();
    }
}
