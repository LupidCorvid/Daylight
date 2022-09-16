using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakParticle : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;

    public float drag;

    public float lifeTime = 5;
    public float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        if(startTime + lifeTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
