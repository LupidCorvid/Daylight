using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempObjectsHolder : MonoBehaviour
{
    public static GameObject holder;
    public static Transform asTransform
    {
        get
        {
            return holder.transform;
        }
    }

    public static TempObjectsHolder main;
    //Loose prefabs that are necessary
    public GameObject sporedDebuffPrefab;

    public GameObject buffIconDisplay;

    public SpritesRoot sprites;

    public Sprite FindSprite(string soundPath)
    {
        List<string> path = new List<string>(soundPath.Trim().Split("."));
        return FindSprite(sprites, path);
    }

    public Sprite FindSprite(SpriteNode current, List<string> path)
    {
        if (current is SpriteEntry)
        {
            return ((SpriteEntry)current).GetEntry();
        }
        else if (current is SpritesRoot)
        {
            foreach (SpriteNode node in ((SpritesRoot)current).children)
            {
                if (path.Count > 0 && node.name.ToLower() == path[0].ToLower())
                {
                    // Debug.Log("Found " + path[0]);
                    current = node;
                    path.RemoveAt(0);
                    return FindSprite(node, path);
                }
            }
            Debug.LogError("Invalid sprite path provided!");
            return null;
        }
        Debug.LogError("Invalid sprite path provided!");
        return null;
    }

    // Start is called before the first frame update
    void Awake()
    {
        holder = gameObject;
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
