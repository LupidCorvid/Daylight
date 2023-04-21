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

    [System.Serializable]
    public class SpriteCollection
    {
        public Sprite movementBuff;
        public Sprite movementDebuff;

        public Sprite attackBuff;
        public Sprite attackDebuff;

        public Sprite defenseBuff;
        public Sprite defenseDebuff;

        public Sprite confused;
        public Sprite poisoned;

        public Sprite spored;
    }

    public SpriteCollection sprites;

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
