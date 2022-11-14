using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWave : TextEffect
{
    public float intensity = 1;
    public float speed = 1;

    public TextWave(float intensity, float speed)
    {
        this.intensity = intensity;
        this.speed = speed;
    }

    public override void ApplyEffectToMesh(in TMP_Text textMesh)
    {
        int endPoint = end;
        if (end == -1 || end > textMesh.textInfo.characterCount)
            end = textMesh.textInfo.characterCount;

        for(int i = start; i < end; i++)
        {
            TMP_CharacterInfo info = textMesh.textInfo.characterInfo[i];

            float offset = Mathf.Sin(info.bottomLeft.x + (Time.time * speed)) * intensity;
        }
    }
}
