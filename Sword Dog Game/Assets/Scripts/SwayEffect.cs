using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;


[Unity.Burst.BurstCompile] //Bursts don't seem to do anything, but leaving them in since they don't seem to hurt
public unsafe class SwayEffect : MonoBehaviour
{
    public Sprite texture;
    public Color color = new Color(0, 0, 0, 1);
    Renderer rend;

    Mesh originalMesh;
    MeshFilter meshFilter;
    Mesh.MeshDataArray originalSnapshot;

    public struct swaySettings
    {
        public float swayPosition;
        public float swayVelocity;

        public float tension;
        public float dampening;

        public float windStrength;
        public float windSpeed;
        //Lower wind volatility means objects near eachother have similar swaying motion
        public float windVolatility;

        public float limit;

        public bool rotate;

        public float minMeshHeight;
        public float maxMeshHeight;

        public Mesh.MeshDataArray originalSnapshot;

        public swaySettings(SwayEffect main)
        {
            swayPosition = main.swayPosition;
            swayVelocity = main.swayVelocity;
            tension = main.tension;
            dampening = main.dampening;
            windStrength = main.windStrength;
            windSpeed = main.windSpeed;
            windVolatility = main.windVolatility;
            limit = main.limit;
            rotate = main.rotate;
            minMeshHeight = main.minMeshHeight;
            maxMeshHeight = main.maxMeshHeight;
            originalSnapshot = main.originalSnapshot;

        }
    }

    public swaySettings settings;

    public float swayPosition = 0;
    public float swayVelocity = 0;

    public float tension = 1;
    public float dampening = 1;

    public float windStrength = 1;
    public float windSpeed = 1;
    //Lower wind volatility means objects near eachother have similar swaying motion
    public float windVolatility = 0.2f;

    public float limit = 1;

    public bool rotate = false;

    //For use with SceneWindSetter
    public static float sceneStrengthScalar = 1;
    public static float sceneSpeedScalar = 1;
    public static float sceneVolatilityScalar = 1;

    //Maximum distance for deformation on the mesh
    

    public Dictionary<Rigidbody2D, Vector2> objectsWithVelocity = new Dictionary<Rigidbody2D, Vector2>();

    public SoundPlayer soundPlayer;
    public SoundSet rustleFX;

    //Used for culling
    public static Transform player;

    //Used for sound capping
    public static int windSounds = 0;
    public static int windSoundCap = 400;
    public float lastWindTime = 0;
    public static float windSoundCooldown = .1f;

    public MaterialPropertyBlock materialBlock;

    WindEffectsJob windJob;
    JobHandle windJobHandle;
    bool ranJob = false;

    public float minMeshHeight;
    public float maxMeshHeight;

