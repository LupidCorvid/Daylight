using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMiniMenu : MonoBehaviour
{
    public CanvasGroup group;


    public DotSlider musicSetting;
    public DotSlider sfxSetting;

    public DotSlider qualitySetting;

    public OnOffSet fullScreen;
    public OnOffSet vSync;

    // Start is called before the first frame update
    void Start()
    {

        musicSetting.changed += MusicSettingChanged;
        sfxSetting.changed += SfxSettingChanged;

        qualitySetting.changed += qualitySettingChanged;

        fullScreen.toggled += fullScreenToggled;
        vSync.toggled += vSyncToggled;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MusicSettingChanged()
    {
        //SettingsManager.currentSettings.musicVolume = musicSetting.value;
        //AudioManager.instance.musicVolume = musicSetting.value;
        AudioManager.instance.musicVolume = Mathf.Log10(musicSetting.value + 0.00001f) * 20;
        //Debug.Log("Check equal: " + (Mathf.Abs(musicSetting.value - Mathf.Pow(10, (SettingsManager.currentSettings.musicVolume / 20)) - 0.00001f) < .001f));
        //Debug.Log("Real: " + musicSetting.value + "Calced: " + (Mathf.Pow(10, (SettingsManager.currentSettings.musicVolume / 20)) - 0.00001f));
    }

    public void SfxSettingChanged()
    {
        //SettingsManager.currentSettings.sfxVolume = sfxSetting.value;
        //AudioManager.instance.sfxVolume = sfxSetting.value;
        AudioManager.instance.sfxVolume = Mathf.Log10(sfxSetting.value + 0.00001f) * 20;
    }

    public void qualitySettingChanged()
    {
        QualitySettings.SetQualityLevel(qualitySetting.dotNum);
        SettingsManager.currentSettings.quality = qualitySetting.dotNum;
    }

    public void fullScreenToggled()
    {
        //Screen.fullScreen = fullScreen.state;
        if (SettingsManager.currentSettings.fullScreen)
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
        SettingsManager.currentSettings.fullScreen = fullScreen.state;
    }

    public void vSyncToggled()
    {
        if (vSync.state)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;

        SettingsManager.currentSettings.vSync = vSync.state;
    }

    private void OnDestroy()
    {
        if(musicSetting != null)
        {
            musicSetting.changed -= MusicSettingChanged;
        }
    }

    public void OpenMenu()
    {
        musicSetting.SetValue(Mathf.Pow(10, (SettingsManager.currentSettings.musicVolume / 20)) - 0.00001f);
        sfxSetting.SetValue(Mathf.Pow(10, (SettingsManager.currentSettings.sfxVolume / 20)) - 0.00001f);
        qualitySetting.SetValue(SettingsManager.currentSettings.quality);

        fullScreen.Set(SettingsManager.currentSettings.fullScreen);
        vSync.Set(SettingsManager.currentSettings.vSync);

        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void CloseMenu()
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;

        SettingsManager.SaveSettings();
    }
}
