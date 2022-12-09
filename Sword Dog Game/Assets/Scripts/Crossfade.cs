using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    public static Action changeScene;

    void Start()
    {
        changeScene += SceneChange;
    }

    private void SceneChange()
    {
        if (this != null)
            Invoke("EndFade", 0.2f);
    }

    void EndFade()
    {
        GetComponent<Animator>().SetTrigger("stop");
        ChangeScene.changingScene = false;
    }
}
