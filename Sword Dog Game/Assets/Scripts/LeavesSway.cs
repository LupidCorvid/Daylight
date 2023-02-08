using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesSway : MonoBehaviour
{
    public float swayIntensity = 5;
    public float swaySpeed = 1;
    public float swayVolatility = .2f;

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

    Collider2D cldr;
    float lastDrop;
    float dropCooldown = 0.5f;

    public float passingEmmissionSensitivty = 2;

    public void Start()
    {
        lastRotation = transform.root.eulerAngles.z;
        particleHandler = GetComponentInChildren<ParticleSystem>();
        lastPosition = transform.position;

        cldr = GetComponent<Collider2D>();
    }

    public void FixedUpdate()
    {
        updateRotations();
        checkSpawnParticles();
    }

    ///Maybe add triggers so that it knows when something is falling through the leaves, and spawns leaves around them at their speed as they fall (so that they look like they broke through the canopy)

    public void updateRotations()
    {
        float newRotation;
        if (useVelocity)
        {
            float swayVelocity = ((Mathf.PerlinNoise(((Time.time * swaySpeed) + transform.position.x) * swayVolatility, 0) * 2) - 1) * swayIntensity;
            newRotation = lastRotation + swayVelocity * Time.deltaTime;
        }
        else
            newRotation = ((Mathf.PerlinNoise(((Time.time * swaySpeed) + transform.position.x) * swayVolatility, 0) * 2) - 1) * swayIntensity;

        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (lastRotation - newRotation));
        lastRotation = newRotation;
    }

    public void checkSpawnParticles()
    {
        if ((transform.position - lastPosition).magnitude > leavesDropSensitivity * Time.deltaTime && lastShakeDrop + ShakenDropCooldown <= Time.time)
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
        if (lastDrop + dropCooldown > Time.time)
            return;

        Vector2 nextLocation = ((cldr.ClosestPoint(collision.transform.position)) + collision.attachedRigidbody.velocity * Time.deltaTime/7);
        if (!cldr.OverlapPoint(nextLocation))
        {
            if (collision.attachedRigidbody.velocity.magnitude >= passingEmmissionSensitivty)
            {
                ParticleSystem.EmitParams particleSetter = new ParticleSystem.EmitParams();
                particleSetter.velocity = new Vector2(collision.attachedRigidbody.velocity.x * .5f, collision.attachedRigidbody.velocity.y - (9.8f * Time.deltaTime));
                particleSetter.position = (cldr.ClosestPoint(collision.transform.position) - (Vector2)transform.position);
                if (particleSetter.velocity.y < -9.8)
                    particleSetter.velocity = new Vector3(particleSetter.velocity.x, -9.8f, 0);

                ParticleSystem.EmitParams tempSetter = new ParticleSystem.EmitParams();

                int dropNum = Mathf.Clamp((int)collision.attachedRigidbody.velocity.magnitude, 0, 15);
                dropNum = (int)((dropNum * Random.Range(1, 1.5f)) * collision.bounds.extents.magnitude);
                
                for (int i = 0; i < dropNum; i++)
                {
                    tempSetter.velocity = particleSetter.velocity * Random.Range(.25f, 1.1f);
                    tempSetter.velocity += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    tempSetter.position = particleSetter.position + (Random.Range(-1f, 1f) * (collision.transform.rotation * collision.bounds.extents));
                    
                    particleHandler.Emit(tempSetter, 1);
                }
                lastDrop = Time.time;
            }
                
        }
            
    }
}
