using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(CutsceneController))]
public class CutsceneCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CutsceneController controller = (CutsceneController)target;
        
        foreach(CutsceneData data in controller.cutscenes)
        {
            
        }


        if (GUILayout.Button("Add new of type"))
        {
            controller.addNewCutscene();
        }
    }
}
#endif