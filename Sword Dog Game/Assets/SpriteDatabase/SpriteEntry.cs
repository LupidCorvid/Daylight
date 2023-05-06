using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sprite Entry", menuName = "Assets/Sprite Entry")]
public class SpriteEntry : SpriteNode
{
    public Sprite entry;

    public Sprite GetEntry()
    {
        return entry;
    }
}