using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeCutscene : CutsceneData
{
    public string Music;
    public AudioManager.GameArea Area;
    public float FadeDuration = 1f;

    public override void startSegment()
    {
        base.startSegment();
        // Automatically fades in/unpauses
        AudioManager.instance.ChangeBGM(Music, Area, FadeDuration);
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        finishedSegment();
    }

    public override void abort()
    {
        base.abort();
    }
}
