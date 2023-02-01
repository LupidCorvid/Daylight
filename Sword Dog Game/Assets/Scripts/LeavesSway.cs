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

    bool rested = true;

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
        if((transform.position - lastPosition).magnitude > leavesDropSensitivity * Time.deltaTime)
        {
            particleHandler.Emit(Mathf.Clamp((int)(Random.Range(15, 30) * (transform.position - lastPosition).magnitude), 1, 30));
            rested = false;
        }
        if ((transform.position - lastPosition).magnitude <= .025)
            rested = true;

        lastPosition = transform.position;
    }
}
