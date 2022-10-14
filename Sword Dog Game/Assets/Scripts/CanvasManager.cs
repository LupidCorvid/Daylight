using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static GameObject instance;
    public static List<Heart> hearts;
    public List<Heart> myHearts;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton design pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = gameObject;
            hearts = myHearts;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
