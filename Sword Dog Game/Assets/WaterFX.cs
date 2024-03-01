using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFX : MonoBehaviour
{
    public Vector2 size = default;
    //Have a vertex how often? Default is 1 vertex/unit
    public float vertexDistance = 1;

    Mesh.MeshDataArray UnmodifiedMesh;
    public Mesh mesh;
    public MeshFilter meshProperty;

    int numSegments;

    public Collider2D cldr;

    WaterSegment[] WaveOffset;

    public float motionResponseDampener = 4;

    public float windSpeed = 1;
    public float windVolatility = 1;
    public float windStrength = 1;

    public Sprite sprite;
    public Color color = Color.white;

    public float buoyantForce = 1;
    public float depthBuoyanceScalar = 1;

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

    public void OnValidate()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        if(sprite != null)
            materialBlock.SetTexture("_MainTex", sprite.texture);
        materialBlock.SetColor("_Color", color);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialBlock);
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
            //WaveOffset[i].waveVel += (Mathf.Sin((Time.time + i * vertexDistance))) * Time.deltaTime * 2;

            WaveOffset[i].waveVel += (SwayEffect.getWindEffect((transform.position.x - size.x/2) + i * vertexDistance, windSpeed, windVolatility, windStrength, true)) * 100;

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
        Vector2[] uvs = new Vector2[newVerts.Length];

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





        for (int i = 0; i < newVerts.Length; i += 2)
        {
            //uvs[i] = new Vector2(newVerts[i].x, newVerts[i].z);
            uvs[i] = new Vector2((vertexDistance * i / 2)/size.x, 1);
            uvs[i + 1] = new Vector2((vertexDistance * i / 2)/size.x, 0);
        }

        mesh.vertices = newVerts;
        mesh.triangles = tris;
        mesh.SetUVs(0, uvs);

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

        //Slow speed on hitting surface of water
        if (collision.attachedRigidbody != null)
        {
            collision.attachedRigidbody.velocity *= .75f;
            //if(clampMaxEnterSpeed)
            //    collision.attachedRigidbody.velocity = new Vector2(collision.attachedRigidbody.velocity.x, Mathf.Clamp(collision.attachedRigidbody.velocity.y, -25, 1000));
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("DamageArea") || Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("Utility"))
            return;

        if (collision.attachedRigidbody != null)
        {
            //Buoyany force attempts

            //collision.attachedRigidbody.AddForce(Vector2.up * (collision.transform.position.y - (transform.position.y + size.y)) * buoyantForce * Time.deltaTime);
            //collision.attachedRigidbody.velocity += (Vector2.down * (collision.transform.position.y - (transform.position.y + size.y/2)) * buoyantForce * Time.fixedDeltaTime);
            //collision.attachedRigidbody.velocity += (Vector2.up * ((transform.position.y + size.y/2) - collision.transform.position.y) * buoyantForce * Time.fixedDeltaTime * (collision.bounds.size.x * collision.bounds.size.x)/collision.attachedRigidbody.mass);

            //collision.attachedRigidbody.velocity += ((Vector2.down * Time.deltaTime * collision.attachedRigidbody.gravityScale * Physics.gravity) * Mathf.Clamp01(((transform.position.y + size.y / 2) - collision.transform.position.y) * 2/collision.bounds.extents.y));
            //collision.attachedRigidbody.velocity += ((Vector2.down * Time.deltaTime * collision.attachedRigidbody.gravityScale * Physics.gravity) * Mathf.Clamp01(((transform.position.y + size.y / 2) - collision.transform.position.y) * 2 / collision.bounds.extents.y));
            //collision.attachedRigidbody.AddForce((Vector2.down * Time.deltaTime * collision.attachedRigidbody.gravityScale * Physics.gravity) * Mathf.Clamp01(((transform.position.y + size.y / 2) - collision.transform.position.y) * 2 / collision.bounds.extents.y) * collision.bounds.size.x * collision.bounds.size.y * buoyantForce);
            //collision.attachedRigidbody.velocity *= (.999f);

            //if(collision.attachedRigidbody.velocity.y < Physics.gravity.y * -1.5f)
            //    collision.attachedRigidbody.velocity += ((Vector2.down * Time.deltaTime * collision.attachedRigidbody.gravityScale * Physics.gravity) * Mathf.Clamp01(((transform.position.y + size.y / 2) - collision.transform.position.y) * 2 / collision.bounds.extents.y));

            collision.attachedRigidbody.velocity += Vector2.down * Physics.gravity.y / 2 * Time.deltaTime * collision.attachedRigidbody.gravityScale;
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
