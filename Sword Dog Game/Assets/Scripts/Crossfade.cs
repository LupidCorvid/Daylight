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
    public static bool over = false;
    public static bool waiting = false;
    public float speed = 1;

    void Start()
    {
        current = this;
        changeScene += SceneChange;
        animator = GetComponent<Animator>();
        FadeStart += FadeOut;
        FadeEnd += FadeIn;
    }

    void Update()
    {
        // Fixes deadlock
        if (animator.GetFloat("Speed") < 0 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
        {
            animator.SetFloat("Speed", 0);
            animator.Play("Crossfade", 0, 0);
        }
    }

    private void SceneChange()
    {
        if (this != null)
            Invoke("EndFade", 0.4f);
    }

    void EndFade()
    {
        animator?.SetFloat("Speed", 1 * speed);
    }

    void NotWaiting()
    {
        if (animator.GetFloat("Speed") >= 0)
        {
            ChangeScene.changingScene = false;
            waiting = false;
        }
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
        animator?.SetFloat("Speed", -1f * speed);
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
        if (animator != null && animator.GetFloat("Speed") > 0)
        {
            over = true;
            animator.SetFloat("Speed", 0);
            speed = 1;
        }        
    }
}
