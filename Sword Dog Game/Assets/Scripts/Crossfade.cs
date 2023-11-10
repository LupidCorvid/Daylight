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
    public static bool over = true;


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
            Invoke("EndFade", 0.4f);
    }

    void EndFade()
    {
        animator?.SetFloat("Speed", 1);
    }

    public void StopFade()
    {
        animator?.SetFloat("Speed", 1);
        ChangeScene.changingScene = false;
    }
    public void StartFade()
    {
        if(animator == null)
        {
            current = FindObjectOfType<Crossfade>();
            if(current != this)
                current.StartFade();
        }
        animator?.SetFloat("Speed", -1.2f);
        over = false;
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

    public void Finished()
    {
        over = true;
        if (animator != null && animator.GetFloat("Speed") > 0)
        {
            animator.SetFloat("Speed", 0);
        }        
    }
}
