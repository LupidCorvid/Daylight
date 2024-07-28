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

    public GameObject splashPrefab;

    public float LightRayDepth = 0.95f;
    public float surfaceWidth = 0.985f;


    public float zWidth = 1;
    public float zStartDist = .5f;

    public Camera cam;
    public bool changeOnZoom = true;
    public bool onlyChangeOnGreaterZoom = true;

    public float distantWavesScalar = 1;
    public float distantWavesVolatilityScalar = 1;
    public float distantWavesFalloffDistance = 1;
    public float distantWavesSpeedScalar = 1;

    public Vector2 parallaxFXScalar = new Vector2(1, 1);

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

        cam ??= Camera.main;
    }

    public void OnValidate()
    {
        MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
        if(sprite != null)
            materialBlock.SetTexture("_MainTex", sprite.texture);
        materialBlock.SetColor("_Color", color);
        materialBlock.SetFloat("_LightRayDepth", LightRayDepth);
        materialBlock.SetFloat("_SurfaceWidth", surfaceWidth);
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
        if (cam == null)
            cam = Camera.main;

        for (int i = 0; i < WaveOffset.Length; i++)
        {
            //WaveOffset[i].waveVel += (Mathf.Sin((Time.time + i * vertexDistance))) * Time.deltaTime * 2;

            WaveOffset[i].waveVel += (SwayEffect.getWindEffect((transform.position.x - size.x/2) + i * vertexDistance, windSpeed, windVolatility, windStrength, true)) * 100;
            CheckForSpawnParticle(WaveOffset[i], i);

            WaveOffset[i].UpdateSegment();
        }


        ApplyAdjacencyEffects();
        ApplyWaveHeights();
        ApplyParallaxEffect();
        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();


    }

    public void CheckForSpawnParticle(WaterSegment segment, int index)
    {
        float acceleration = segment.tension * (-segment.wavePosition) - segment.waveVel * segment.dampening;

        if(segment.waveVel + acceleration < 0 && segment.waveVel > 0 && acceleration < -0.6f)
        {
            GameObject addedObj = Instantiate(splashPrefab, new Vector3(transform.position.x - size.x / 2 + (index * vertexDistance), transform.position.y + size.y / 2 + segment.wavePosition), transform.rotation, transform);

            //Make water droplets quantity and size vary with intensity of the splash?
            //make only the centermost peak spawn the droplet set?
            //  ^\ if not doing this, could instead make the droplets be launched in the direction of the normal of the line formed
            //     by the current segment and the height of the ones next to it
            //NOTE: can also just get the normal data from the mesh (though this may be pointing into z axis)

            Vector2 adjacentLeftNormal = new Vector2();
            if(index > 1)
            {
                adjacentLeftNormal = new Vector2(WaveOffset[index - 1].wavePosition - segment.wavePosition, vertexDistance);
            }
            Vector2 adjacentRightNormal = new Vector2();
            if (index < WaveOffset.Length - 1)
            {
                adjacentRightNormal = new Vector2(segment.wavePosition- WaveOffset[index + 1].wavePosition, vertexDistance);
            }

            Vector2 average = (adjacentLeftNormal + adjacentRightNormal) / 2;
            Vector3 worldPos = new Vector3(transform.position.x - size.x / 2 + (index * vertexDistance), transform.position.y + size.y / 2 + segment.wavePosition);
            
            Debug.DrawLine(worldPos, (worldPos + (Vector3)average * 5), Color.magenta);

            addedObj.GetComponent<Rigidbody2D>().velocity = average * 5;
            addedObj.transform.localScale *= Mathf.Clamp(average.magnitude, 0.5f, 2);
            addedObj.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a * 1.2f) * .6f;


        }
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

        Vector3[] newVerts = new Vector3[(numSegments * 2 + 2)*2];

        //Forward facing section
        for (int i = 0; i <= numSegments; i++)
        {
            newVerts[2 * i] = new Vector3(i * vertexDistance - size.x/2, size.y / 2, 0);
            newVerts[2 * i + 1] = new Vector3(i * vertexDistance - size.x/2, -size.y / 2, 0);
            //Odds are lower area, evens are upper
        }

        int[] tris = new int[(numSegments * 6) * 3];
        Vector2[] uvs = new Vector2[newVerts.Length];

        int j = 0;
        for(int i = 0; i + 6 <= tris.Length/3; i += 6)
        {
            tris[i] = j + 1;           //02
            tris[i + 1] = j;   //1
            tris[i + 2] = j + 2;   

            tris[i + 3] = j + 1;   // 2
            tris[i + 4] = j + 2;   //13
            tris[i + 5] = j + 3;
            j += 2;
        }


        for (int i = 0; i < newVerts.Length/2; i += 2)
        {
            //uvs[i] = new Vector2(newVerts[i].x, newVerts[i].z);
            uvs[i] = new Vector2((vertexDistance * i / 2)/size.x, 1);
            uvs[i + 1] = new Vector2((vertexDistance * i / 2)/size.x, 0);
        }


        //Top Section
        //----------------------------------------------
        int topIndexStart = (numSegments * 2 + 2);
        for (int i = 0; i <= numSegments; i++)
        {
            newVerts[topIndexStart + 2 * i] = new Vector3(i * vertexDistance - size.x / 2, size.y/2, zWidth);
            newVerts[topIndexStart + 2 * i + 1] = new Vector3(i * vertexDistance - size.x / 2, size.y/2, zStartDist);
            //Odds are lower area, evens are upper
        }

        j = topIndexStart;
        for (int i = tris.Length/3; i + 6 <= tris.Length * 2 / 3; i += 6)
        {
            tris[i] = j + 1;           //02
            tris[i + 1] = j;   //1
            tris[i + 2] = j + 2;

            tris[i + 3] = j + 1;   // 2
            tris[i + 4] = j + 2;   //13
            tris[i + 5] = j + 3;
            j += 2;
        }

        for (int i = newVerts.Length / 2; i < newVerts.Length; i += 2)
        {
            //uvs[i] = new Vector2(newVerts[i].x, newVerts[i].z);
            uvs[i] = new Vector2((vertexDistance * i / 2) / size.x, 1);
            uvs[i + 1] = new Vector2((vertexDistance * i / 2) / size.x, 1);
        }


        j = 0;
        for (int i = tris.Length * 2 / 3; i + 6 <= tris.Length; i += 6)
        {
            tris[i] = topIndexStart + j + 1;   //02
            tris[i + 1] = j;   //1
            tris[i + 2] = j + 2;

            tris[i + 3] = topIndexStart + j + 1;   // 2
            tris[i + 4] = j + 2;   //13
            tris[i + 5] = topIndexStart + j + 3;
            j += 2;
        }



        mesh.vertices = newVerts;
        mesh.triangles = tris;
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        //Debug.Log("Num segments: " + numSegments);
        //Debug.Log("Verts: " + mesh.vertices.Length);
        
    }


    public void ApplyParallaxEffect()
    {

        if (cam == null)
            return;

        //Apply parallaxToTopSection
        Vector3[] newVerts = new Vector3[mesh.vertexCount];

        for(int i = 0; i < (numSegments * 2 + 2); i++)
        {
            newVerts[i] = mesh.vertices[i];
        }

        for (int i = (numSegments * 2 + 2); i < (numSegments * 2 + 2)*2; i++)
        {
            //newVerts[topIndexStart + 2 * i] = new Vector3(i * vertexDistance - size.x / 2, size.y/2, zWidth);
            //newVerts[topIndexStart + 2 * i + 1] = new Vector3(i * vertexDistance - size.x / 2, size.y / 2, zStartDist);
            
            Vector3 worldSpace = transform.position + new Vector3(((i - (numSegments * 2 + 2))/2 * vertexDistance - size.x / 2), size.y/2);
            float depth = (i - (numSegments * 2 + 2)) % 2 == 0 ? zWidth : zStartDist;
            worldSpace.y += SwayEffect.getWindEffect(worldSpace.x, windSpeed/7 * distantWavesSpeedScalar, windVolatility * distantWavesVolatilityScalar, windStrength, true) * 10 * distantWavesScalar/(Mathf.Clamp(Mathf.Abs(cam.transform.position.y - worldSpace.y) * .5f * distantWavesFalloffDistance, 1, 20 * distantWavesScalar));

            if (changeOnZoom && (!onlyChangeOnGreaterZoom || cam.orthographicSize > 5))
                newVerts[i] = new Vector3(worldSpace.x + ((worldSpace.x - cam.transform.position.x) * -depth * 5f / cam.orthographicSize) * parallaxFXScalar.x, worldSpace.y + ((worldSpace.y - cam.transform.position.y) * -depth / 5f * 5f / cam.orthographicSize) * parallaxFXScalar.y, transform.position.z) - transform.position;
            else
                newVerts[i] = new Vector3(worldSpace.x + ((worldSpace.x - cam.transform.position.x) * -depth) * parallaxFXScalar.x, worldSpace.y + ((worldSpace.y - cam.transform.position.y) * -depth / 5f) * parallaxFXScalar.y, transform.position.z) - transform.position;
        }

        mesh.vertices = newVerts;
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
        {
            if (collision.GetComponent<PlayerSubmergeTracker>() != null)
                collision.GetComponent<PlayerSubmergeTracker>().EnteringWater();
            return;
        }

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
            collision.attachedRigidbody.velocity *= .66f;
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
        {
            if (collision.GetComponent<PlayerSubmergeTracker>() != null)
                collision.GetComponent<PlayerSubmergeTracker>().LeavingWater();

            return;
        }

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
