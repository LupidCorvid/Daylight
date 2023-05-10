using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Disabled for now
#if false //Replace with UNITY_EDITOR to get it back
using UnityEditor;
[CustomEditor(typeof(Parallax2))]

public class Parallax2Editor : Editor
{
    public Parallax2 script;
    public Color suggestedColor;
    

    public override void OnInspectorGUI()
    {
        script = (Parallax2)target;
        script.simplifiedOptions = EditorGUILayout.Toggle("Simplify parameters", script.simplifiedOptions);
        if(script.simplifiedOptions)
        {
            script.simpleDistance = EditorGUILayout.FloatField("Distance", script.simpleDistance);

            if (script.simpleDistance >= 0)
            {
                script.distance.x = (script.simpleDistance / 5f) * -.05f;
                script.distance.y = (script.simpleDistance / 5f) * .0175f;
            }
            else
            {
                script.distance.x = 0.5f / (script.simpleDistance / 30f + 1f) - .5f;
                script.distance.y = -.0175f / (script.simpleDistance / 30f + 1f) + .0175f;
            }

            script.transform.localScale = new Vector3(1/((script.simpleDistance/30f) + 1), 1/((script.simpleDistance/30f) + 1), 1);
            if (script.simpleDistance > 0)
                suggestedColor = Color.Lerp(new Color(255f/255, 221f/255, 28f/255), Color.black, 1 / ((script.simpleDistance/10 + 1)));
            else
                suggestedColor = Color.black;
            EditorGUILayout.ColorField("Suggested Color", suggestedColor);
            script.autoChangeColor = EditorGUILayout.Toggle("Auto Change Color", script.autoChangeColor);
            if(script.autoChangeColor)
            {
                script.spriteRenderer = EditorGUILayout.ObjectField("Sprite Renderer", script.spriteRenderer, typeof(SpriteRenderer), true) as SpriteRenderer;
                if (script.spriteRenderer != null)
                {
                    script.spriteRenderer.color = suggestedColor;
                    script.spriteRenderer.sortingOrder = -(int)script.simpleDistance;
                }
            }

        }

        GUI.enabled = !script.simplifiedOptions;
        DrawDefaultInspector();
    }

}
#endif