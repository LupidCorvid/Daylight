using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class CutsceneData : MonoBehaviour
{
    public bool finished = false;
    public Action finish;

    public virtual void Start()
    {

    }

    public virtual void abort()
    {

    }

    public virtual void startSegment()
    {
        finished = false;
    }

    public virtual void finishedSegment()
    {
        finished = true;
        finish?.Invoke();
    }

    public virtual void cycleExecution()
    {

    }

    public CutsceneData()
    {

    }
}
