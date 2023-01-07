using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(CutsceneController))]
public class CutsceneCustomInspector : Editor
{
    public CutsceneController controller;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        controller = (CutsceneController)target;
        if (GUILayout.Button("Auto Fill"))
            controller.FillListFromComponents();
            
    }
}
#endif