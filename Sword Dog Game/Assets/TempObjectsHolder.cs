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
    // Start is called before the first frame update
    void Awake()
    {
        holder = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
