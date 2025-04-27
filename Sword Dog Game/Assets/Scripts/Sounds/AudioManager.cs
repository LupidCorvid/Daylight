using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer musicMixer, sfxMixer, globalSfxMixer;
    public AudioMixerGroup musicMixerGroup;
    public MusicClip currentSong = null;
    public GameArea currentArea;

    private int activePlayer = 0;
    public AudioSource[] BGM1, BGM2;
    private IEnumerator[] fader = new IEnumerator[2];
    public float musicVolume = 1.0f, sfxVolume = 10.0f, targetSFXVolume= -80.0f, actualSFXVolume = -80.0f;

    //Note: If the volumeChangesPerSecond value is higher than the fps, the duration of the fading will be extended!
    private int volumeChangesPerSecond = 15;

    public float fadeDuration = 1.0f;
    private float loopPointSeconds, repeatPointSeconds;
    private bool firstSet = true;
    private bool firstSongPlayed = false;
    public bool paused = false;

    public AudioMixerSnapshot normal, hurt;
    public SoundCategory soundDatabase;
    public MusicCategory musicDatabase;

    /// <summary>
    /// List of all different game areas that may have different sets of music
    /// </summary>
    public enum GameArea
    {
        CURRENT, MENU, PROLOGUE, FOREST, TOWN, CAVES, MOUNTAIN, DESERT, UNDERGROUND_DESERT, OCEAN
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
        if (SettingsManager.currentSettings != null)
        {
            musicVolume = SettingsManager.currentSettings.musicVolume;
            mute = SettingsManager.currentSettings.musicMute;

            sfxVolume = SettingsManager.currentSettings.sfxVolume;
        }
        else
        {
            musicMixer.GetFloat("Volume", out musicVolume);
        }

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
            s.outputAudioMixerGroup = musicMixerGroup;
        }

        foreach (AudioSource s in BGM2)
        {
            s.loop = false;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null)
        {
            instance = this;
        }

        //// Check for desyncing during crossfades
        //if ((fader[0] != null || fader[1] != null) && BGM2[activePlayer].isPlaying && BGM1[activePlayer].isPlaying)
        //{
        //    if (firstSet && BGM2[activePlayer].timeSamples != BGM1[activePlayer].timeSamples)
        //    {
        //        BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples;
        //    }
        //    else if (!firstSet && BGM1[activePlayer].timeSamples != BGM2[activePlayer].timeSamples)
        //    {
        //        BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples;
        //    }
        //}

        //Manages looping tracks
        if (firstSet)
        {
            if (BGM1[activePlayer].time >= loopPointSeconds)
            {
                activePlayer = 1 - activePlayer;
                if (currentSong != null)
                    BGM1[activePlayer].clip = currentSong.GetClip();
                BGM1[activePlayer].volume = 1.0f;
                BGM1[activePlayer].time = repeatPointSeconds;
                BGM1[activePlayer].Play();
            }
        }
        else
        {
            if (BGM2[activePlayer].time >= loopPointSeconds)
            {
                activePlayer = 1 - activePlayer;
                if (currentSong != null)
                    BGM2[activePlayer].clip = currentSong.GetClip();
                BGM2[activePlayer].volume = 1.0f;
                BGM2[activePlayer].time = repeatPointSeconds;
                BGM2[activePlayer].Play();
            }
        }

        // Toggle muting (press M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            mute = !mute;
        }
        SettingsManager.currentSettings.musicMute = mute;

        // Volume controls (hold down + or -)
        musicMixer.SetFloat("Volume", musicVolume);
        musicMixer.GetFloat("Volume", out musicVolume);

        if (Input.GetKey("="))
        {
            if (musicVolume < 0f)
                musicVolume += 0.1f;
        }

        if (Input.GetKey("-"))
        {
            if (musicVolume > -80f)
                musicVolume -= 0.1f;
        }
        SettingsManager.currentSettings.musicVolume = musicVolume;
        SettingsManager.currentSettings.sfxVolume = sfxVolume;

        float pitch;
        musicMixer.GetFloat("Pitch", out pitch);
        if (pitch <= 0.05f)
        {
            SetPitch(1f);
            if (firstSet)
            {
                BGM1[activePlayer].volume = 0;
                BGM1[activePlayer].timeSamples = 0;
                fader[0] = FadeAudioSource(BGM1[activePlayer], fadeDuration, 1.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            else
            {
                BGM2[activePlayer].volume = 0;
                BGM2[activePlayer].timeSamples = 0;
                fader[0] = FadeAudioSource(BGM2[activePlayer], fadeDuration, 1.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
        }

        // Scene specific SFX effects
        if (currentArea == GameArea.CAVES)
            sfxMixer.SetFloat("Reverb", -4f);
        else
            sfxMixer.SetFloat("Reverb", -10000f);
        
        // sfxVolume is a float from 0.0-1.0 but we'd want 1.0 to correspond to 10dB => *10f
        targetSFXVolume = SettingsManager.currentSettings.sfxVolume*10f - Mathf.Clamp(2 * Camera.main.orthographicSize, 10, 100);
        if (ChangeScene.changingScene || GameSaver.loading)
        {
            actualSFXVolume = Mathf.Lerp(actualSFXVolume, -80, 0.1f);
        }
        else
        {
            actualSFXVolume = Mathf.Lerp(actualSFXVolume, targetSFXVolume, 0.1f);
        }
        sfxMixer.SetFloat("Volume", actualSFXVolume);
    }

    public void ChangeBGM(string musicPath, float duration = 1f)
    {
        MusicClip music = FindMusic(musicPath);
        ChangeBGM(music, duration);
    }

    public void ChangeBGM(string musicPath, string area, float duration = 1f)
    {
        GameArea theArea;
        switch (area.Trim().ToUpper())
        {
            case "CURRENT":
                theArea = currentArea;
                break;
            case "MENU":
                theArea = GameArea.MENU;
                break;
            case "PROLOGUE":
                theArea = GameArea.PROLOGUE;
                break;
            case "TOWN":
                theArea = GameArea.TOWN;
                break;
            case "FOREST":
                theArea = GameArea.FOREST;
                break;
            case "CAVES":
                theArea = GameArea.CAVES;
                break;
            case "MOUNTAIN":
                theArea = GameArea.MOUNTAIN;
                break;
            case "DESERT":
                theArea = GameArea.DESERT;
                break;
            case "UNDERGROUND_DESERT":
                theArea = GameArea.UNDERGROUND_DESERT;
                break;
            case "OCEAN":
                theArea = GameArea.OCEAN;
                break;
            default:
                Debug.LogWarning("Invalid area provided! Using current");
                theArea = currentArea;
                break;
        }
        ChangeBGM(FindMusic(musicPath), theArea, duration);
    }

    public void ChangeBGM(string musicPath, GameArea area, float duration = 1f)
    {
        ChangeBGM(FindMusic(musicPath), area, duration);
    }

    public void ChangeBGM(MusicClip music, float duration = 1f)
    {
        ChangeBGM(music, music.area, duration);
    }

    public void ChangeBGM(MusicClip music, GameArea newArea, float duration = 1f)
    {
        // support cutscenes keeping music area
        if (newArea == GameArea.CURRENT) newArea = currentArea;

        // carry on music if area has not changed
        bool carryOn = newArea == currentArea;
        currentArea = newArea;

        //Calculate loop point
        loopPointSeconds = 60.0f * (music.barsLength * 4 * music.timeSignature / music.timeSignatureBottom) / music.BPM;

        //Calculate repeat point
        repeatPointSeconds = 60.0f * (music.repeatBar * 4 * music.timeSignature / music.timeSignatureBottom) / music.BPM;

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
                fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM1[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM2[activePlayer].clip = music.GetClip();
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
                fader[1] = FadeAudioSource(BGM2[activePlayer], duration, 1.0f, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM2[activePlayer].volume = 1.0f;
            }
        }
        else
        {
            //Fade-out the active play, if it is not silent (eg: first start)
            if (BGM2[activePlayer].volume > 0)
            {
                fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM2[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM1[activePlayer].clip = music.GetClip();
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
                fader[1] = FadeAudioSource(BGM1[activePlayer], duration, 1.0f, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM1[activePlayer].volume = 1.0f;
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
        musicMixer.SetFloat("Pitch", pitch);
    }

    public IEnumerator PitchDown()
    {
        for (float i = 0; i <= 1; i += 0.05f)
        {
            SetPitch(Mathf.Lerp(1f, 0f, i));
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void FadeOutCurrent(float duration = 1f)
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
    }

    public void FadeInCurrent(float duration = 1f)
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 1.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 1.0f, () => { fader[0] = null; });
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
        paused = true;
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
        paused = false;
    }

    public void Stop()
    {
        foreach (AudioSource source in BGM1)
        {
            source.Stop();
            source.clip = null;
        }
        foreach (AudioSource source in BGM2)
        {
            source.Stop();
            source.clip = null;
        }
        currentSong = null;
        paused = false;
    }

    public void ApplyMixerEffect(string mixer, string effect, float value, float duration = 0)
    {
        AudioMixer theMixer = null;
        switch (mixer.Trim().ToLower())
        {
            case "music":
            case "bgm":
                theMixer = musicMixer;
                break;
            case "sound":
            case "sfx":
                theMixer = sfxMixer;
                break;
            case "global":
                theMixer = globalSfxMixer;
                break;
        }
        if (theMixer != null) 
            ApplyMixerEffect(theMixer, effect, value, duration);
        else
            Debug.LogError("Invalid mixer!");
    }

    public void ApplyMixerEffect(AudioMixer mixer, string effect, float value, float duration = 0)
    {
        if (duration == 0)
        {
            mixer.SetFloat(effect.Trim(), value);
            return;
        }
        StartCoroutine(gradualMixerEffect(mixer, effect.Trim(), value, duration));
    }

    private IEnumerator gradualMixerEffect(AudioMixer mixer, string effect, float value, float duration)
    {
        float startTime = Time.time;
        float startValue;
        mixer.GetFloat(effect, out startValue);
        while (startTime + duration >= Time.time)
        {
            mixer.SetFloat(effect, Mathf.Lerp(startValue, value, (Time.time - startTime) / duration));
            yield return new WaitForEndOfFrame();
        }
        mixer.SetFloat(effect, value);
    }

    public IEnumerator Muffle()
    {
        hurt.TransitionTo(0.1f);
        yield return new WaitForSeconds(1f);
        normal.TransitionTo(1f);
    }

    public AudioClip FindSound(string soundPath)
    {
        List<string> path = new List<string>(soundPath.Trim().Split("."));
        return FindSound(soundDatabase, path);
    }

    public AudioClip FindSound(SoundNode current, List<string> path)
    {
        if (current is SoundPlayable)
        {
            return ((SoundPlayable)current).GetClip();
        }
        else if (current is SoundCategory)
        {
            foreach (SoundNode node in ((SoundCategory)current).children)
            {
                if (path.Count > 0 && node.name.ToLower() == path[0].ToLower())
                {
                    // Debug.Log("Found " + path[0]);
                    current = node;
                    path.RemoveAt(0);
                    return FindSound(node, path);
                }
            }
            Debug.LogError("Invalid sound path provided!");
            return null;
        }
        Debug.LogError("Invalid sound path provided!");
        return null;
    }

    public MusicClip FindMusic(string musicPath)
    {
        List<string> path = new List<string>(musicPath.Trim().Split("."));
        return FindMusic(musicDatabase, path);
    }

    public MusicClip FindMusic(SoundNode current, List<string> path)
    {
        if (current is MusicClip)
        {
            return ((MusicClip)current);
        }
        else if (current is MusicCategory)
        {
            foreach (SoundNode node in ((MusicCategory)current).children)
            {
                if (path.Count > 0 && node.name.ToLower() == path[0].ToLower())
                {
                    // Debug.Log("Found " + path[0]);
                    current = node;
                    path.RemoveAt(0);
                    return FindMusic(node, path);
                }
            }
            Debug.LogError("Invalid music path provided!");
            return null;
        }
        Debug.LogError("Invalid music path provided!");
        return null;
    }
}