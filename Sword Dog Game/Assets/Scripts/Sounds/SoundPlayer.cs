using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource[] sources;

    // Start is called before the first frame update
    void Start()
    {
        sources = transform.GetComponents<AudioSource>();
    }

    public void PlaySound(string path, float volume = 1, bool loop = false)
    {
        PlaySound(AudioManager.instance?.FindSound(path), volume, loop);
    }

    public void PlaySound(SoundPlayable clip, float volume = 1, bool loop = false)
    {
        PlaySound(clip.GetClip(), volume, loop);
    }
    
    public void PlaySound(AudioClip clip, float volume = 1, bool loop = false)
    {
        if (clip == null) return;

        foreach (AudioSource source in sources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                if (source.time < 0.2f) return;
                else source.Stop();
            }
        }
        for (int index = sources.Length - 1; index >= 0; index--)
        {
            if (!sources[index].isPlaying)
            {
                sources[index].clip = clip;
                sources[index].loop = loop;
                sources[index].volume = volume;
                sources[index].Play();
                return;
            }
        }
    }

    public void EndSound(string path = null)
    {
        if (path == null)
        {
            foreach (AudioSource source in sources)
            {
                source.Stop();
                source.clip = null;
                source.time = 0;
            }
            return;
        }
        EndSound(AudioManager.instance?.FindSound(path));
    }

    public void EndSound(SoundClip clip) // TODO does not support SoundSet
    {
        EndSound(clip?.GetClip());
    }

    public void EndSound(AudioClip clip = null)
    {
        if (clip == null)
        {
            foreach (AudioSource source in sources)
            {
                source.Stop();
                source.clip = null;
                source.time = 0;
            }
            return;
        }

        foreach (AudioSource source in sources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    public void PauseSound(string path = null)
    {
        if (path == null)
        {
            foreach (AudioSource source in sources)
            {
                source.Pause();
            }
            return;
        }
        PauseSound(AudioManager.instance?.FindSound(path));
    }

    public void PauseSound(SoundClip clip) // TODO does not support SoundSet
    {
        PauseSound(clip?.GetClip());
    }

    public void PauseSound(AudioClip clip = null)
    {
        if (clip == null)
        {
            foreach (AudioSource source in sources)
            {
                source.Pause();
            }
            return;
        }

        foreach (AudioSource source in sources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    public void UnPauseSound(string path = null)
    {
        if (path == null)
        {
            foreach (AudioSource source in sources)
            {
                source.UnPause();
            }
            return;
        }
        UnPauseSound(AudioManager.instance?.FindSound(path));
    }

    public void UnPauseSound(SoundClip clip) // TODO does not support SoundSet
    {
        UnPauseSound(clip?.GetClip());
    }

    public void UnPauseSound(AudioClip clip = null)
    {
        if (clip == null)
        {
            foreach (AudioSource source in sources)
            {
                source.UnPause();
            }
            return;
        }

        foreach (AudioSource source in sources)
        {
            if (source.clip == clip)
            {
                source.UnPause();
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
