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
        
    }
}
#endif