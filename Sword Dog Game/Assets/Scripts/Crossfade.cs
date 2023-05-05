using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    public static Action changeScene;
    private Animator animator;

    public static Crossfade current;

    public static Action FadeStart;
    public static Action FadeEnd;


    void Start()
    {
        current = this;
        changeScene += SceneChange;
        animator = GetComponent<Animator>();
        FadeStart += FadeOut;
        FadeEnd += FadeIn;
    }

    private void SceneChange()
    {
        if (this != null)
            Invoke("EndFade", 0.2f);
    }

    void EndFade()
    {
        animator?.SetTrigger("stop");
        ChangeScene.changingScene = false;
    }

    public void StopFade()
    {
        animator?.SetTrigger("stop");
    }
    public void StartFade()
    {
        if(animator == null)
        {
            current = GameObject.FindObjectOfType<Crossfade>();
            if(current != this)
                current.StartFade();
        }
        else
            animator?.SetTrigger("start");
    }

    public void FadeOut()
    {
        if (this != null)
            StartFade();
    }

    public void FadeIn()
    {
        if (this != null)
            EndFade();
    }

}
