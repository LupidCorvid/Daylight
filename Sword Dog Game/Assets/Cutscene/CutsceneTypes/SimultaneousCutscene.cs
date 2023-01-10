using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimultaneousCutscene : CutsceneData
{

    public CutscenePair[] cutscenes;

    public SimultaneousCutscene(CutscenePair[] cutscenes)
    {
        this.cutscenes = cutscenes;
    }

    public SimultaneousCutscene()
    {

    }

    public override void startSegment()
    {
        foreach (CutscenePair data in cutscenes)
        {
            data.cutscene.finish += partialFinish;
        }

        foreach (CutscenePair data in cutscenes)
        {
            data.cutscene.startSegment();
        }

    }

    public override void Start()
    {
        base.Start();

        
    }

    public override void abort()
    {
        base.abort();
        foreach (CutscenePair data in cutscenes)
        {
            if(data.cutscene != null && !data.cutscene.finished)
                data.cutscene.abort();
        }

        foreach (CutscenePair data in cutscenes)
        {
            data.cutscene.finish -= partialFinish;
        }
    }

    public bool checkIfRequiredFinished()
    {
        foreach(CutscenePair cutscene in cutscenes)
        {
            if (cutscene.RequireCompletion && !cutscene.cutscene.finished)
                return false;
        }

        return true;
    }

    public void partialFinish()
    {
        if (checkIfRequiredFinished())
            finishedSegment();
    }

    public override void finishedSegment()
    {
        abort();
        base.finishedSegment();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        foreach(CutscenePair cutscene in cutscenes)
        {
            if (cutscene.cutscene != null && !cutscene.cutscene.finished)
                cutscene.cutscene.cycleExecution();
        }
    }

    [Serializable]
    public struct CutscenePair
    {
        public bool RequireCompletion;
        public CutsceneData cutscene;

        public CutscenePair(bool required, CutsceneData data)
        {
            RequireCompletion = required;
            cutscene = data;
        }
    }
}
