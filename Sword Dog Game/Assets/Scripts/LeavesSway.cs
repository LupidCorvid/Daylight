using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesSway : MonoBehaviour
{
    public float swayIntensity = 5;
    public float swaySpeed = 1;
    public float swayVolatility = .2f;

    //For adjusting numbers with SceneWindSetter
    public static float sceneIntensityScalar = 1;
    public static float sceneSpeedScalar = 1;
    public static float sceneVolatilityScalar = 1;

    public float lastRotation = 0;

    public float swayVelocity = 0;

    public bool useVelocity = true;

    public ParticleSystem particleHandler;

    private Vector3 lastPosition;

    public float leavesDropSensitivity = 1;

    public bool waitUntilRestingToDropAgain = true;

    public float ShakenDropCooldown = 5;
    private float lastShakeDrop;
    private Vector3 lastLastPosition; //

    public int minImpactDrop = 10;
    public int maxImpactDrop = 20;

    public Collider2D cldr;
    float lastDrop = 1f;
    float dropCooldown = 0.05f;

    public float passingEmmissionSensitivity = 2;
    public bool Group;
    private List<LeavesSway> groupMembers = new List<LeavesSway>();

    public SpriteRenderer sprite;

    public bool onExitEmit = true;

    public float lastEmptyParticleDrop;

    public bool disableLeaves;

    public void Start()
    {
        lastShakeDrop = Time.time - 1.5f;
        lastRotation = transform.root.eulerAngles.z;
        particleHandler = GetComponentInChildren<ParticleSystem>();
        lastPosition = transform.position;

        cldr = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (Group)
            groupMembers.AddRange(transform.parent.GetComponentsInChildren<LeavesSway>());
        if (disableLeaves)
        {
            ParticleSystem.EmissionModule tester = particleHandler.emission;
            tester.rateOverTime = 0;
        }
            

    }

    public void Update()
    {
        //Done to make sure that the particle bounds always include all particles, as the particle bounds being offset from all particles made it so that particles would disappear while still on screen.
        if(Time.time >= lastEmptyParticleDrop + .09f)
        {
            ParticleSystem.EmitParams velocitySetter = new ParticleSystem.EmitParams()
            {
                startLifetime = .1f,
                position = Vector3.zero
            };
            particleHandler?.Emit(velocitySetter, 1);
            lastEmptyParticleDrop = Time.time;
        }
    }

    public void FixedUpdate()
    {
        updateRotations();
        checkSpawnParticles();
    }

    ///Maybe add triggers so that it knows when something is falling through the leaves, and spawns leaves around them at their speed as they fall (so that they look like they broke through the canopy)
    [Unity.Burst.BurstCompile]
    public void updateRotations()
    {
        if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > 25 || !SettingsManager.currentSettings.GrassSway)
            return;

        float newRotation;
        if (useVelocity)
        {
            //float swayVelocity = ((Mathf.PerlinNoise(((Time.time * swaySpeed * sceneSpeedScalar) + transform.position.x) * swayVolatility * sceneVolatilityScalar, 0) * 2) - 1) * swayIntensity * sceneIntensityScalar;
            float swayVelocity = ((SwayEffect.getWindEffect(transform.position.x,swaySpeed,swayVolatility, swayIntensity) * 2) - 1);
            newRotation = lastRotation + swayVelocity * Time.deltaTime;
        }
        else
            newRotation = ((SwayEffect.getWindEffect(transform.position.x, swaySpeed, swayVolatility, swayIntensity) * 2) - 1);

        sprite.transform.rotation = Quaternion.Euler(0, 0, sprite.transform.rotation.eulerAngles.z + (lastRotation - newRotation));
        lastRotation = newRotation;
    }

    public void checkSpawnParticles()
    {
        if (lastShakeDrop + ShakenDropCooldown <= Time.time && (transform.position - lastPosition).magnitude > leavesDropSensitivity * Time.deltaTime)
        {
            //particleHandler.Emit(Mathf.Clamp((int)(Random.Range(15, 30) * (transform.position - lastPosition).magnitude), 1, 30));
            ParticleSystem.EmitParams velocitySetter = new ParticleSystem.EmitParams();
            //Might not want? Should try to simulate inertia, but because of the spring motion some leaves are launched upward
            velocitySetter.velocity = (transform.position - lastLastPosition) + (Vector3.down * .04f);
            particleHandler.Emit(velocitySetter, Random.Range(minImpactDrop, maxImpactDrop));
            lastShakeDrop = Time.time;
        }

        lastLastPosition = lastPosition;
        lastPosition = transform.position;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!onExitEmit)
            return;

        if (Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("DamageArea") || Mathf.Pow(2, collision.gameObject.layer) == LayerMask.GetMask("Utility") || collision.gameObject.CompareTag("PassingLeavesBlacklist"))
            return;

        if (lastDrop + dropCooldown > Time.time)
            return;

        
        if (!cldr.OverlapPoint(collision.transform.position))
            return;

        Vector2 nextLocation = ((cldr.ClosestPoint(collision.transform.position)) + collision.attachedRigidbody.velocity * Time.deltaTime/1);//1 was 7
        if (!cldr.OverlapPoint(nextLocation))
        {
            if(Group)
            {
                foreach(LeavesSway sway in groupMembers)
                {
                    if (sway.cldr.OverlapPoint(nextLocation))
                        return;
                    if (sway.cldr.OverlapPoint(nextLocation + collision.attachedRigidbody.velocity * .25f))
                        return;
                }
            }

            if (collision.attachedRigidbody.velocity.magnitude >= passingEmmissionSensitivity)
            {
                ParticleSystem.EmitParams particleSetter = new ParticleSystem.EmitParams();
                particleSetter.velocity = new Vector2(collision.attachedRigidbody.velocity.x * .5f, .5f * collision.attachedRigidbody.velocity.y - (9.8f * Time.deltaTime));
                particleSetter.position = (cldr.ClosestPoint(collision.transform.position) - (Vector2)transform.position);
                if (particleSetter.velocity.y < -9.8)
                    particleSetter.velocity = new Vector3(particleSetter.velocity.x, -9.8f, 0);

                ParticleSystem.EmitParams tempSetter = new ParticleSystem.EmitParams();

                int dropNum = Mathf.Clamp((int)collision.attachedRigidbody.velocity.magnitude, 0, 15);
                
                dropNum = (int)((dropNum * Random.Range(1, 1.5f)) * collision.bounds.extents.magnitude/1f);
                if (collision.bounds.extents.magnitude >= 1.5f)
                    dropNum = dropNum / 2;

                for (int i = 0; i < dropNum; i++)
                {
                    tempSetter.velocity = particleSetter.velocity * Random.Range(.25f, 1.1f);
                    tempSetter.velocity += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    tempSetter.position = particleSetter.position + (Random.Range(-1.2f, 1.2f) * (collision.transform.rotation * collision.bounds.extents));
                    
                    particleHandler?.Emit(tempSetter, 1);
                }

                lastDrop = Time.time;
            }
                
        }
            
    }
}
