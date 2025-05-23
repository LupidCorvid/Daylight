using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static GameObject instance, HUD;
    public static List<Heart> hearts;
    public List<Heart> myHearts;
    public GameObject myHUD;
    public Image backsplash;
    public Sprite[] backsplashSprites;
    private static float HUDscale = 1.0f;
    private static bool instaHide = false, instaShow = false;
    public static bool shownHUD = true;
    public static CanvasManager main;
    public static Coroutine blinkRoutine;

    public Animator newQuestNotif;
    public TMPro.TextMeshProUGUI questNotifText;

    public string queuedQuestNotif;

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
            main = this;
            hearts = myHearts;
            HUD = myHUD;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseScreen.paused)
        {
            HUD = myHUD;
            HUD.transform.localScale = Vector3.Lerp(HUD.transform.localScale, new Vector3(HUDscale, HUDscale, HUDscale), 0.05f);
            if (instaHide)
                HUD.transform.localScale = Vector3.one * HUDscale;
            instaHide = false;

            if (instaShow)
                HUD.transform.localScale = Vector3.one * HUDscale;
            instaShow = false;
        }
    }

    public static void HideHUD(bool instant = false)
    {
        HUDscale = 0;
        shownHUD = false;
        instaHide = instant;
    }

    public static void ShowHUD(bool instant = false)
    {
        HUDscale = 1;
        shownHUD = true;
        instaShow = instant;
    }

    public static void HurtBacksplash(bool lowHealth)
    {
        if (blinkRoutine == null)
            blinkRoutine = main.StartCoroutine(main.BlinkBacksplash(lowHealth ? 2 : 1, 1f, 3));
    }

    private IEnumerator BlinkBacksplash(int index, float duration, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            backsplash.sprite = backsplashSprites[index];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
            backsplash.sprite = backsplashSprites[0];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
        }
        blinkRoutine = null;
    }

    public void Notif(string notifText)
    {
        questNotifText.text = notifText;
        newQuestNotif.Play("NewQuest");
    }

    public void DisplayQueuedNotif()
    {
        if(queuedQuestNotif != "")
            Notif(queuedQuestNotif);
        queuedQuestNotif = "";
    }
}
