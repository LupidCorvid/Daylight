using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static GameObject instance, HUD;
    public static List<Heart> hearts;
    public List<Heart> myHearts;
    public GameObject myHUD;
    private static float HUDscale = 1.0f;
    private static bool instaHide = false;

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
            HUD = myHUD;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HUD = myHUD;
        HUD.transform.localScale = Vector3.Lerp(HUD.transform.localScale, new Vector3(HUDscale, HUDscale, HUDscale), 0.05f);
        if (instaHide)
            HUD.transform.localScale = Vector3.one * HUDscale;
        instaHide = false;
    }

    public static void HideHUD()
    {
        HUDscale = 0.0f;
    }

    public static void InstantHideHud()
    {
        HUDscale = 0;
        instaHide = true;
    }

    public static void ShowHUD()
    {
        HUDscale = 1.0f;
    }
}
