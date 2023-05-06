using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sprite Category", menuName = "Assets/Sprite Category")]
public class SpritesRoot : SpriteNode
{
    [SerializeField] public List<SpriteNode> children = new List<SpriteNode>();
}
