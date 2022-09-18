using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class SwayEffect : MonoBehaviour
{
    public Sprite texture;
    Renderer rend;

    Mesh originalMesh;
    MeshFilter meshFilter;
    Mesh.MeshDataArray originalSnapshot;

    private float swayPosition = 1;
    private float swayVelocity = 0;

    public float tension = 1;
    public float dampening = 1;

    public float windStrength = 1;
    public float windSpeed = 1;
    //Lower wind volatility means objects near eachother have similar swaying motion
    public float windVolatility = 0.2f;

    //Maximum distance for deformation on the mesh
    public float limit = 1;

    public bool rotate = false;

    public Dictionary<Rigidbody2D, Vector2> objectsWithVelocity = new Dictionary<Rigidbody2D, Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = texture.texture;
        rend = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.sharedMesh;
        originalSnapshot = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        Mesh.MeshDataArray tempData = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        meshFilter.sharedMesh = new Mesh();
        Mesh.ApplyAndDisposeWritableMeshData(tempData, meshFilter.sharedMesh);
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
    }

    public void sway(float intensity)
    {
        NativeArray<Vector3> vertices = new NativeArray<Vector3>(originalSnapshot[0].vertexCount, Allocator.Temp);
        originalSnapshot[0].GetVertices(vertices);
        Vector3[] newVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float height = (originalMesh.vertices[i].y - originalMesh.bounds.min.y) / originalMesh.bounds.size.y;
            newVertices[i] = new Vector3((Mathf.Lerp(0, originalMesh.bounds.size.y, height) * intensity) + vertices[i].x, vertices[i].y, vertices[i].z);
        }
        meshFilter.sharedMesh.SetVertices(newVertices);
    }
    public void swayRotate(float intensity)
    {
        NativeArray<Vector3> vertices = new NativeArray<Vector3>(originalSnapshot[0].vertexCount, Allocator.Temp);
        originalSnapshot[0].GetVertices(vertices);
        Vector3[] newVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float height = (originalMesh.vertices[i].y - originalMesh.bounds.min.y) / originalMesh.bounds.size.y;
            newVertices[i] = new Vector3((Mathf.Lerp(0, originalMesh.bounds.size.y, height) * intensity) + vertices[i].x, vertices[i].y, vertices[i].z);
            float angle = Mathf.Atan2(newVertices[i].y, newVertices[i].x);
            newVertices[i].y = Mathf.Sin(angle);
        }

        meshFilter.sharedMesh.SetVertices(newVertices);
    }
    public void OnDestroy()
    {
        meshFilter.sharedMesh = originalMesh;
    }
    public void OnApplicationQuit()
    {
        meshFilter.sharedMesh = originalMesh;
        meshFilter.sharedMesh.SetVertices(originalMesh.vertices);
        Mesh.ApplyAndDisposeWritableMeshData(originalSnapshot, meshFilter.sharedMesh);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.GetMask("TerrainFX"))
        {
            float distanceModifier = 1;
            distanceModifier = Mathf.Abs((transform.position.x - collision.transform.position.x)/collision.bounds.extents.x);
            distanceModifier = Mathf.Clamp(distanceModifier, 0, 1);
            distanceModifier = 1 - distanceModifier;
            swayVelocity += ((collision.attachedRigidbody.velocity.x) * Time.deltaTime) * distanceModifier;
            
            if(collision.attachedRigidbody != null && objectsWithVelocity.ContainsKey(collision.attachedRigidbody))
            {
                distanceModifier = (transform.position.x - collision.transform.position.x)/collision.bounds.extents.x;
                distanceModifier = Mathf.Clamp(distanceModifier, -1, 1);
                distanceModifier += distanceModifier > 0 ? -1 : 1;
                distanceModifier *= -1;
                swayVelocity += Mathf.Abs(objectsWithVelocity[collision.attachedRigidbody].y - collision.attachedRigidbody.velocity.y)/75 * distanceModifier;
                objectsWithVelocity[collision.attachedRigidbody] = collision.attachedRigidbody.velocity;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.GetMask("TerrainFX") && collision.attachedRigidbody != null && !objectsWithVelocity.ContainsKey(collision.attachedRigidbody)) 
        {
            objectsWithVelocity.Add(collision.attachedRigidbody, collision.attachedRigidbody.velocity);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.GetMask("TerrainFX") && collision.attachedRigidbody != null && objectsWithVelocity.ContainsKey(collision.attachedRigidbody))
        {
            objectsWithVelocity.Remove(collision.attachedRigidbody);
        }
    }

    private void FixedUpdate()
    {
        //Wind direction makes it so that wind rolls in the same direction as things are bending
        int windDirection = (windStrength > 0 ? -1 : 1);
        swayVelocity += Mathf.PerlinNoise(((Time.time * windSpeed * windDirection) + (transform.position.x)) * windVolatility, 0) * Time.deltaTime * windStrength;
        swayVelocity += tension * (-swayPosition) - swayVelocity * dampening;
        swayPosition += swayVelocity;

        if(Mathf.Abs(swayPosition) > limit)
        {
            if (swayPosition > 0)
                swayPosition = limit;
            else
                swayPosition = -limit;
            swayVelocity /= limit;
        }
        if (rend.isVisible)
        {
            if (rotate)
                swayRotate(swayPosition);
            else
                sway(swayPosition);
        }
    }
}
