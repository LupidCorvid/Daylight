using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundCutscene : CutsceneData
{
    public AudioClip Sound;
    public SoundPlayer Source;
    public float Volume = 1f;
    public bool Loop = false;

    public override void startSegment()
    {
        base.startSegment();
        Source.PlaySound(Sound, Volume, Loop);
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
