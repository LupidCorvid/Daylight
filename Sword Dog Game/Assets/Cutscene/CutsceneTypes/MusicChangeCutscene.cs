using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeCutscene : CutsceneData
{
    public MusicClip Music;
    // TODO deprecate
    public AudioManager.GameArea Area;
    public float FadeDuration = 1f;
    float startTime;
    public bool Stall = false;

    public override void startSegment()
    {
        base.startSegment();
        startTime = Time.time;
        // Automatically crossfades/unpauses

        // TODO this in the future
        // AudioManager.instance.ChangeBGM(Music, FadeDuration);
        Debug.Log(AudioManager.instance.currentSong + " " + Music);
        if (AudioManager.instance.currentSong != Music)
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
