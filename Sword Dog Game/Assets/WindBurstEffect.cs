using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBurstEffect : MonoBehaviour
{
    public ParticleSystemForceField particleFF;
    public CollisionsTracker collisions;
    public CircleCollider2D cldr;

    public float radius;
    public float lifeTime = .5f;
    public float strength;

    float startTime;

    float lastRange;

    // Start is called before the first frame update
    void Start()
    {
        particleFF.gravity = -strength;
        startTime = Time.time;
        cldr.radius = radius;
    }

    // Update is called once per frame
    void Update()
    {

        float range = (radius * (Time.time - startTime) / lifeTime);
        float curStrength = -strength / (Mathf.Pow(range, 2) * Mathf.PI);
        if (Time.time > lifeTime + startTime)
        {
            Destroy(gameObject);
            return;
        }

        particleFF.startRange = range;
        particleFF.endRange = particleFF.startRange + .5f;

        particleFF.gravity = curStrength;

        foreach(Collider2D collision in collisions.triggersInContact)
        {
            float distance = Vector2.Distance(collision.transform.position, transform.position);
            if (!(distance > lastRange &&  distance <= range))
            {
                continue;
            }
            SwayEffect swayer = collision.GetComponent<SwayEffect>();

            if (swayer != null)
            {
                swayer.swayVelocity += (swayer.transform.position - transform.position).normalized.x * strength;
            }

            VineSegment vine = collision.GetComponent<VineSegment>();

            if(vine != null && vine?.rb != null && vine?.transform != null)
            {
                vine.rb.AddForce(((Vector2)(vine.transform.position - transform.position)).normalized * strength);
            }

        }

        lastRange = range;
    }
}
