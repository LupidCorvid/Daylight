using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(VineController))]
public class VinesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        VineController controller = (VineController)target;
        if(GUILayout.Button("Create vines"))
        {
            controller.loadSegments();
        }
        if(GUILayout.Button("Remove vines"))
        {
            controller.ClearSegments();
        }
        if(GUILayout.Button("Update wind settings"))
        {
            controller.setSegmentWind();
        }
        if(GUILayout.Button("Update sprites"))
        {
            controller.setSegmentSprites();
        }
        if (GUILayout.Button("Update Movement Reaction Scalar"))
        {
            controller.UpdateSegmentScalars();
        }

    }
}
#endif
