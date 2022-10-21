using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mixer;
    public AudioMixerGroup music, sounds;
    public AudioClip currentSong;
    public GameArea currentArea;

    private int activePlayer = 0;
    public AudioSource[] BGM1, BGM2;
    private IEnumerator[] fader = new IEnumerator[2];
    public float volume = 1.0f;

    //Note: If the volumeChangesPerSecond value is higher than the fps, the duration of the fading will be extended!
    private int volumeChangesPerSecond = 15;

    public float fadeDuration = 1.0f;
    private float loopPointSeconds;
    private bool firstSet = true;
    private bool firstSongPlayed = false;

    public AudioMixerSnapshot normal, hurt;

    /// <summary>
    /// List of all different game areas that may have different sets of music
    /// </summary>
    public enum GameArea
    {
        MENU, FOREST, TOWN, CAVE
    }

    /// <summary>
    /// Mutes all AudioSources, but does not stop them!
    /// </summary>
    public bool mute
    {
        set
        {
            foreach (AudioSource s in BGM1)
            {
                s.mute = value;
            }
            foreach (AudioSource s in BGM2)
            {
                s.mute = value;
            }
        }
        get
        {
            if (firstSet)
                return BGM1[activePlayer].mute;
            return BGM2[activePlayer].mute;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            instance = null;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Setup the AudioSources
    /// </summary>
    private void Awake()
    {
        //Generate two AudioSource lists
        BGM1 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        BGM2 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        //Set default values
        foreach (AudioSource s in BGM1)
        {
            s.loop = false;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = music;
        }

        foreach (AudioSource s in BGM2)
        {
            s.loop = false;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = music;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null)
        {
            instance = this;
        }

        //Manages looping tracks
        if (firstSet)
        {
            if (BGM1[activePlayer].time >= loopPointSeconds)
            {
                BGM2[activePlayer].Stop();
                BGM2[activePlayer].time = 0;
                activePlayer = 1 - activePlayer;
                BGM1[activePlayer].clip = currentSong;
                BGM1[activePlayer].volume = volume;
                BGM1[activePlayer].time = 0;
                BGM1[activePlayer].Play();
            }
        }
        else
        {
            if (BGM2[activePlayer].time >= loopPointSeconds)
            {
                BGM2[activePlayer].Stop();
                BGM2[activePlayer].time = 0;
                activePlayer = 1 - activePlayer;
                BGM2[activePlayer].clip = currentSong;
                BGM2[activePlayer].volume = volume;
                BGM2[activePlayer].time = 0;
                BGM2[activePlayer].Play();
            }
        }

        // Toggle muting (press M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            mute = !mute;
        }

        // Volume controls (hold down + or -)
        float vol;
        mixer.GetFloat("Volume", out vol);

        if (Input.GetKey("="))
        {
            if (vol < 0f)
                vol += 0.1f;
            mixer.SetFloat("Volume", vol);
        }

        if (Input.GetKey("-"))
        {
            if (vol > -80f)
                vol -= 0.1f;
            mixer.SetFloat("Volume", vol);
        }

        float pitch;
        mixer.GetFloat("Pitch", out pitch);
        if (pitch <= 0.05f)
        {
            SetPitch(1f);
            if (firstSet)
            {
                BGM1[activePlayer].volume = 0;
                BGM1[activePlayer].timeSamples = 0;
                fader[0] = FadeAudioSource(BGM1[activePlayer], fadeDuration, volume, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            else
            {
                BGM2[activePlayer].volume = 0;
                BGM2[activePlayer].timeSamples = 0;
                fader[0] = FadeAudioSource(BGM2[activePlayer], fadeDuration, volume, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
        }

        // Scene specific SFX effects
        if (currentArea == GameArea.CAVE)
            mixer.SetFloat("Reverb", -4f);
        else
            mixer.SetFloat("Reverb", -10000f);
    }

    public void ChangeBGM(AudioClip music, int BPM, int timeSignature, int barsLength, GameArea newArea)
    {
        // carry on music if area has not changed
        bool carryOn = newArea == currentArea;
        currentArea = newArea;

        //Calculate loop point
        loopPointSeconds = 60.0f * (barsLength * timeSignature) / BPM;

        //Prevent fading the same clip on both players
        if (music == currentSong)
            return;

        //Kill all playing
        foreach (IEnumerator i in fader)
        {
            if (i != null)
            {
                StopCoroutine(i);
            }
        }

        if (firstSet)
        {
            //Fade-out the active play, if it is not silent (eg: first start)
            if (BGM1[activePlayer].volume > 0)
            {
                fader[0] = FadeAudioSource(BGM1[activePlayer], fadeDuration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM1[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM2[activePlayer].clip = music;
            if (carryOn)
            {
                BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples; // syncs up time
            }
            else
            {
                BGM2[activePlayer].timeSamples = 0;
            }
            BGM2[activePlayer].Play();
            if (firstSongPlayed)
            {
                fader[1] = FadeAudioSource(BGM2[activePlayer], fadeDuration, volume, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM2[activePlayer].volume = volume;
            }
        }
        else
        {
            //Fade-out the active play, if it is not silent (eg: first start)
            if (BGM2[activePlayer].volume > 0)
            {
                fader[0] = FadeAudioSource(BGM2[activePlayer], fadeDuration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM2[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM1[activePlayer].clip = music;
            if (carryOn)
            {
                BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples; // syncs up time
            }
            else
            {
                BGM1[activePlayer].timeSamples = 0;
            }
            BGM1[activePlayer].Play();
            if (firstSongPlayed)
            {
                fader[1] = FadeAudioSource(BGM1[activePlayer], fadeDuration, volume, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM1[activePlayer].volume = volume;
            }
        }

        firstSet = !firstSet;
        firstSongPlayed = true;

        //Set new clip to current song
        currentSong = music;
    }

    /// <summary>
    /// Fades an AudioSource(player) during a given amount of time(duration) to a specific volume(targetVolume)
    /// </summary>
    /// <param name="player">AudioSource to be modified</param>
    /// <param name="duration">Duration of the fading</param>
    /// <param name="targetVolume">Target volume, the player is faded to</param>
    /// <param name="finishedCallback">Called when finshed</param>
    /// <returns></returns>
    IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume, System.Action finishedCallback)
    {
        //Calculate the steps
        int Steps = (int)(volumeChangesPerSecond * duration);
        float StepTime = duration / Steps;
        float StepSize = (targetVolume - player.volume) / Steps;

        //Fade now
        for (int i = 1; i < Steps; i++)
        {
            player.volume += StepSize;
            yield return new WaitForSeconds(StepTime);
        }
        //Make sure the targetVolume is set
        player.volume = targetVolume;

        //Callback
        if (finishedCallback != null)
        {
            finishedCallback();
        }
    }

    public void SetPitch(float pitch)
    {
        mixer.SetFloat("Pitch", pitch);
    }

    public IEnumerator PitchDown()
    {
        for (float i = 0; i <= 1; i += 0.05f)
        {
            SetPitch(Mathf.Lerp(1f, 0f, i));
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void FadeOutCurrent()
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], fadeDuration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], fadeDuration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
    }

    public void FadeInCurrent()
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], fadeDuration, volume, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], fadeDuration, volume, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
    }

    public void PauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].Pause();
        }
        else
        {
            BGM2[activePlayer].Pause();
        }
    }

    public void UnPauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].UnPause();
        }
        else
        {
            BGM2[activePlayer].UnPause();
        }
    }

    public void Stop()
    {
        foreach (AudioSource source in BGM1)
        {
            source.Stop();
        }
        foreach (AudioSource source in BGM2)
        {
            source.Stop();
        }
    }

    public IEnumerator Muffle()
    {
        hurt.TransitionTo(0.1f);
        yield return new WaitForSeconds(1f);
        normal.TransitionTo(1f);
    }
}