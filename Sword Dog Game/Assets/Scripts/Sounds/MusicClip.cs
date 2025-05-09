using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Clip", menuName = "Assets/Music Clip")]
public class MusicClip : SoundPlayable
{
    public AudioClip clip;
    public int BPM, timeSignature, timeSignatureBottom = 4, barsLength, repeatBar;
    public AudioManager.GameArea area;
    public bool disableSceneFade = false;
    public override AudioClip GetClip()
    {
        return clip;
    }
    public float length()
    {
        return clip.length;
    }
}
