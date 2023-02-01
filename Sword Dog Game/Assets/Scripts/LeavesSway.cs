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

    public void Start()
    {
        lastRotation = transform.root.eulerAngles.z;
    }

    public void FixedUpdate()
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
}
