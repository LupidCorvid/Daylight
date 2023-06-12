using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public KeyCode interactKey = KeyCode.T;
    public KeyCode openMenuKey = KeyCode.U;

    public KeyCode sprintKey = KeyCode.LeftShift;

    public KeyCode pauseKey = KeyCode.Escape;

    //Separate settings for controller inputs;
    //public KeyCode interactButton



    public float sfxVolume = 1f;

    public float musicVolume = 1f;

    public bool musicMute = false;

    public bool sfxMute = false;

    public bool fullScreen = true;
}
