using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static GameObject instance, healthBar;
    public static List<Heart> hearts;
    public List<Heart> myHearts;
    public GameObject myHealthBar;
    private static float healthBarScale = 1.0f;

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
            healthBar = myHealthBar;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.localScale = Vector3.Lerp(healthBar.transform.localScale, new Vector3(healthBarScale, healthBarScale, healthBarScale), 0.05f);
    }

    public static void HideHealth()
    {
        healthBarScale = 0.0f;
    }

    public static void ShowHealth()
    {
        healthBarScale = 1.0f;
    }
}