    // Start is called before the first frame update
    void Start()
    {
        windSounds = 0;
        player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
        if (soundPlayer == null)
            soundPlayer = GetComponent<SoundPlayer>();
        //GetComponent<MeshRenderer>().material.mainTexture = texture.texture;
        materialBlock = new MaterialPropertyBlock();
        materialBlock.SetTexture("_MainTex", texture.texture);
        materialBlock.SetColor("_Color", color);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialBlock);
        rend = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.sharedMesh;
        originalSnapshot = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        Mesh.MeshDataArray tempData = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        meshFilter.sharedMesh = new Mesh();
        Mesh.ApplyAndDisposeWritableMeshData(tempData, meshFilter.sharedMesh);
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        minMeshHeight = originalMesh.bounds.min.y;
        maxMeshHeight = originalMesh.bounds.size.y;
        settings = new swaySettings(this);
    }

    public void OnValidate()
    {
        materialBlock = new MaterialPropertyBlock();
        materialBlock.SetTexture("_MainTex", texture.texture);
        materialBlock.SetColor("_Color", color);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialBlock);
    }

    [Unity.Burst.BurstCompile]
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

    [Unity.Burst.BurstCompile]
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
        try
        {
            meshFilter.sharedMesh = originalMesh;
            originalSnapshot.Dispose();
        }
        catch
        {
            //Only here to prevent errors from stoping debug    
        }
    }

    public void OnApplicationQuit()
    {
        meshFilter.sharedMesh = originalMesh;
        meshFilter.sharedMesh.SetVertices(originalMesh.vertices);
        try
        {
            //Mesh.ApplyAndDisposeWritableMeshData(originalSnapshot, meshFilter.sharedMesh);
        }
        catch
        {
            //Only here to prevent having to stop debugging every time this recompiles when running
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((int)Mathf.Pow(2, collision.gameObject.layer) & LayerMask.GetMask("TerrainFX", "Utility")) == 0)
        {
            float distanceModifier = 1;
            distanceModifier = Mathf.Abs((transform.position.x - collision.transform.position.x) / collision.bounds.extents.x);
            distanceModifier = Mathf.Clamp(distanceModifier, 0, 1);
            distanceModifier = 1 - distanceModifier;
            int neg = transform.localScale.x > 0 ? 1 : -1;
            float physicsVelocity = ((collision.attachedRigidbody.velocity.x) * Time.deltaTime * neg) * distanceModifier;

            if (collision.attachedRigidbody != null && objectsWithVelocity.ContainsKey(collision.attachedRigidbody))
            {
                //Affects additional force applied to grass behind the launching object
                const float PUSH_BACK_STRENGTH = 1.0f / 130;
                //Affects all grass around the launching or landing object
                const float PUSH_AWAY_STRENGTH = 1.0f / 115;

                distanceModifier = (transform.position.x - collision.transform.position.x) / collision.bounds.extents.x;
                distanceModifier = Mathf.Clamp(distanceModifier, -1, 1);
                distanceModifier += distanceModifier > 0 ? -1 : 1;
                distanceModifier *= -1;
                physicsVelocity += Mathf.Abs(collision.attachedRigidbody.velocity.y - objectsWithVelocity[collision.attachedRigidbody].y) * PUSH_AWAY_STRENGTH * distanceModifier;

                //For pushing grass behind something being launched
                if (collision.attachedRigidbody.velocity.y - objectsWithVelocity[collision.attachedRigidbody].y > .25f)
                {
                    if (transform.position.x - collision.transform.position.x < 0 && collision.attachedRigidbody.velocity.x > 0
                        || transform.position.x - collision.transform.position.x > 0 && collision.attachedRigidbody.velocity.x < 0)
                    {
                        physicsVelocity += Mathf.Abs(collision.attachedRigidbody.velocity.y - objectsWithVelocity[collision.attachedRigidbody].y) * PUSH_BACK_STRENGTH * distanceModifier
                                        * Mathf.Abs(collision.attachedRigidbody.velocity.x);

                    }
                }

                objectsWithVelocity[collision.attachedRigidbody] = collision.attachedRigidbody.velocity;
            }
            swayVelocity += physicsVelocity;
            if (Mathf.Abs(physicsVelocity) > 0.01f)
            {
                PlayWindSound(Mathf.Abs(physicsVelocity / Time.fixedDeltaTime * 0.075f * 13f / 3 * .5f));
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

    //[Unity.Burst.BurstCompile]
    public static float getWindEffect(float xPos, float windSpeed, float windVolatility, float windStrength)
    {
        //return Mathf.PerlinNoise(Time.time * windSpeed * (windStrength > 0 ? -1 : 1) + xPos * windVolatility, 0) * Time.fixedDeltaTime * windStrength;
        //THIS IS THE ONE THAT COMPILES, but does not improve performance much
        //return noise.cnoise(new float2(Time.time * windSpeed * (windStrength > 0 ? -1 : 1) + xPos * windVolatility, 0)) * Time.fixedDeltaTime * windStrength;
        //Doesn't work, scalars must be read only
        // return noise.cnoise(new float2(((Time.time * windSpeed * (windStrength > 0 ? -1 : 1) * sceneSpeedScalar) + xPos) * windVolatility * sceneVolatilityScalar, 0)) * Time.fixedDeltaTime * windStrength * sceneStrengthScalar;
        //This DOES NOT work with burst compile, but its what works/looks good for now
        return Mathf.PerlinNoise(((Time.time * windSpeed * (windStrength > 0 ? -1 : 1) * sceneSpeedScalar) + (xPos)) * windVolatility * sceneVolatilityScalar, 0) * Time.fixedDeltaTime * windStrength * sceneStrengthScalar;
    }

    //[Unity.Burst.BurstCompile]
    //private void FixedUpdate()
    //{
    //    //Culling
    //    //player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
    //    //if (player != null && ((player.position - transform.position).x > 25 || (player.position - transform.position).y > 15))
    //    //    return;
    //    //New culling
    //    if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > 20 * Camera.main.orthographicSize/6)
    //        return;

    //    //Wind direction makes it so that wind rolls in the same direction as things are bending
    //    int windDirection = (windStrength * sceneStrengthScalar > 0 ? -1 : 1);
    //    //Changed Time.deltaTime to Time.fixedTime to reflect that this is in FixedUpdate

    //    //float windEffect = Mathf.PerlinNoise(((Time.time * windSpeed * windDirection * sceneSpeedScalar) + (transform.position.x)) * windVolatility * sceneVolatilityScalar, 0) * Time.fixedDeltaTime * windStrength * sceneStrengthScalar;
    //    float windEffect = getWindEffect(transform.position.x, windSpeed, windVolatility, windStrength);
    //    swayVelocity += windEffect;
    //    swayVelocity += tension * (-swayPosition) - swayVelocity * dampening;
    //    swayPosition += swayVelocity;

    //    if(Mathf.Abs(swayPosition) > limit)
    //    {
    //        if (swayPosition > 0)
    //            swayPosition = limit;
    //        else
    //            swayPosition = -limit;

    //        //swayVelocity /= limit;
    //    }
    //    if (rend.isVisible)
    //    {
    //        if (rotate)
    //            swayRotate(swayPosition);
    //        else
    //            sway(swayPosition);
    //    }

    //    //Needs optimization
    //    //if (windEffect/Time.fixedDeltaTime * 10/Mathf.Abs(windStrength) > 3 && windSoundCooldown >= windSoundCooldownMax && windSounds < windSoundCap)
    //    //    PlayWindSound(windEffect / Time.fixedDeltaTime * 0.04f);
    //    if (Mathf.Abs(windEffect) / Time.fixedDeltaTime > 1.5f && windSoundCooldown + lastWindTime <= Time.time && windSounds < windSoundCap)
    //    {
    //        float dampen = MainMenuManager.inMainMenu ? 0.25f : 1f;
    //        PlayWindSound(windEffect / Time.fixedDeltaTime * 0.04f * dampen);
    //        lastWindTime = Time.time;
    //    }

    //    //if (windSoundCooldown < windSoundCooldownMax)
    //    //    windSoundCooldown += Time.fixedDeltaTime;
    //}

    public void Update()
    {

        player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null && ((player.position - transform.position).x > 25 || (player.position - transform.position).y > 15))
            return;
        //windJob = new WindEffectsJob(windStrength, windSpeed, windVolatility, transform.position.x, originalSnapshot, maxMeshHeight, minMeshHeight, tension, dampening, limit, swayPosition, swayVelocity, Time.time, Time.deltaTime, rotate);
        unsafe
        {
            windJob = new WindEffectsJob(settings, Time.time, Time.deltaTime, transform.position.x);
        }
        //windJob.Execute();
        windJobHandle = windJob.Schedule();
        ranJob = true;

    }

    public void LateUpdate()
    {
        if (!ranJob)
            return;
        windJobHandle.Complete();
        swayPosition = windJob.swayPosition;
        swayVelocity = windJob.swayVelocity;
        //meshFilter.sharedMesh.MarkDynamic();
        meshFilter.sharedMesh.SetVertices(windJob.vertices);

        windJob.vertices.Dispose();
        ranJob = false;

    }

    private void PlayWindSound(float volume)
    {
        if (soundPlayer == null)
            return;

        if (rustleFX != null)
        {
            soundPlayer.PlaySound(rustleFX, volume);
            windSounds++;
            Invoke("EndWindSound", rustleFX.length());
            windSoundCooldown = 0.0f;
        }
    }

    private void EndWindSound()
    {
        if (windSounds > 0)
            windSounds--;
    }


    public unsafe struct WindEffectsJob : IJob
    {
        
        //[ReadOnly]
        //public float strength;
        //[ReadOnly]
        //public float speed;
        //[ReadOnly]
        //public float volatility;

        //[ReadOnly]
        //public float limit;

        
        //[ReadOnly]
        //public float tension;
        //[ReadOnly]
        //public float dampening;

        //[ReadOnly]
        //public Mesh.MeshDataArray originalMeshData;
        //[ReadOnly]
        //public float meshHeight;
        //[ReadOnly]
        //public float meshMinHeight;

        //[ReadOnly]
        //public bool swayRotate;

        [Unity.Collections.LowLevel.Unsafe.NativeDisableUnsafePtrRestriction]
        public swaySettings *main;


        public NativeArray<Vector3> vertices;
        

        [ReadOnly]
        public float time;
        [ReadOnly]
        public float deltaTime;

        [ReadOnly]
        public float xPos;

        public float swayPosition;
        public float swayVelocity;


        //public WindEffectsJob(float windStrength, float windSpeed, float windVolatility, float xPosition, Mesh.MeshDataArray meshData, float meshMaxHeight, float meshMinHeight, float tension, float dampening, float limit, float swayPosition, float swayVelocity, float time, float deltaTime, bool swayRotate = false)
        //{
        //    strength = windStrength;
        //    speed = windSpeed;
        //    volatility = windVolatility;
        //    xPos = xPosition;
        //    originalMeshData = meshData;
        //    meshHeight = meshMaxHeight;
        //    this.meshMinHeight = meshMinHeight;
        //    this.tension = tension;
        //    this.dampening = dampening;
        //    this.swayPosition = swayPosition;
        //    this.swayVelocity = swayVelocity;
        //    this.limit = limit;
        //    this.swayRotate = swayRotate;
        //    vertices = new NativeArray<Vector3>(originalMeshData[0].vertexCount, Allocator.TempJob);
        //    this.time = time;
        //    this.deltaTime = deltaTime;
        //}

        public WindEffectsJob(swaySettings* main, float Time, float deltaTime, float xPos)
        {
            this.main = main;
            this.time = Time;
            this.deltaTime = deltaTime;
            this.xPos = xPos;

            swayPosition = (*main).swayPosition;
            swayVelocity = (*main).swayVelocity;

            vertices = new NativeArray<Vector3>((*main).originalSnapshot[0].vertexCount, Allocator.TempJob);
        }

        public void Execute()
        {
            float windEffect = Mathf.PerlinNoise(((time * (*main).windSpeed * ((*main).windStrength > 0 ? -1 : 1) * sceneSpeedScalar) + (xPos)) * (*main).windVolatility * sceneVolatilityScalar, 0) * deltaTime * main.windStrength * sceneStrengthScalar;
            swayVelocity += windEffect;
            swayVelocity += (*main).tension * (-swayPosition) - swayVelocity * (*main).dampening;
            swayPosition += swayVelocity;

            if (Mathf.Abs(swayPosition) > (*main).limit)
            {
                if (swayPosition > 0)
                    swayPosition = (*main).limit;
                else
                    swayPosition = -(*main).limit;

                //swayVelocity /= limit;
            }

            if ((*main).rotate)
            {
                SwayRotate(swayPosition);
            }
            else
            {
                Sway(swayPosition);
            }
        }

        public void Sway(float intensity)
        {

            (*main).originalSnapshot[0].GetVertices(vertices);
            //Vector3[] newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float height = (vertices[i].y - (*main).minMeshHeight) / (*main).maxMeshHeight;
                vertices[i] = new Vector3((Mathf.Lerp(0, (*main).maxMeshHeight, height) * intensity) + vertices[i].x, vertices[i].y, vertices[i].z);

            }
            //meshFilter.sharedMesh.SetVertices(newVertices);
        }

        public void SwayRotate(float intensity)
        {

            (*main).originalSnapshot[0].GetVertices(vertices);
            for (int i = 0; i < vertices.Length; i++)
            {
                float height = (vertices[i].y - (*main).minMeshHeight) / (*main).maxMeshHeight;
                vertices[i] = new Vector3((Mathf.Lerp(0, (*main).maxMeshHeight, height) * intensity) + vertices[i].x, vertices[i].y, vertices[i].z);
                float angle = Mathf.Atan2(vertices[i].y, vertices[i].x);
                //newVertices[i].y = Mathf.Sin(angle);
                vertices[i] = new Vector3(vertices[i].x, Mathf.Sin(angle), vertices[i].z);
            }
        }

    }
}
