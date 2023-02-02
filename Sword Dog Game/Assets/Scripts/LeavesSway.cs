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

    public bool waitUntilRestingToDropAgain = false;

    public float ShakenDropCooldown;
    private float lastShakeDrop;
    private Vector3 lastLastPosition; //
    
    public void Start()
    {
        lastRotation = transform.root.eulerAngles.z;
        particleHandler = GetComponentInChildren<ParticleSystem>();
        lastPosition = transform.position;
    }

    public void FixedUpdate()
    {
        updateRotations();
        checkSpawnParticles();
    }

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
            particleHandler.Emit(velocitySetter, Random.Range(15, 30));
            lastShakeDrop = Time.time;
        }

        lastLastPosition = lastPosition;
        lastPosition = transform.position;
    }
}
