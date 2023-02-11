using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RenderLayerSetter))]
public class RenderLayerSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RenderLayerSetter controller = (RenderLayerSetter)target;
        if (GUILayout.Button("Change layers"))
        {
            controller.setAllLayers();
        }

    }
}
#endif
