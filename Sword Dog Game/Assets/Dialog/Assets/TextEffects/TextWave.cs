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

    public override void ApplyEffectToMesh(TMP_Text textMesh)
    {
        int endPoint = end;
        if (endPoint == -1 || endPoint > textMesh.textInfo.characterCount)
            endPoint = textMesh.textInfo.characterCount;
        Vector3[] vertices = new Vector3[textMesh.mesh.vertices.Length];
        for(int i = start; i < endPoint; i++)
        {
            TMP_CharacterInfo info = textMesh.textInfo.characterInfo[i];

            float offset = Mathf.Sin(info.vertex_BL.position.x + (Time.time * speed)) * intensity;

            Vector3 offsetVector = new Vector3(0, offset, 0);
            //textMesh.mesh.vertices[info.vertexIndex].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 1].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 2].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 3].y += offset;

            vertices[info.vertexIndex] = offsetVector + textMesh.mesh.vertices[info.vertexIndex];
            vertices[info.vertexIndex + 1] = offsetVector + textMesh.mesh.vertices[info.vertexIndex + 1];
            vertices[info.vertexIndex + 2] = offsetVector + textMesh.mesh.vertices[info.vertexIndex + 2];
            vertices[info.vertexIndex + 3] = offsetVector + textMesh.mesh.vertices[info.vertexIndex + 3];

        }
        textMesh.mesh.vertices = vertices;
    }
}
