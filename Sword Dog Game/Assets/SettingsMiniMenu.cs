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
        
    }

    public void SfxSettingChanged()
    {

    }

    public void qualitySettingChanged()
    {

    }

    public void fullScreenToggled()
    {

    }

    public void vSyncToggled()
    {

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
        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void CloseMenu()
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
}
