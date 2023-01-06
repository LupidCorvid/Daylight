using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    public static Action changeScene;
    private Animator animator;

    public static Crossfade current;

    void Start()
    {
        current = this;
        changeScene += SceneChange;
        animator = GetComponent<Animator>();
    }

    private void SceneChange()
    {
        if (this != null)
            Invoke("EndFade", 0.2f);
    }

    void EndFade()
    {
        animator.SetTrigger("stop");
        ChangeScene.changingScene = false;
    }

    public void StopFade()
    {
        animator.SetTrigger("stop");
    }
    public void StartFade()
    {
        animator.SetTrigger("start");
    }
}
