using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimultaneousCustscene : CutsceneData
{
    private int _numCompleted;

    public int numCompleted
    {
        get { return _numCompleted; }
        set
        {
            if (numCompleted >= cutscenes.Length && !finished)
                finishedSegment();
        }
    }

    public CutsceneData[] cutscenes;
    public SimultaneousCustscene(CutsceneData[] cutscenes)
    {
        this.cutscenes = cutscenes;
    }

    public SimultaneousCustscene()
    {

    }

    public override void startSegment()
    {
        base.startSegment();
        numCompleted = 0;

        foreach (CutsceneData data in cutscenes)
        {
            data.startSegment();
        }

    }

    public override void Start()
    {
        base.Start();

        foreach (CutsceneData data in cutscenes)
        {
            data.finish += (() => numCompleted++);
        }
    }
}
