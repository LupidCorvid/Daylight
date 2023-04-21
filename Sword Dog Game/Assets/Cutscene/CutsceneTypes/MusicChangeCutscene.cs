using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeCutscene : CutsceneData
{
    public AudioClip NewTrack;
    public int BPM, TimeSignature, BarsLength;
    public AudioManager.GameArea Area;

    public override void startSegment()
    {
        base.startSegment();
        // Automatically fades in/unpauses
        AudioManager.instance.ChangeBGM(NewTrack, BPM, TimeSignature, BarsLength, Area);
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
