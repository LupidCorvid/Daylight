using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Set", menuName = "Assets/Sound Set")]
public class SoundSet : SoundPlayable
{
    public List<AudioClip> clips;

    public override AudioClip GetClip()
    {
        return clips[Random.Range(0, clips.Count)];
    }
}