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

    int numSegments;

    public Collider2D cldr;

    WaterSegment[] WaveOffset;

    public float motionResponseDampener = 4;

    // Start is called before the first frame update
    void Start()
    {
        meshProperty = GetComponent<MeshFilter>();
        mesh = meshProperty.mesh;

        if(cldr != null)
        {
            size = cldr.bounds.size;
        }

        SetUp();
        
    }

    public void SetUp()
    {
        BuildMesh();
        WaveOffset = new WaterSegment[numSegments + 1];
        for(int i = 0; i < WaveOffset.Length; i++)
        {
            WaveOffset[i] = new WaterSegment();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < WaveOffset.Length; i++)
        {
            WaveOffset[i].waveVel += (Mathf.Sin((Time.time + i * vertexDistance))) * Time.deltaTime * 2;

            WaveOffset[i].UpdateSegment();
        }


        ApplyAdjacencyEffects();
        ApplyWaveHeights();
        

    }

    public void ApplyWaveHeights()
    {
        Vector3[] newVerts = mesh.vertices;
        for(int i = 0; i < WaveOffset.Length; i++)
        {
            
            newVerts[2 * i] = new Vector3(newVerts[2 * i].x, (WaveOffset[i].wavePosition + size.y / 2));
        }

        mesh.vertices = newVerts;
    }

    public void BuildMesh()
    {
        mesh.MarkDynamic();
        numSegments = (int)(size.x / vertexDistance);

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
            tris[i] = j + 1;           //02
            tris[i + 1] = j;   //1
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

    public void ApplyAdjacencyEffects()
    {
        
        List<int> order = new List<int>();

        for(int i = 0; i < WaveOffset.Length; i++)
        {
            order.Add(i);
        }

        while(order.Count > 0)
        {
            int selectedItem = Random.Range(0, order.Count);

            WaterSegment chosenWave = WaveOffset[order[selectedItem]];
            if (order[selectedItem] - 1 >= 0)
                WaveOffset[order[selectedItem] - 1].waveVel += WaveOffset[order[selectedItem] - 1].tension * (-(WaveOffset[order[selectedItem] - 1].wavePosition - chosenWave.wavePosition)) - WaveOffset[order[selectedItem] - 1].waveVel * WaveOffset[order[selectedItem] - 1].dampening;
            if (order[selectedItem] + 1 < WaveOffset.Length)
                WaveOffset[order[selectedItem] + 1].waveVel += WaveOffset[order[selectedItem] + 1].tension * (-(WaveOffset[order[selectedItem] + 1].wavePosition - chosenWave.wavePosition)) - WaveOffset[order[selectedItem] + 1].waveVel * WaveOffset[order[selectedItem] + 1].dampening;

            order.RemoveAt(selectedItem);
        }


    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("DamageArea") || Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("Utility"))
            return;

        float relPosition = collision.transform.position.x - (transform.position.x - (size.x)/2);

        float currPos = 0;
        for(int i = 0; i < WaveOffset.Length; i++)
        {
            if (Mathf.Abs(currPos - relPosition) < collision.bounds.extents.x)
                WaveOffset[i].waveVel += collision.attachedRigidbody.velocity.y * (1 - Mathf.Abs(currPos - relPosition)/collision.bounds.extents.x) / motionResponseDampener;

            currPos += vertexDistance;
        }
        
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("DamageArea") || Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("Utility"))
            return;

        float relPosition = collision.transform.position.x - (transform.position.x - (size.x) / 2);

        float currPos = 0;
        for (int i = 0; i < WaveOffset.Length; i++)
        {
            if (Mathf.Abs(currPos - relPosition) < collision.bounds.extents.x)
                WaveOffset[i].waveVel += collision.attachedRigidbody.velocity.y * (1 - Mathf.Abs(currPos - relPosition) / collision.bounds.extents.x) / motionResponseDampener;

            currPos += vertexDistance;
        }
        
    }

    public class WaterSegment
    {
        public float wavePosition;
        public float waveVel;

        public float tension = .7f;
        public float dampening = 0.01f;


        public WaterSegment()
        {

        }

        public void UpdateSegment()
        {
            wavePosition += waveVel * Time.deltaTime;
            waveVel += tension * (-wavePosition) - waveVel * dampening;
        }
    }
}
