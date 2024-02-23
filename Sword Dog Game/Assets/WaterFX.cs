using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFX : MonoBehaviour
{
    public Vector2 size = default;
    //Have a vertex how often? Default is 1 vertex/unit
    public float vertexDistance = 1;

    Mesh.MeshDataArray UnmodifiedMesh;
    Mesh mesh;
    MeshFilter meshProperty;

    // Start is called before the first frame update
    void Start()
    {
        meshProperty = GetComponent<MeshFilter>();
        mesh = meshProperty.mesh;

        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] newVerts = mesh.vertices;

        for(int i = 0; i < newVerts.Length; i += 2)
        {
            newVerts[i] = new Vector3(mesh.vertices[i].x, size.y / 2 + Mathf.Sin((Time.time * 3 + i * vertexDistance)), 0);
        }

        mesh.vertices = newVerts;
    }

    public void BuildMesh()
    {

        int numSegments = (int)(size.x / vertexDistance);

        if(numSegments < 0)
        {
            return;
        }

        Vector3[] newVerts = new Vector3[numSegments * 2 + 2];

        for(int i = 0; i <= numSegments; i++)
        {
            newVerts[2 * i] = new Vector3(i * vertexDistance - size.x/2, size.y / 2, 0);
            newVerts[2 * i + 1] = new Vector3(i * vertexDistance - size.x/2, -size.y / 2, 0);
            //Odds are lower area, evens are upper
        }

        int[] tris = new int[numSegments * 6];

        int j = 0;
        for(int i = 0; i + 6 <= tris.Length; i += 6)
        {
            tris[i] = j;           //02
            tris[i + 1] = j + 1;   //1
            tris[i + 2] = j + 2;   

            tris[i + 3] = j + 1;   // 2
            tris[i + 4] = j + 2;   //13
            tris[i + 5] = j + 3;
            j += 2;
        }



        Vector2[] uvs = new Vector2[newVerts.Length];

        for(int i = 0; i < newVerts.Length; i += 2)
        {
            uvs[i] = new Vector2(newVerts[i].x, newVerts[i].z);
            //uvs[i + 1] = new Vector2(newVerts[i].x, newVerts[i].z);
        }

        mesh.vertices = newVerts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        Debug.Log("Num segments: " + numSegments);
        Debug.Log("Verts: " + mesh.vertices.Length);
        
    }
}
