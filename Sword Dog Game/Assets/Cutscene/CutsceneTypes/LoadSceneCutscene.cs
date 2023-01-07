using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneCutscene : CutsceneData
{
    public string changeToScene;
    public string toNewCutscene;

    private static bool alreadySubscribed;

    public override void startSegment()
    {
        base.startSegment();
        if (!alreadySubscribed)
            SceneHelper.ImbetweenUnloads += CutsceneController.CheckForLoadCutsceneTransition;
        SceneHelper.betweenSceneData.Add(new string[]
            {
                "cutsceneOnLoad",
                toNewCutscene
            });
        ChangeScene.ChangeSceneMinimal(changeToScene);
        alreadySubscribed = true;
    }
}
