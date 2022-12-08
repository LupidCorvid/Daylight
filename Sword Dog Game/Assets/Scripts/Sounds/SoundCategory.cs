using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Category", menuName = "Assets/Sound Category")]
public class SoundCategory : SoundNode
{
    [SerializeField] public List<SoundNode> children = new List<SoundNode>();
}