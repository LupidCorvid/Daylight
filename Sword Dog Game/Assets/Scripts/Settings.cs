using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class Settings
{
    //public InputBinding interactKey;
    //public InputBinding openMenuKey;

    //public InputBinding sprintKey;
    //public InputBinding pauseKey;
    
    //JSON data for the inputActions setup
    public string keybinds;
    //Separate settings for controller inputs;
    //public KeyCode interactButton



    public float sfxVolume = 1f;

    public float musicVolume = 1f;

    public bool musicMute = false;

    public bool sfxMute = false;

    public bool fullScreen = true;

    public bool vSync = false;

    public int quality = 1;

    public bool GrassSway = true;

    public enum FontOption
    {
        GOOD_DOG, OPEN_DYSLEXIC
    }
    public FontOption fontFace = FontOption.GOOD_DOG;
    // public float fontSize = ??
}
