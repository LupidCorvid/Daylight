using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Set", menuName = "Assets/Sound Set")]
public class SoundSet : SoundPlayable
{
    public List<AudioClip> clips;

    public override AudioClip GetClip()
    {
        int index = Random.Range(0, clips.Count);
        Debug.Log(index + " " + clips.Count);
        return clips[index];
    }
}