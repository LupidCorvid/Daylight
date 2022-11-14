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

    public override void ApplyEffectToMesh(TMP_TextInfo textMesh)
    {
        int endPoint = end;
        if (endPoint == -1 || endPoint > textMesh.characterCount)
            endPoint = textMesh.characterCount;
        for(int i = start; i < endPoint; i++)
        {
            TMP_CharacterInfo info = textMesh.characterInfo[i];
            int index = info.materialReferenceIndex;
            int vertexIndex = info.vertexIndex;

            float offset = Mathf.Sin(info.vertex_BL.position.x + (Time.time * speed)) * intensity;

            Vector3 offsetVector = new Vector3(0, offset, 0);
            //offsetVector = Vector3.up;
            //textMesh.mesh.vertices[info.vertexIndex].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 1].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 2].y += offset;
            //textMesh.mesh.vertices[info.vertexIndex + 3].y += offset;
            textMesh.meshInfo[index].vertices[vertexIndex + 0] += offsetVector;
            textMesh.meshInfo[index].vertices[vertexIndex + 1] += offsetVector;
            textMesh.meshInfo[index].vertices[vertexIndex + 2] += offsetVector;
            textMesh.meshInfo[index].vertices[vertexIndex + 3] += offsetVector;


        }
        //textMesh.mesh.vertices = vertices;
    }
}
