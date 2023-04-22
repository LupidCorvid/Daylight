using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeCutscene : CutsceneData
{
    public string Music;
    public AudioManager.GameArea Area;
    public float FadeDuration = 1f;
    float startTime;
    public bool Stall = false;

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
        // Automatically crossfades/unpauses
        AudioManager.instance.ChangeBGM(Music, Area, FadeDuration);
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (startTime + FadeDuration <= Time.time || !Stall)
            finishedSegment();
    }

    public override void abort()
    {
        base.abort();
    }
}
