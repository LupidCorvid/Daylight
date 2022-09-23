using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource[] sources;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        sources = transform.GetComponents<AudioSource>();
    }

    public void PlaySound(int sound)
    {
        AudioClip clip = clips[sound];
        foreach (AudioSource source in sources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                if (source.time < 0.2f && source.isPlaying) return;
                else source.Stop();
            }
        }
        for (int index = sources.Length - 1; index >= 0; index--)
        {
            if (!sources[index].isPlaying)
            {
                sources[index].clip = clip;
                sources[index].loop = false;
                sources[index].Play();
                return;
            }
        }
    }

    void CleanSounds()
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                source.Stop();
                source.clip = null;
                source.time = 0;
            }
        }
    }
}
