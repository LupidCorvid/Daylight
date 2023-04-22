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
    bool Stall = true;

    public override void startSegment()
    {
        startTime = Time.time;
        Mixer.GetFloat(Effect, out startValue);
        base.startSegment();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (Stall)
        {
            if (Duration > 0)
                Mixer.SetFloat(Effect, Mathf.Lerp(startValue, Value, (Time.time - startTime) / Duration));
            else
                abort();

            if (startTime + Duration <= Time.time)
                finishedSegment();
        }
        else // Handle as coroutine and allow other cutscene events to progress
        {
            AudioManager.instance?.ApplyMixerEffect(Mixer, Effect, Value, Duration);
            finishedSegment();
        }
    }

    public override void abort()
    {
        base.abort();
        Mixer.SetFloat(Effect, Value);
    }
}
