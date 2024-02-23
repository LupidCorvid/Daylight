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

    public OnOffSet grassSway;

    public OnOffSet altFont;

    public static SettingsMiniMenu main;

    // Start is called before the first frame update
    void Start()
    {
        main ??= this;
        musicSetting.changed += MusicSettingChanged;
        sfxSetting.changed += SfxSettingChanged;

        qualitySetting.changed += qualitySettingChanged;

        fullScreen.toggled += fullScreenToggled;
        vSync.toggled += vSyncToggled;

        grassSway.toggled += grassToggle;

        altFont.toggled += altFontToggled;
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
        SettingsManager.currentSettings.fullScreen = fullScreen.state;
        if (SettingsManager.currentSettings.fullScreen)
        {
            SettingsManager.currentSettings.xRes = (float)Screen.width / Display.main.systemWidth;
            SettingsManager.currentSettings.yRes = (float)Screen.height / Display.main.systemHeight;
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
        else
        {
            SettingsManager.currentSettings.xRes = Mathf.Clamp(SettingsManager.currentSettings.xRes, 0.1f, 1.0f);
            SettingsManager.currentSettings.yRes = Mathf.Clamp(SettingsManager.currentSettings.yRes, 4 / 9f, 1.0f);
            Screen.SetResolution((int)(SettingsManager.currentSettings.xRes * Display.main.systemWidth), (int)(SettingsManager.currentSettings.yRes * Display.main.systemHeight), false);
            Screen.SetResolution((int)(SettingsManager.currentSettings.xRes * Display.main.systemWidth), (int)(SettingsManager.currentSettings.yRes * Display.main.systemHeight), false);
        }
    }

    public void vSyncToggled()
    {
        if (vSync.state)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;

        SettingsManager.currentSettings.vSync = vSync.state;
    }

    public void grassToggle()
    {
        SettingsManager.currentSettings.GrassSway = grassSway.state;
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
        musicSetting.SetValue(Mathf.Pow(10, SettingsManager.currentSettings.musicVolume / 20) - 0.00001f);
        sfxSetting.SetValue(Mathf.Pow(10, SettingsManager.currentSettings.sfxVolume / 20) - 0.00001f);
        qualitySetting.SetValue(SettingsManager.currentSettings.quality);

        fullScreen.Set(SettingsManager.currentSettings.fullScreen);
        vSync.Set(SettingsManager.currentSettings.vSync);
        grassSway.Set(SettingsManager.currentSettings.GrassSway);
        if(SettingsManager.currentSettings.fontFace == Settings.FontOption.OPEN_DYSLEXIC)
            altFont.Set(true);
        else
            altFont.Set(false);

        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void altFontToggled()
    {
        if (altFont.state)
            SettingsManager.currentSettings.fontFace = Settings.FontOption.OPEN_DYSLEXIC;
        else
            SettingsManager.currentSettings.fontFace = Settings.FontOption.GOOD_DOG;

        FontManager.main.current = SettingsManager.currentSettings.fontFace;
    }

    public void CloseMenu()
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;

        SettingsManager.SaveSettings();

        if (PauseScreen.canPause)
        {
            PauseScreen.main.ShowMenu();
        }
        else
            PauseScreen.main.CloseMenuContainer();
    }
}
