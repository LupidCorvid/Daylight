using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchParallax : MonoBehaviour
{
    public Vector2 distance = new Vector2(0, 0);
    public Camera cam;
    //public Vector3 startPos;

    public bool changeOnZoom = true;
    public bool onlyChangeOnGreaterZoom = true;


    Mesh originalMesh;
    MeshFilter meshFilter;
    Mesh.MeshDataArray originalSnapshot;
    Unity.Collections.NativeArray<Vector3> originalVertices;

    public MaterialPropertyBlock materialBlock;

    public Sprite texture;
    public Color color = new Color(0, 0, 0, 1);

    public Vector3 scalar = new Vector3(1,1,1);
    public Vector3 rotation = new Vector3();

    public float distanceOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam ??= Camera.main;
        //startPos = transform.position;

        //rend = GetComponent<Renderer>();

        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.sharedMesh; 
        //originalSnapshot = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh)[0].GetVertices();

        originalVertices = new Unity.Collections.NativeArray<Vector3>(originalMesh.vertexCount, Unity.Collections.Allocator.Persistent);

        originalSnapshot = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        originalSnapshot[0].GetVertices(originalVertices);



        meshFilter.sharedMesh = new Mesh();
        Mesh.ApplyAndDisposeWritableMeshData(originalSnapshot, meshFilter.sharedMesh);


    }

    public void OnValidate()
    {
        materialBlock = new MaterialPropertyBlock();
        materialBlock.SetTexture("_MainTex", texture.texture);
        materialBlock.SetColor("_Color", color);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialBlock);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        if (cam == null)
            cam = Camera.main;

        
        Vector3[] vertices = new Vector3[originalVertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldSpace = (Quaternion.Euler(rotation) * Vector3.Scale(originalVertices[i], scalar)) + transform.position;
            float depth = (originalVertices[i].z) * scalar.z + distanceOffset;

            if (changeOnZoom && (!onlyChangeOnGreaterZoom || cam.orthographicSize > 5))
                vertices[i] = new Vector3(worldSpace.x + ((worldSpace.x - cam.transform.position.x) * depth * 5f / cam.orthographicSize), worldSpace.y + ((worldSpace.y - cam.transform.position.y) * depth/5f * 5f / cam.orthographicSize), transform.position.z) - transform.position;
            else
                vertices[i] = new Vector3(worldSpace.x + ((worldSpace.x - cam.transform.position.x) * depth), worldSpace.y + ((worldSpace.y - cam.transform.position.y) * depth / 5f), transform.position.z) - transform.position;
        }

        meshFilter.sharedMesh.SetVertices(vertices);
        meshFilter.sharedMesh.RecalculateBounds();
    }
}
