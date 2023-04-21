using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerEffectCutscene : CutsceneData
{
    public AudioMixer Mixer;
    public string Effect;
    public float Value;
    public float Duration = 0f;
    float startValue;
    float startTime;

    public override void startSegment()
    {
        startTime = Time.time;
        Mixer.GetFloat(Effect, out startValue);
        base.startSegment();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        Mixer.SetFloat(Effect, Mathf.Lerp(startValue, Value, (Time.time - startTime) / Duration));
        if (startTime + Duration <= Time.time) {
            finishedSegment();
        }
    }

    public override void abort()
    {
        base.abort();
        Mixer.SetFloat(Effect, Value);
    }
}
