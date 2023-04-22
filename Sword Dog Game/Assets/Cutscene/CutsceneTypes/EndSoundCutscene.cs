using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSoundCutscene : CutsceneData
{
    public AudioClip Sound = null;
    public SoundPlayer Source;

    public override void startSegment()
    {
        base.startSegment();
        Source.EndSound(Sound);
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
